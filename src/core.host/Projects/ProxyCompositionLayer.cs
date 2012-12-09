//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Apollo.Core.Base.Plugins;
using Apollo.Core.Host.Plugins;
using Apollo.Utilities;
using QuickGraph;

namespace Apollo.Core.Host.Projects
{
    /// <summary>
    /// Defines a proxy for the group composition graph in a dataset. Also caches the current state of the graph for faster
    /// access.
    /// </summary>
    internal sealed class ProxyCompositionLayer : IProxyCompositionLayer
    {
        /// <summary>
        /// The object used to lock on.
        /// </summary>
        private readonly ILockObject m_Lock = new LockObject();

        /// <summary>
        /// The collection that contains all the currently loaded part groups.
        /// </summary>
        private readonly IDictionary<GroupCompositionId, GroupDefinition> m_Groups
            = new Dictionary<GroupCompositionId, GroupDefinition>();

        /// <summary>
        /// The graph that describes the connections between the linked part groups.
        /// </summary>
        /// <design>
        /// Note that the edges point from the export to the import.
        /// </design>
        private readonly BidirectionalGraph<GroupCompositionId, CompositionLayerGroupEdge> m_GroupConnections
            = new BidirectionalGraph<GroupCompositionId, CompositionLayerGroupEdge>();

        /// <summary>
        /// The object that provides the commands for the composition of part groups.
        /// </summary>
        private readonly ICompositionCommands m_Commands;

        /// <summary>
        /// The object that handles the connection of part groups.
        /// </summary>
        private readonly IConnectGroups m_Connector;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyCompositionLayer"/> class.
        /// </summary>
        /// <param name="commands">The object that provides the commands for the composition of part groups.</param>
        /// <param name="groupConnector">The object that handles the connection of part groups.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="commands"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="groupConnector"/> is <see langword="null" />.
        /// </exception>
        public ProxyCompositionLayer(
            ICompositionCommands commands,
            IConnectGroups groupConnector)
        {
            {
                Lokad.Enforce.Argument(() => commands);
                Lokad.Enforce.Argument(() => groupConnector);
            }

            m_Commands = commands;
            m_Connector = groupConnector;
        }

        /// <summary>
        /// Adds a new <see cref="GroupDefinition"/> to the graph and returns the ID for that group.
        /// </summary>
        /// <param name="group">The group that should be added to the graph.</param>
        /// <returns>
        /// A task which returns the ID for the group.
        /// </returns>
        public Task<GroupCompositionId> Add(GroupDefinition group)
        {
            {
                Debug.Assert(group != null, "The definition that should be added should not be a null reference.");
            }

            var id = new GroupCompositionId();
            var remoteTask = m_Commands.Add(id, group);

            return remoteTask.ContinueWith(
                t =>
                {
                    lock (m_Lock)
                    {
                        m_Groups.Add(id, group);
                        m_GroupConnections.AddVertex(id);
                    }

                    return id;
                });
        }

        /// <summary>
        /// Removes the group that is related to the specified ID.
        /// </summary>
        /// <param name="group">The ID of the group that should be removed.</param>
        /// <returns>A task that indicates when the removal has taken place.</returns>
        public Task Remove(GroupCompositionId group)
        {
            {
                Debug.Assert(group != null, "The ID that should be removed should not be a null reference.");
                Debug.Assert(m_Groups.ContainsKey(group), "The ID should be known.");
            }

            var remoteTask = m_Commands.Remove(group);
            return remoteTask.ContinueWith(
                t =>
                {
                    lock (m_Lock)
                    {
                        if (m_GroupConnections.ContainsVertex(group))
                        {
                            m_GroupConnections.RemoveVertex(group);
                        }

                        if (m_Groups.ContainsKey(group))
                        {
                            m_Groups.Remove(group);
                        }
                    }
                });
        }

