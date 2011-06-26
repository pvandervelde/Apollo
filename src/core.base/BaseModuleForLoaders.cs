//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Apollo.Core.Base.Communication;
using Apollo.Core.Base.Loaders;
using Autofac;

namespace Apollo.Core.Base
{
    /// <summary>
    /// Handles the component registrations for the loader components that
    /// are used by applications that start dataset applications.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class BaseModuleForLoaders : Module
    {
        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <param name="builder">The builder through which components can be registered.</param>
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Register(c => new DatasetApplicationLoader(
                    () =>
                    {
                        return (from connection in c.Resolve<ICommunicationLayer>().LocalConnectionPoints()
                                where connection.ChannelType.Equals(typeof(NamedPipeChannelType))
                                select connection.Address).First();
                    },
                    c.Resolve<ISendCommandsToRemoteEndpoints>()))
                .As<IApplicationLoader>();

            builder.Register(c => new DatasetDistributionGenerator(
                    c.Resolve<IEnumerable<IGenerateDistributionProposals>>()))
                .As<IHelpDistributingDatasets>();

            builder.Register(c => new DistributionCalculator())
                .As<ICalculateDistributionParameters>();

            builder.Register(c => new LocalDatasetDistributor(
                    c.Resolve<ICalculateDistributionParameters>(),
                    c.Resolve<IApplicationLoader>(),
                    c.Resolve<ICommunicationLayer>(),
                    c.Resolve<ISendCommandsToRemoteEndpoints>()))
                .As<IGenerateDistributionProposals>()
                .As<ILoadDatasets>()
                .SingleInstance();
        }
    }
}
