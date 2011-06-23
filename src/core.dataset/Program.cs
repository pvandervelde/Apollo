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
            EndpointId creatorId = null;
            Uri channel = null;
            ConversationToken token = null;

            // Parse the command line options
            var options = new OptionSet 
                {
                    { 
                        "p|parent", 
                        "The {ENDPOINTID} of the application that started this application.", 
                        v => creatorId = EndpointIdExtensions.Deserialize(v)
                    },
                    {
                        "c|channel",
                        "The {URI} of the named pipe connection that can be used to connect to the parent application.",
                        v => channel = new Uri(v)
                    },
                    {
                        "t|token",
                        "The {TOKEN} that is used to communicate with the parent application.",
                        v => token = ConversationTokenExtensions.Deserialize(v)
                    }
                };

            options.Parse(arguments);
            if ((creatorId == null) || (channel == null) || (token == null))
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
