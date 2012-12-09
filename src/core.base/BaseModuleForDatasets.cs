//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base.Communication;
using Apollo.Core.Base.Communication.Messages.Processors;
using Apollo.Utilities;
using Autofac;

namespace Apollo.Core.Base
{
    /// <summary>
    /// Handles the component registration for the dataset components for 
    /// applications that load and manipulate datasets.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class BaseModuleForDatasets : Module
    {
        private static void RegisterCommandCollection(ContainerBuilder builder)
        {
            builder.Register(c => new LocalCommandCollection(
                    c.Resolve<ICommunicationLayer>()))
                .As<ICommandCollection>()
                .SingleInstance();
        }

        private static void RegisterNotificationCollection(ContainerBuilder builder)
        {
            builder.Register(c => new LocalNotificationCollection(
                    c.Resolve<ICommunicationLayer>()))
                .As<INotificationSendersCollection>()
                .As<ISendNotifications>()
                .SingleInstance();
        }

        private static void RegisterMessageProcessingActions(ContainerBuilder builder)
        {
            builder.Register(
                   c =>
                   {
                       // Autofac 2.4.5 forces the 'c' variable to disappear. See here:
                       // http://stackoverflow.com/questions/5383888/autofac-registration-issue-in-release-v2-4-5-724
                       var ctx = c.Resolve<IComponentContext>();
                       return new CommandInformationRequestProcessAction(
                           EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                           (endpoint, msg) => ctx.Resolve<ICommunicationLayer>().SendMessageTo(endpoint, msg),
                           c.Resolve<ICommandCollection>(),
                           c.Resolve<SystemDiagnostics>());
                   })
               .As<IMessageProcessAction>();

            builder.Register(
                    c =>
                    {
                        // Autofac 2.4.5 forces the 'c' variable to disappear. See here:
                        // http://stackoverflow.com/questions/5383888/autofac-registration-issue-in-release-v2-4-5-724
                        var ctx = c.Resolve<IComponentContext>();
                        return new CommandInvokedProcessAction(
                            EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                            (endpoint, msg) => ctx.Resolve<ICommunicationLayer>().SendMessageTo(endpoint, msg),
                            c.Resolve<ICommandCollection>(),
                            c.Resolve<SystemDiagnostics>());
                    })
                .As<IMessageProcessAction>();

            builder.Register(
                    c =>
                    {
                        // Autofac 2.4.5 forces the 'c' variable to disappear. See here:
                        // http://stackoverflow.com/questions/5383888/autofac-registration-issue-in-release-v2-4-5-724
                        var ctx = c.Resolve<IComponentContext>();
                        return new NotificationInformationRequestProcessAction(
                            EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                            (endpoint, msg) => ctx.Resolve<ICommunicationLayer>().SendMessageTo(endpoint, msg),
                            c.Resolve<INotificationSendersCollection>(),
                            c.Resolve<SystemDiagnostics>());
                    })
                .As<IMessageProcessAction>();

            builder.Register(c => new RegisterForNotificationProcessAction(
                    c.Resolve<ISendNotifications>()))
                .As<IMessageProcessAction>();

            builder.Register(c => new UnregisterFromNotificationProcessAction(
                    c.Resolve<ISendNotifications>()))
                .As<IMessageProcessAction>();
        }

        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <param name="builder">The builder through which components can be registered.</param>
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            RegisterCommandCollection(builder);
            RegisterNotificationCollection(builder);
            RegisterMessageProcessingActions(builder);
        }
    }
}