        /// <summary>
        /// Returns a value indicating if the graph contains a group for the given ID.
        /// </summary>
        /// <param name="id">The ID for which the graph is searched.</param>
        /// <returns>
        ///     <see langword="true" /> if the graph contains a group with the given ID; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Contains(GroupCompositionId id)
        {
            lock (m_Lock)
            {
                return (id != null) && m_Groups.ContainsKey(id);
            }
        }

        /// <summary>
        /// Returns the <see cref="GroupDefinition"/> which is related to the given ID.
        /// </summary>
        /// <param name="id">The ID of the requested group.</param>
        /// <returns>The requested group.</returns>
        public GroupDefinition Group(GroupCompositionId id)
        {
            {
                Debug.Assert(id != null, "The ID that should be removed should not be a null reference.");
            }

            lock (m_Lock)
            {
                if (!m_Groups.ContainsKey(id))
                {
                    throw new UnknownPartGroupException();
                }

                return m_Groups[id];
            }
        }

        /// <summary>
        /// Returns the collection of all known groups.
        /// </summary>
        /// <returns>The collection of all known groups.</returns>
        public IEnumerable<GroupCompositionId> Groups()
        {
            lock (m_Lock)
            {
                return m_Groups.Keys.ToList();
            }
        }

        /// <summary>
        /// Connects the given export to the given import.
        /// </summary>
        /// <param name="importingGroup">The ID of the group that owns the import.</param>
        /// <param name="importDefinition">The import.</param>
        /// <param name="exportingGroup">The ID of the group that owns the export.</param>
        /// <returns>A task which indicates when the connection has taken place.</returns>
        public Task Connect(
            GroupCompositionId importingGroup,
            GroupImportDefinition importDefinition,
            GroupCompositionId exportingGroup)
        {
            {
                Debug.Assert(importingGroup != null, "The ID of the importing group should not be a null reference.");
                Debug.Assert(importDefinition != null, "The import definition should not be a null reference.");
                Debug.Assert(exportingGroup != null, "The ID of the exporting group should not be a null reference.");
            }

            if (!Contains(importingGroup) || !Contains(exportingGroup))
            {
                throw new UnknownPartGroupException();
            }

            var parts = m_Connector.GenerateConnectionFor(m_Groups[importingGroup], importDefinition, m_Groups[exportingGroup]);
            var state = new GroupConnection(importingGroup, exportingGroup, importDefinition, parts);
            var remoteTask = m_Commands.Connect(state);
            return remoteTask.ContinueWith(
                t =>
                {
                    lock (m_Lock)
                    {
                        m_GroupConnections.AddEdge(new CompositionLayerGroupEdge(importingGroup, importDefinition, exportingGroup));
                    }
                });
        }

        /// <summary>
        /// Disconnects the two groups.
        /// </summary>
        /// <remarks>
        /// This method assumes that two groups will only be connected via one import - export relation. This
        /// method will remove all connections from the exporting group to the importing group.
        /// </remarks>
        /// <param name="importingGroup">The ID of the group that owns the import.</param>
        /// <param name="exportingGroup">The ID of the group that owns the export.</param>
        /// <returns>A task which indicates when the disconnection has taken place.</returns>
        public Task Disconnect(GroupCompositionId importingGroup, GroupCompositionId exportingGroup)
        {
            {
                Debug.Assert(importingGroup != null, "The ID of the importing group should not be a null reference.");
                Debug.Assert(exportingGroup != null, "The ID of the exporting group should not be a null reference.");
            }

            var remoteTask = m_Commands.Disconnect(importingGroup, exportingGroup);
            return remoteTask.ContinueWith(
                t =>
                {
                    m_GroupConnections.RemoveInEdgeIf(importingGroup, edge => edge.Source.Equals(exportingGroup));
                });
        }

