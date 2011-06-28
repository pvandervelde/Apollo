//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1400:AccessModifierMustBeDeclared",
            Justification = "Access modifiers should not be declared on the entry point for a command line application. See FxCop.")]
    static class Program
    {
        /// <summary>
        /// The default name for the error log.
        /// </summary>
        private const string DefaultErrorFileName = "dataset.error.log";

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
            return CommandLineProgram.EntryPoint(applicationLogic, eventLogSource, DefaultErrorFileName);
        }

        private static int RunApplication(string[] arguments, ApplicationContext context)
        {
            EndpointId hostId = null;
            Type channelType = null;
            Uri channelUri = null;

            // Parse the command line options
            var options = new OptionSet 
                {
                    { 
                        "h|host", 
                        "The {ENDPOINTID} of the host application that requested the start of this application.", 
                        v => hostId = EndpointIdExtensions.Deserialize(v)
                    },
                    {
                        "t|channeltype",
                        "The {TYPE} of the channel over which the connection should be made.",
                        v => channelType = Type.GetType(v, null, null, true, false)
                    },
                    {
                        "u|channeluri",
                        "The {URI} of the connection that can be used to connect to the host application.",
                        v => channelUri = new Uri(v)
                    },
                };

            options.Parse(arguments);
            if ((hostId == null) || (channelType == null) || (channelUri == null))
            {
                throw new InvalidCommandLineArgumentsException();
            }

            // To stop the application from running use the ApplicationContext
            // and call context.ExitThread();
            var container = DependencyInjection.Load(context);

            // Notify the host app that we're alive, after which the 
            // rest of the app should pick up the loading of the dataset etc.
            var resolver = container.Resolve<IAcceptExternalEndpointInformation>();
            resolver.RecentlyConnectedEndpoint(hostId, channelType, channelUri);
            
            // Start with the message processing loop and then we 
            // wait for it to either get terminated or until we kill ourselves.
            Application.Run(context);
            return CommandLineProgram.NormalApplicationExitCode;
        }
    }
}
