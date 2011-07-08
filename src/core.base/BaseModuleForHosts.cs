﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Apollo.Core.Base.Communication;
using Apollo.Core.Base.Loaders;
using Apollo.Utilities;
using Apollo.Utilities.Configuration;
using Autofac;

namespace Apollo.Core.Base
{
    /// <summary>
    /// Handles the component registration for the loader components for 
    /// applications that initiate the loading of datasets in local or
    /// remote applications.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class BaseModuleForHosts : Module
    {
        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <param name="builder">The builder through which components can be registered.</param>
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Register(c => new RemoteDatasetDistributor(
                    c.Resolve<ISendCommandsToRemoteEndpoints>(),
                    c.Resolve<IConfiguration>(),
                    c.Resolve<WaitingUploads>(),
                    (dataset, endpoint, network) =>
                    {
                        return new DatasetOnlineInformation(
                            dataset,
                            endpoint,
                            network,
                            c.Resolve<ISendCommandsToRemoteEndpoints>(),
                            c.Resolve<Action<LogSeverityProxy, string>>());
                    },
                    () =>
                    {
                        return (from connection in c.Resolve<ICommunicationLayer>().LocalConnectionPoints()
                                where connection.ChannelType.Equals(typeof(TcpChannelType))
                                select connection).First();
                    }))
                .As<IGenerateDistributionProposals>()
                .As<ILoadDatasets>()
                .SingleInstance();

            builder.Register(c => new LocalDatasetDistributor(
                    c.Resolve<ICalculateDistributionParameters>(),
                    c.Resolve<IApplicationLoader>(),
                    c.Resolve<ISendCommandsToRemoteEndpoints>(),
                    c.Resolve<WaitingUploads>(),
                    (dataset, endpoint, network) =>
                    {
                        return new DatasetOnlineInformation(
                            dataset,
                            endpoint,
                            network,
                            c.Resolve<ISendCommandsToRemoteEndpoints>(),
                            c.Resolve<Action<LogSeverityProxy, string>>());
                    },
                    () =>
                    {
                        return (from connection in c.Resolve<ICommunicationLayer>().LocalConnectionPoints()
                                where connection.ChannelType.Equals(typeof(NamedPipeChannelType))
                                select connection).First();
                    }))
                .As<IGenerateDistributionProposals>()
                .As<ILoadDatasets>()
                .SingleInstance();

            // HostCommands?
        }
    }
}
