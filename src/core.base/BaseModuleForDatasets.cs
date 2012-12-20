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
        private static void RegisterNotificationCollection(ContainerBuilder builder)
        {
            builder.Register(c => new LocalNotificationCollection(
                    c.Resolve<ICommunicationLayer>()))
                .As<INotificationSendersCollection>()
                .As<ISendNotifications>()
                .SingleInstance();
        }

        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <param name="builder">The builder through which components can be registered.</param>
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            RegisterNotificationCollection(builder);
        }
    }
}
