//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base.Loaders;
using Apollo.Core.Utils.Licensing;
using Autofac;

namespace Apollo.Core.Projects
{
    /// <summary>
    /// Handles the component registrations for the project part 
    /// of the core.
    /// </summary>
    [ExcludeFromCodeCoverage()]
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
