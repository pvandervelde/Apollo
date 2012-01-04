//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base;
using Apollo.Core.Base.Loaders;
using Apollo.Utilities.History;
using Autofac;
using QuickGraph;

namespace Apollo.Core.Host.Projects
{
    /// <summary>
    /// Handles the component registrations for the project part 
    /// of the core.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal sealed class ProjectModule : Module
    {
        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <param name="builder">The builder through which components can be registered.</param>
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Register(
                    c => 
                    {
                        // Autofac 2.4.5 forces the 'c' variable to disappear. See here:
                        // http://stackoverflow.com/questions/5383888/autofac-registration-issue-in-release-v2-4-5-724
                        var ctx = c.Resolve<IComponentContext>();
                        return new ProjectService(
                            () => ctx.Resolve<ITimeline>(),
                            c.Resolve<IHelpDistributingDatasets>(),
                            c.Resolve<IBuildProjects>());
                    })
                .As<ProjectService>();

            builder.Register(c => new ProjectBuilder())
                .As<IBuildProjects>();

            builder.Register(c => new DictionaryHistory<DatasetId, DatasetOfflineInformation>())
                .As<IDictionaryTimelineStorage<DatasetId, DatasetOfflineInformation>>();

            builder.Register(c => new DictionaryHistory<DatasetId, DatasetOnlineInformation>())
                .As<IDictionaryTimelineStorage<DatasetId, DatasetOnlineInformation>>();

            builder.Register(c => new BidirectionalGraphHistory<DatasetId, Edge<DatasetId>>())
                .As<IBidirectionalGraphHistory<DatasetId, Edge<DatasetId>>>();
        }
    }
}
