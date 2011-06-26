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
using Apollo.Core.Base;
using Apollo.Core.Base.Communication;
using Apollo.Utilities.Applications;
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
            DatasetId dataset = null;

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
                    {
                        "d|dataset",
                        "The {DATASET} that is used to communicate with the parent application.",
                        v => dataset = DatasetIdExtensions.Deserialize(v)
                    }
                };

            options.Parse(arguments);
            if ((hostId == null) || (channelType == null) || (channelUri == null) || (dataset == null))
            {
                throw new InvalidCommandLineArgumentsException();
            }

            // To stop the application from running use the ApplicationContext
            // and call context.ExitThread();
            var container = DependencyInjection.Load(context);
            
            // Prepare the application for running. This includes setting up the communication channel etc.
            // - Create the entry point class

            // Start with the message processing loop and then we 
            // wait for it to either get terminated or until we kill ourselves.
            Application.Run(context);
            return CommandLineProgram.NormalApplicationExitCode;
        }
    }
}
