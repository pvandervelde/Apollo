//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base;
using Apollo.Core.Base.Loaders;
using Apollo.Core.Base.Plugins;
using Apollo.Core.Host.Plugins;
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
                            d => ctx.Resolve<DatasetStorageProxy>(new TypedParameter(typeof(DatasetOnlineInformation), d)),
                            c.Resolve<IHelpDistributingDatasets>(),
                            c.Resolve<IBuildProjects>());
                    })
                .As<ProjectService>();

            builder.Register(c => new ProjectBuilder())
                .As<IBuildProjects>();

            builder.Register(
                (c, p) => 
                {
                    var layer = c.Resolve<IProxyCompositionLayer>(
                        new TypedParameter(
                            typeof(ICompositionCommands),
                            p.TypedAs<DatasetOnlineInformation>().Command<ICompositionCommands>()));
                    return new DatasetStorageProxy(
                        p.TypedAs<DatasetOnlineInformation>(),
                        c.Resolve<GroupSelector>(new TypedParameter(typeof(ICompositionLayer), layer)),
                        layer);
                });

            builder.Register((c, p) => new ProxyCompositionLayer(
                    p.TypedAs<ICompositionCommands>(),
                    c.Resolve<IConnectGroups>()))
                .As<IProxyCompositionLayer>();

            builder.Register((c, p) => new GroupSelector(
                c.Resolve<IConnectGroups>(),
                p.TypedAs<ICompositionLayer>()));

            builder.Register(c => new BidirectionalGraphHistory<DatasetId, Edge<DatasetId>>())
                .As<IBidirectionalGraphHistory<DatasetId, Edge<DatasetId>>>();
        }
    }
}
