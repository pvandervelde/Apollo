//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Apollo.Core.Base.Projects;
using Apollo.Core.Messaging;
using Apollo.Core.Utils.Licensing;
using Apollo.Utils;
using Autofac;

namespace Apollo.Core.Projects
{
    /// <summary>
    /// Handles the component registrations for the project part 
    /// of the core.
    /// </summary>
    [ExcludeFromCoverage("Modules are used for dependency injection purposes. Testing is done through integration testing.")]
    internal sealed class ProjectModule : Module
    {
        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <param name="builder">The builder through which components can be registered.</param>
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Register(c => new ProjectService(
                    c.Resolve<IDnsNameConstants>(),
                    c.Resolve<IHelpMessageProcessing>(),
                    c.Resolve<IValidationResultStorage>(),
                    c.Resolve<IHelpDistributingDatasets>(),
                    c.Resolve<IBuildProjects>()))
                .As<ProjectService>();

            builder.Register(c => new ProjectBuilder())
                .As<IBuildProjects>();

            builder.Register(c => new MockDatasetDistributor())
                .As<IHelpDistributingDatasets>();
        }
    }
}
