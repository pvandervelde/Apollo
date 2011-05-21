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
using Apollo.Utilities.Applications;

namespace Apollo.Core.LoaderApplication
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
        private const string DefaultErrorFileName = "loaderapplication.error.log";

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
                    var context = new ApplicationContext()
                    {
                        Tag = args
                    };

                    // To stop the application from running use the ApplicationContext
                    // and call context.ExitThread();
                    //
                    // Prepare the application for running. This includes setting up the communication channel etc.
                    // Then once that is done we can start with the message processing loop and then we 
                    // wait for it to either get terminated or until we kill ourselves.
                    Application.Run(context);
                    return CommandLineProgram.NormalApplicationExitCode;
                };

            var eventLogSource = Assembly.GetExecutingAssembly().GetName().Name;
            return CommandLineProgram.EntryPoint(applicationLogic, eventLogSource, DefaultErrorFileName);
        }
    }
}
