//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using Apollo.Core.Base.Communication;
using Apollo.Utilities.Applications;
using Autofac;
using AutofacContrib.Startable;

namespace Test.Manual.Console
{
    /// <summary>
    /// Defines the main entry point for the test application.
    /// </summary>
    /// <remarks>
    /// This application is being used to manually test the communication system of Apollo.
    /// The reason there is a manual test application is that testing the communication layer
    /// (and especially the TCP based communication) requires at least two machines in order
    /// to setup a network connection (it's not possible to have 2 applications on the same
    /// machine sending data to and reading from the same (TCP) port).
    /// </remarks>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1400:AccessModifierMustBeDeclared",
            Justification = "Access modifiers should not be declared on the entry point for a command line application. See FxCop.")]
    class Program
    {
        /// <summary>
        /// The default name for the error log.
        /// </summary>
        private const string s_DefaultErrorFileName = "communicationtest.crash.log";

        /// <summary>
        /// The collection of addresses for the named pipes of applications on the local
        /// machine with which the current application needs to communicate.
        /// </summary>
        private static readonly IList<string> s_NamedPipes = new List<string>();

        /// <summary>
        /// The DI container.
        /// </summary>
        private static IContainer s_Container;

        [STAThread]
        [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1400:AccessModifierMustBeDeclared",
            Justification = "Access modifiers should not be declared on the entry point for a command line application. See FxCop.")]
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
                    RunApplication(args, context);

                    // Prepare the application for running. This includes setting up the communication channel etc.
                    // Then once that is done we can start with the message processing loop and then we 
                    // wait for it to either get terminated or until we kill ourselves.
                    System.Windows.Forms.Application.Run(context);
                    return CommandLineProgram.NormalApplicationExitCode;
                };

            var eventLogSource = Assembly.GetExecutingAssembly().GetName().Name;
            return CommandLineProgram.EntryPoint(applicationLogic, eventLogSource, s_DefaultErrorFileName);
        }

        private static void RunApplication(string[] args, ApplicationContext context)
        {
            var endpoint = string.Empty;
            var address = string.Empty;
            if (args.Length != 0)
            { 
                // The arguments should be:
                // - The endpoint ID of another endpoint on the same machine
                // - The named pipe address of the other endpoint.
                endpoint = args[0];
                address = args[1];
            }

            s_Container = DependencyInjection.CreateContainer(context);

            // Load a communication layer and start listening.
            s_Container.Resolve<IStarter>().Start();
            if (!string.IsNullOrWhiteSpace(endpoint) && !string.IsNullOrWhiteSpace(address))
            {
                var manualResolver = s_Container.Resolve<IAcceptExternalEndpointInformation>();
                manualResolver.RecentlyConnectedEndpoint(
                    new EndpointId(endpoint), 
                    typeof(NamedPipeChannelType), 
                    new Uri(address));
            }

            var window = s_Container.Resolve<IInteractiveWindow>();
            ElementHost.EnableModelessKeyboardInterop(window as Window);
            window.Show();
        }
    }
}
