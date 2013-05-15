//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Apollo.Core.Host.Scripting;
using Apollo.Core.Host.UserInterfaces.Application;
using Apollo.UI.Console.Nuclei;
using Apollo.UI.Console.Properties;
using Autofac;
using Mono.Options;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using EmbeddedResourceExtracter = Nuclei.EmbeddedResourceExtracter;

namespace Apollo.UI.Console
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1400:AccessModifierMustBeDeclared",
            Justification = "Access modifiers should not be declared on the entry point for a command line application. See FxCop.")]
    static class Program
    {
        /// <summary>
        /// The exit code used when the application has been provided with one or more invalid
        /// command line parameters.
        /// </summary>
        private const int InvalidCommandLineParametersExitCode = 2;

        /// <summary>
        /// The exit code used when the application has shown the help information.
        /// </summary>
        private const int HelpShownExitCode = 3;

        /// <summary>
        /// The exit code used when the application has been provided with an invalid script file.
        /// </summary>
        private const int InvalidScriptFileExitCode = 4;

        /// <summary>
        /// The default name for the error log.
        /// </summary>
        private const string DefaultErrorFileName = "console.error.{0}.log";

        /// <summary>
        /// The event that is used to signal the application that it is safe to shut down.
        /// </summary>
        private static readonly AutoResetEvent s_ShutdownEvent
            = new AutoResetEvent(false);

        /// <summary>
        /// The full path of the file that contains the script that should be run.
        /// </summary>
        private static string s_ScriptFile;

        /// <summary>
        /// A flag indicating if the help information for the application should be displayed.
        /// </summary>
        private static bool s_ShouldDisplayHelp;

        /// <summary>
        /// The UI dependency injection container.
        /// </summary>
        private static IContainer s_UiContainer;

        /// <summary>
        /// The object that hosts the scripting engine.
        /// </summary>
        private static IHostScripts s_ScriptHost;

        /// <summary>
        /// The object that provides the diagnostics methods for the application.
        /// </summary>
        private static SystemDiagnostics s_Diagnostics;

        /// <summary>
        /// The object that provides access to the application commands.
        /// </summary>
        private static IAbstractApplications s_ApplicationFacade;

        /// <summary>
        /// The main entry point for the dataset application.
        /// </summary>
        /// <param name="args">
        /// The array containing the start-up arguments for the application.
        /// </param>
        /// <returns>A value indicating if the process exited normally (0) or abnormally (&gt; 0).</returns>
        [STAThread]
        static int Main(string[] args)
        {
            {
                Debug.Assert(args != null, "The arguments array should not be null.");
            }

            Func<int> applicationLogic = () => RunApplication(args);

            var eventLogSource = Assembly.GetExecutingAssembly().GetName().Name;
            return CommandLineProgram.EntryPoint(
                applicationLogic,
                eventLogSource,
                string.Format(
                    CultureInfo.InvariantCulture,
                    DefaultErrorFileName,
                    Process.GetCurrentProcess().Id));
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "We're catching the exception and then exiting the application.")]
        private static int RunApplication(string[] arguments)
        {
            try
            {
                ShowHeader();
                LoadKernel();
                LogStartup();

                var options = CreateOptionSet();
                try
                {
                    options.Parse(arguments);
                }
                catch (OptionException e)
                {
                    s_Diagnostics.Log(
                        LevelToLog.Fatal,
                        ConsoleConstants.LogPrefix,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Log_Error_InvalidInputParameters_WithException,
                            e));

                    WriteErrorToConsole(Resources.Output_Error_InvalidInput);
                    return InvalidCommandLineParametersExitCode;
                }

                if (string.IsNullOrWhiteSpace(s_ScriptFile) || s_ShouldDisplayHelp)
                {
                    ShowHelp(options);
                    return HelpShownExitCode;
                }

                if (!File.Exists(s_ScriptFile))
                {
                    s_Diagnostics.Log(
                        LevelToLog.Fatal,
                        ConsoleConstants.LogPrefix,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Log_Error_ScripFileDoesNotExist,
                            s_ScriptFile));

                    WriteErrorToConsole(Resources.Output_Error_ScriptFileDoesNotExist);
                    return InvalidScriptFileExitCode;
                }

                WriteInputParametersToLog(s_ScriptFile);
                System.Console.WriteLine(Resources.Output_Information_LoadingScriptFile);

                var text = string.Empty;
                using (var reader = new StreamReader(s_ScriptFile))
                {
                    text = reader.ReadToEnd();
                }

                System.Console.WriteLine(Resources.Output_Information_ExecutingScript);
                var executionPair = s_ScriptHost.Execute(SelectScriptLanguageFromFileExtension(s_ScriptFile), text, System.Console.Out);
                try
                {
                    CancellationTokenSource cancellationSource = executionPair.Item2;
                    System.Console.CancelKeyPress += (s, e) =>
                        {
                            var source = cancellationSource;
                            if (source != null)
                            {
                                source.Cancel();
                            }
                        };

                    executionPair.Item1.Wait();
                    return CommandLineProgram.NormalApplicationExitCode;
                }
                catch (AggregateException e)
                {
                    s_Diagnostics.Log(
                        LevelToLog.Fatal,
                        ConsoleConstants.LogPrefix,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Log_Error_ProcessingError_WithException,
                            e));

                    WriteErrorToConsole(Resources.Output_Error_WhileProcessing);
                    return CommandLineProgram.UnhandledExceptionApplicationExitCode;
                }
            }
            finally
            {
                if (s_ApplicationFacade != null)
                {
                    s_ApplicationFacade.Shutdown();
                }

                s_ShutdownEvent.WaitOne();
                if (s_UiContainer != null)
                {
                    s_UiContainer.Dispose();
                }
            }
        }

        private static void ShowHeader()
        {
            System.Console.WriteLine(
                string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.Header_ApplicationAndVersion,
                    GetVersion()));
            System.Console.WriteLine(GetCopyright());
            System.Console.WriteLine(GetLibraryLicenses());
        }

        private static void ShowHelp(OptionSet argProcessor)
        {
            System.Console.WriteLine(Resources.Help_Usage_Intro);
            System.Console.WriteLine();
            argProcessor.WriteOptionDescriptions(System.Console.Out);
        }

        private static void WriteErrorToConsole(string errorText)
        {
            try
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine(errorText);
            }
            finally
            {
                System.Console.ResetColor();
            }
        }

        private static string GetVersion()
        {
            var attribute = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false);
            Debug.Assert(attribute.Length == 1, "There should be a copyright attribute.");

            return (attribute[0] as AssemblyFileVersionAttribute).Version;
        }

        private static string GetCopyright()
        {
            var attribute = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            Debug.Assert(attribute.Length == 1, "There should be a copyright attribute.");

            return (attribute[0] as AssemblyCopyrightAttribute).Copyright;
        }

        private static string GetLibraryLicenses()
        {
            var licenseXml = EmbeddedResourceExtracter.LoadEmbeddedTextFile(
                Assembly.GetExecutingAssembly(),
                @"Apollo.UI.Console.Properties.licenses.xml");
            var doc = XDocument.Parse(licenseXml);
            var licenses = from element in doc.Descendants("package")
                           select new
                           {
                               Id = element.Element("id").Value,
                               Version = element.Element("version").Value,
                               Source = element.Element("url").Value,
                               License = element.Element("licenseurl").Value,
                           };

            var builder = new StringBuilder();
            builder.AppendLine(Resources.Header_OtherPackages_Intro);
            foreach (var license in licenses)
            {
                builder.AppendLine(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.Header_OtherPackages_IdAndLicense,
                        license.Id,
                        license.Version,
                        license.Source));
            }

            return builder.ToString();
        }

        /// <summary>
        /// Loads the kernel.
        /// </summary>
        private static void LoadKernel()
        {
            // At a later stage we need to clean this up.
            // there are two constants and a DI reference.
            var bootstrapper = new KernelBootstrapper(
                s_ShutdownEvent,
                module =>
                {
                    var builder = new ContainerBuilder();
                    {
                        builder.RegisterModule(module);
                        builder.RegisterModule(new UtilitiesModule());
                    }

                    s_UiContainer = builder.Build();
                    s_ScriptHost = s_UiContainer.Resolve<IHostScripts>();
                    s_Diagnostics = s_UiContainer.Resolve<SystemDiagnostics>();
                    s_ApplicationFacade = s_UiContainer.Resolve<IAbstractApplications>();
                });

            // Load the core system. This will automatically
            // run the UI bootstrapper which will then
            // load up the UI and display it.
            bootstrapper.Load();
        }

        private static void LogStartup()
        {
            s_Diagnostics.Log(
                LevelToLog.Info,
                ConsoleConstants.LogPrefix,
                string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.Log_Information_ApplicationAndVersion,
                    GetVersion()));
        }

        private static OptionSet CreateOptionSet()
        {
            var options = new OptionSet 
                {
                    { 
                        Resources.CommandLine_Options_Help_Key, 
                        Resources.CommandLine_Options_Help_Description, 
                        v => s_ShouldDisplayHelp = v != null
                    },
                    {
                        Resources.CommandLine_Options_ScriptFile_Key,
                        Resources.CommandLine_Options_ScriptFile_Description,
                        v => s_ScriptFile = v
                    },
                };
            return options;
        }

        private static void WriteInputParametersToLog(string scriptFile)
        {
            s_Diagnostics.Log(
                LevelToLog.Trace,
                ConsoleConstants.LogPrefix,
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.Log_Information_InputParameterScriptFile,
                    scriptFile));
        }

        private static ScriptLanguage SelectScriptLanguageFromFileExtension(string scriptFile)
        {
            var extension = Path.GetExtension(scriptFile);
            switch (extension)
            {
                case ScriptingConstants.PythonFileExtension:
                    return ScriptLanguage.IronPython;
                case ScriptingConstants.RubyFileExtension:
                    return ScriptLanguage.IronRuby;
                case ScriptingConstants.PowershellFileExtension:
                    return ScriptLanguage.PowerShell;
                default:
                    return ScriptLanguage.None;
            }
        }
    }
}
