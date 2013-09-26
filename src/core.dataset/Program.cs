//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows.Forms;
using Apollo.Core.Dataset.Nuclei.ExceptionHandling;
using Apollo.Utilities;
using Autofac;
using Mono.Options;
using Nuclei.Communication;
using Nuclei.Configuration;
using Nuclei.Diagnostics.Logging;

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
        /// Defines the error code for a normal application exit (i.e without errors).
        /// </summary>
        private const int NormalApplicationExitCode = 0;

        /// <summary>
        /// Defines the error code for an application exit with an unhandled exception.
        /// </summary>
        private const int UnhandledExceptionApplicationExitCode = 1;

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

            int functionReturnResult = -1;

            var processor = new LogBasedExceptionProcessor(
                LoggerBuilder.ForFile(
                    Path.Combine(new FileConstants(new ApplicationConstants()).LogPath(), DefaultErrorFileName),
                    new DebugLogTemplate(new NullConfiguration(), () => DateTimeOffset.Now)));
            var result = TopLevelExceptionGuard.RunGuarded(
                () => functionReturnResult = RunApplication(args, new ApplicationContext()),
                new ExceptionProcessor[]
                    {
                        processor.Process,
                    });

            return (result == GuardResult.Failure) ? UnhandledExceptionApplicationExitCode : functionReturnResult;
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "We're catching the exception and then exiting the application.")]
        private static int RunApplication(string[] arguments, ApplicationContext context)
        {
            string hostIdText = null;
            string channelTypeText = null;
            string channelUriText = null;

            // Parse the command line options
            var options = new OptionSet 
                {
                    { 
                        "h=|host=", 
                        "The {ENDPOINTID} of the host application that requested the start of this application.", 
                        v => hostIdText = v
                    },
                    {
                        "t=|channeltype=",
                        "The {TYPE} of the channel over which the connection should be made.",
                        v => channelTypeText = v
                    },
                    {
                        "u=|channeluri=",
                        "The {URI} of the connection that can be used to connect to the host application.",
                        v => channelUriText = v
                    },
                };

            options.Parse(arguments);
            if (string.IsNullOrWhiteSpace(hostIdText) ||
                string.IsNullOrWhiteSpace(channelTypeText) ||
                string.IsNullOrWhiteSpace(channelUriText))
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

            var hostId = EndpointIdExtensions.Deserialize(hostIdText);
            var channelType = (ChannelType)Enum.Parse(typeof(ChannelType), channelTypeText);

            // Notify the host app that we're alive, after which the 
            // rest of the app should pick up the loading of the dataset etc.
            // Note that this call to the container also registers all global commands and initializes the
            // communication channels etc.
            var resolver = container.Resolve<ManualEndpointConnection>();
            resolver(hostId, channelType, channelUriText);

            // Connect to the assembly resolve event so that we can intercept assembly loads for plugins etc.
            var assemblyResolver = container.Resolve<PluginLoadingAssemblyResolver>(
                new TypedParameter(
                    typeof(EndpointId), 
                    EndpointIdExtensions.Deserialize(hostIdText)));
            AppDomain.CurrentDomain.AssemblyResolve += assemblyResolver.LocatePluginAssembly;

            // Start with the message processing loop and then we 
            // wait for it to either get terminated or until we kill ourselves.
            Application.Run(context);
            return NormalApplicationExitCode;
        }
    }
}
