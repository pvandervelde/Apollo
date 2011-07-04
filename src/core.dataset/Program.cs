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
using Apollo.Core.Base;
using Apollo.Core.Base.Communication;
using Apollo.Utilities;
using Apollo.Utilities.Applications;
using Autofac;
using AutofacContrib.Startable;
using Mono.Options;

namespace Apollo.Core.Dataset
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
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

            // To stop the application from running use the ApplicationContext
            // and call context.ExitThread();
            var container = DependencyInjection.Load(context);
            var logger = container.Resolve<Action<LogSeverityProxy, string>>();

            // Load the communication system and get it going
            if (container.IsRegistered<IStarter>())
            {
                container.Resolve<IStarter>().Start();
            }

            // Register all global commands
            try
            {
                var commands = container.Resolve<ICommandCollection>();
                var datasetCommand = container.Resolve<IDatasetApplicationCommands>();
                commands.Register(typeof(IDatasetApplicationCommands), datasetCommand);
            }
            catch (Exception e)
            {
                logger(
                    LogSeverityProxy.Error,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Exception while registering commands. Exception was {0}",
                        e));

                return CommandLineProgram.UnhandledExceptionApplicationExitCode;
            }

            // Notify the host app that we're alive, after which the 
            // rest of the app should pick up the loading of the dataset etc.
            var resolver = container.Resolve<Action<string, string, string>>();
            resolver(hostId, channelType, channelUri);

            // Start with the message processing loop and then we 
            // wait for it to either get terminated or until we kill ourselves.
            Application.Run(context);
            return CommandLineProgram.NormalApplicationExitCode;
        }
    }
}
