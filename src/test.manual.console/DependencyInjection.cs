//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Windows.Forms;
using Apollo.Core.Base;
using Apollo.Core.Base.Communication;
using Apollo.Utilities;
using Apollo.Utilities.Configuration;
using Autofac;
using AutofacContrib.Startable;
using Test.Manual.Console.Models;
using Test.Manual.Console.Views;

namespace Test.Manual.Console
{
    /// <summary>
    /// Provides methods for use with the Dependency Injection of the different
    /// objects.
    /// </summary>
    internal static class DependencyInjection
    {
        /// <summary>
        /// Creates the DI container.
        /// </summary>
        /// <param name="context">The application context that can be used to terminate the application.</param>
        /// <returns>A new DI container.</returns>
        public static IContainer CreateContainer(ApplicationContext context)
        {
            var builder = new ContainerBuilder();
            {
                builder.RegisterModule(new BaseModule());
                builder.RegisterModule(new UtilitiesModule());

                builder.RegisterModule(
                  new StartableModule<ILoadOnApplicationStartup>(s => s.Initialize()));

                // Register the elements from the current assembly
                builder.Register(c => new InteractiveWindow(
                        context,
                        c.Resolve<IHandleCommunication>()))
                    .OnActivated(a => 
                        {
                            a.Instance.ConnectionState = a.Context.Resolve<ConnectionViewModel>();
                        })
                    .As<InteractiveWindow>()
                    .As<IInteractiveWindow>()
                    .SingleInstance();

                builder.Register(c => new FakeConfiguration())
                    .As<IConfiguration>()
                    .SingleInstance();

                builder.Register(c => new ApplicationCentral(
                        c.Resolve<ICommunicationLayer>(),
                        c.Resolve<ConnectionViewModel>()))
                    .As<IFormTheApplicationCenter>()
                    .As<ILoadOnApplicationStartup>()
                    .SingleInstance();

                builder.Register(c => new ConnectionViewModel(c.Resolve<InteractiveWindow>().Dispatcher))
                    .SingleInstance();

                builder.Register(c => new CommunicationPassThrough(
                        c.Resolve<ICommunicationLayer>()))
                    .As<IHandleCommunication>();

                builder.Register(c => new EchoMessageProcessingAction(
                        c.Resolve<ConnectionViewModel>()))
                    .As<IMessageProcessAction>();

                builder.Register(c => new AllMessageProcessingAction(
                        c.Resolve<ConnectionViewModel>()))
                    .As<IMessageProcessAction>();
            }

            return builder.Build();
        }
    }
}
