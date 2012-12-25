//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;
using Apollo.Core.Base.Communication;
using Apollo.Utilities.Applications;
using Autofac;
using Mono.Options;

namespace Apollo.Core.Dataset
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [ExcludeFromCodeCoverage]
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1400:AccessModifierMustBeDeclared",
            Justification = "Access modifiers should not be declared on the entry point for a command line application. See FxCop.")]
    static class Program
    {
        /// <summary>
        /// The default name for the error log.
        /// </summary>
        private const string DefaultErrorFileName = "dataset.error.{0}.log";

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

            System.Threading.Thread.Sleep(3000);

            Func<int> applicationLogic =
                () =>
                {
                    var context = new ApplicationContext();
                    return RunApplication(args, context);
                };

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
        private static int RunApplication(string[] arguments, ApplicationContext context)
        {
            string hostId = null;
            string channelType = null;
            string channelUri = null;

            // Parse the command line options
            var options = new OptionSet 
                {
                    { 
                        "h=|host=", 
                        "The {ENDPOINTID} of the host application that requested the start of this application.", 
                        v => hostId = v
                    },
                    {
                        "t=|channeltype=",
                        "The {TYPE} of the channel over which the connection should be made.",
                        v => channelType = v
                    },
                    {
                        "u=|channeluri=",
                        "The {URI} of the connection that can be used to connect to the host application.",
                        v => channelUri = v
                    },
                };

            options.Parse(arguments);
            if (string.IsNullOrWhiteSpace(hostId) ||
                string.IsNullOrWhiteSpace(channelType) ||
                string.IsNullOrWhiteSpace(channelUri))
            {
                throw new InvalidCommandLineArgumentsException();
            }

            // Load the communication system and get it going
            // All startables are automatically started once the container gets
            // build.
            // The container will stay in scope as long as the app runs, because
            // we won't exit the app until we exit this method, which happens
            // when we exit the main message loop.
            var container = DependencyInjection.Load(context);

            // Notify the host app that we're alive, after which the 
            // rest of the app should pick up the loading of the dataset etc.
            // Note that this call to the container also registers all global commands and initializes the
            // communication channels etc.
            var resolver = container.Resolve<Action<string, string, string>>();
            resolver(hostId, channelType, channelUri);

            // Connect to the assembly resolve event so that we can intercept assembly loads for plugins etc.
            var assemblyResolver = container.Resolve<PluginLoadingAssemblyResolver>(
                new TypedParameter(
                    typeof(EndpointId), 
                    EndpointIdExtensions.Deserialize(hostId)));
            AppDomain.CurrentDomain.AssemblyResolve += assemblyResolver.LocatePluginAssembly;

            // Start with the message processing loop and then we 
            // wait for it to either get terminated or until we kill ourselves.
            Application.Run(context);
            return CommandLineProgram.NormalApplicationExitCode;
        }
    }
}
