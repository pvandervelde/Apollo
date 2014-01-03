//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Apollo.Utilities;
using Autofac;
using Nuclei.Communication;
using Nuclei.Configuration;
using Nuclei.Diagnostics;

namespace Apollo.Core.Base.Activation
{
    /// <summary>
    /// Handles the component registrations for the loader components that
    /// are used by applications that start dataset applications.
    /// </summary>
    public sealed class ActivatorModule : Module
    {
        private static void RegisterDistributors(ContainerBuilder builder)
        {
            builder.Register(
                    c =>
                    {
                        var ctx = c.Resolve<IComponentContext>();
                        Func<DatasetId, EndpointId, NetworkIdentifier, DatasetOnlineInformation> func =
                            (dataset, endpoint, networkId) => new DatasetOnlineInformation(
                                dataset,
                                endpoint,
                                networkId,
                                ctx.Resolve<ISendCommandsToRemoteEndpoints>(),
                                ctx.Resolve<INotifyOfRemoteEndpointEvents>(),
                                ctx.Resolve<SystemDiagnostics>());

                        return new LocalDatasetDistributor(
                            c.Resolve<ICalculateDistributionParameters>(),
                            c.Resolve<IDatasetActivator>(),
                            c.Resolve<ISendCommandsToRemoteEndpoints>(),
                            c.Resolve<INotifyOfRemoteEndpointEvents>(),
                            c.Resolve<IStoreUploads>(),
                            func,
                            c.Resolve<ICommunicationLayer>(),
                            c.Resolve<SystemDiagnostics>());
                    })
                .As<IGenerateDistributionProposals>();

            builder.Register(
                    c =>
                    {
                        var ctx = c.Resolve<IComponentContext>();
                        Func<DatasetId, EndpointId, NetworkIdentifier, DatasetOnlineInformation> func =
                            (dataset, endpoint, networkId) => new DatasetOnlineInformation(
                                dataset,
                                endpoint,
                                networkId,
                                ctx.Resolve<ISendCommandsToRemoteEndpoints>(),
                                ctx.Resolve<INotifyOfRemoteEndpointEvents>(),
                                ctx.Resolve<SystemDiagnostics>());

                        return new RemoteDatasetDistributor(
                            c.Resolve<IConfiguration>(),
                            c.Resolve<ISendCommandsToRemoteEndpoints>(),
                            c.Resolve<INotifyOfRemoteEndpointEvents>(),
                            c.Resolve<IStoreUploads>(),
                            func, 
                            c.Resolve<ICommunicationLayer>(), 
                            c.Resolve<SystemDiagnostics>());
                    })
                .As<IGenerateDistributionProposals>();
        }

        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <param name="builder">The builder through which components can be registered.</param>
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            RegisterDistributors(builder);

            builder.Register(c => new DatasetActivator(
                    c.Resolve<ApplicationConstants>(),
                    c.Resolve<SystemDiagnostics>()))
                .As<IDatasetActivator>();

            builder.Register(c => new DatasetDistributionGenerator(
                    c.Resolve<IEnumerable<IGenerateDistributionProposals>>()))
                .As<IHelpDistributingDatasets>();

            builder.Register(c => new DistributionCalculator())
                .As<ICalculateDistributionParameters>();
        }
    }
}