        /// <summary>
        /// Disconnects all connection to and from the given group.
        /// </summary>
        /// <param name="group">The ID of the group.</param>
        /// <returns>A task which indicates when the disconnection has taken place.</returns>
        public Task Disconnect(GroupCompositionId group)
        {
            {
                Debug.Assert(group != null, "The ID of the group should not be a null reference.");
            }

            var remoteTask = m_Commands.Disconnect(group);
            return remoteTask.ContinueWith(
                t =>
                {
                    m_GroupConnections.RemoveInEdgeIf(group, edge => true);
                    m_GroupConnections.RemoveOutEdgeIf(group, edge => true);
                });
        }

        /// <summary>
        /// Returns a value indicating if the given import is connected to anything.
        /// </summary>
        /// <param name="importingGroup">The ID of the group owning the import.</param>
        /// <param name="importDefinition">The import.</param>
        /// <returns>
        /// <see langword="true" /> if the import is connected to an export; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool IsConnected(GroupCompositionId importingGroup, GroupImportDefinition importDefinition)
        {
            if ((importingGroup == null) || (importDefinition == null))
            {
                return false;
            }

            if (!Contains(importingGroup))
            {
                return false;
            }

            lock (m_Lock)
            {
                return m_GroupConnections.InEdges(importingGroup).Where(e => e.Import.Equals(importDefinition)).Any();
            }
        }

        /// <summary>
        /// Returns a value indicating if the given import is connected to the given export.
        /// </summary>
        /// <param name="importingGroup">The ID of the group owning the import.</param>
        /// <param name="importDefinition">The import.</param>
        /// <param name="exportingGroup">The ID of the group owning the export.</param>
        /// <returns>
        /// <see langword="true" /> if the import is connected to an export; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool IsConnected(
            GroupCompositionId importingGroup,
            GroupImportDefinition importDefinition,
            GroupCompositionId exportingGroup)
        {
            if ((importingGroup == null) || (importDefinition == null) || (exportingGroup == null))
            {
                return false;
            }

            if (!Contains(importingGroup))
            {
                return false;
            }

            lock (m_Lock)
            {
                return m_GroupConnections.InEdges(importingGroup)
                    .Where(e => e.Import.Equals(importDefinition) && e.Source.Equals(exportingGroup))
                    .Any();
            }
        }

        /// <summary>
        /// Returns the group information indicating which export the given import is connected to.
        /// </summary>
        /// <param name="importingGroup">The ID of the group owning the import.</param>
        /// <param name="importDefinition">The import.</param>
        /// <returns>The ID of the group the given import is connected to, if there is a connection; otherwise, <see langword="null" />.</returns>
        public GroupCompositionId ConnectedTo(
            GroupCompositionId importingGroup,
            GroupImportDefinition importDefinition)
        {
            if ((importingGroup == null) || (importDefinition == null))
            {
                return null;
            }

            if (!Contains(importingGroup))
            {
                return null;
            }

            lock (m_Lock)
            {
                return m_GroupConnections.InEdges(importingGroup)
                    .Where(e => e.Import.Equals(importDefinition))
                    .Select(e => e.Source)
                    .FirstOrDefault();
            }
        }

        /// <summary>
        /// Reloads the proxy data from the dataset.
        /// </summary>
        /// <returns>A task that will finish when the reload is complete.</returns>
        public Task ReloadFromDataset()
        {
            var task = m_Commands.CurrentState();
            return task.ContinueWith(
                t => 
                {
                    lock (m_Lock)
                    {
                        m_Groups.Clear();
                        m_GroupConnections.Clear();

                        var state = t.Result;
                        foreach (var group in state.Groups)
                        {
                            m_Groups.Add(group.Item1, group.Item2);
                            m_GroupConnections.AddVertex(group.Item1);
                        }

                        foreach (var connection in state.Connections)
                        {
                            m_GroupConnections.AddEdge(new CompositionLayerGroupEdge(connection.Item1, connection.Item2, connection.Item3));
                        }
                    }
                });
        }
    }
}
