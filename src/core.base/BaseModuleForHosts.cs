//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base.Communication;
using Apollo.Core.Base.Loaders;
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
                    c.Resolve<ICommunicationLayer>(),
                    c.Resolve<ISendCommandsToRemoteEndpoints>(),
                    c.Resolve<IConfiguration>()))
                .As<IGenerateDistributionProposals>()
                .As<ILoadDatasets>()
                .SingleInstance();

            // HostCommands?
        }
    }
}
