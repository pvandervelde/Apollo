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
using System.Text;
using System.Threading.Tasks;
using Apollo.Core.Base.Plugins;
using Apollo.Core.Base.Scheduling;
using Apollo.Core.Dataset.Properties;
using Apollo.Core.Extensions.Plugins;
using Apollo.Utilities.History;

namespace Apollo.Core.Dataset.Plugins
{
    /// <summary>
    /// Defines the graph of components groups that describes how the different
    /// groups are connected.
    /// </summary>
    internal class CompositionLayer : IStoreGroupsAndConnections, IAmHistoryEnabled
    {
        /// <summary>
        /// The history index of the definitions field.
        /// </summary>
        private const byte GroupDefinitionIndex = 0;

        /// <summary>
        /// The history index of the groups field.
        /// </summary>
        private const byte GroupCompositionIndex = 1;

        /// <summary>
        /// The history index of the group connections field.
        /// </summary>
        private const byte GroupConnectionIndex = 2;

        /// <summary>
        /// The history index of the part definition field.
        /// </summary>
        private const byte PartCompositionIndex = 3;

        /// <summary>
        /// The history index of the part connections field.
        /// </summary>
        private const byte PartConnectionIndex = 4;

        /// <summary>
        /// Creates a new instance of the <see cref="CompositionLayer"/> class with the given 
        /// history information.
        /// </summary>
        /// <param name="id">The history ID for the dataset storage.</param>
        /// <param name="members">The collection that holds all the members for the current object.</param>
        /// <param name="constructorArguments">The optional constructor arguments.</param>
        /// <returns>A new instance of the <see cref="CompositionLayer"/> class.</returns>
        internal static CompositionLayer Build(
            HistoryId id,
            IEnumerable<Tuple<byte, IStoreTimelineValues>> members,
            params object[] constructorArguments)
        {
            {
                Debug.Assert(members.Count() == 3, "There should be 3 members.");
            }

            IDictionaryTimelineStorage<GroupRegistrationId, GroupDefinition> definitions = null;
            IDictionaryTimelineStorage<GroupCompositionId, GroupRegistrationId> groups = null;
            IBidirectionalGraphHistory<GroupCompositionId, GroupCompositionGraphEdge> graph = null;
            IDictionaryTimelineStorage<PartCompositionId, PartCompositionInfo> parts = null;
            IBidirectionalGraphHistory<PartCompositionId, PartCompositionGraphEdge> partConnections = null;
            foreach (var member in members)
            {
                if (member.Item1 == GroupDefinitionIndex)
                {
                    definitions = member.Item2 as IDictionaryTimelineStorage<GroupRegistrationId, GroupDefinition>;
                    continue;
                }

                if (member.Item1 == GroupCompositionIndex)
                {
                    groups = member.Item2 as IDictionaryTimelineStorage<GroupCompositionId, GroupRegistrationId>;
                    continue;
                }

                if (member.Item1 == GroupConnectionIndex)
                {
                    graph = member.Item2 as IBidirectionalGraphHistory<GroupCompositionId, GroupCompositionGraphEdge>;
                    continue;
                }

                if (member.Item1 == PartCompositionIndex)
                {
                    parts = member.Item2 as IDictionaryTimelineStorage<PartCompositionId, PartCompositionInfo>;
                    continue;
                }

                if (member.Item1 == PartConnectionIndex)
                {
                    partConnections = member.Item2 as IBidirectionalGraphHistory<PartCompositionId, PartCompositionGraphEdge>;
                    continue;
                }

                throw new UnknownMemberException();
            }

            return new CompositionLayer(id, definitions, groups, graph, parts, partConnections);
        }

        /// <summary>
        /// The ID used by the timeline to uniquely identify the current object.
        /// </summary>
        private readonly HistoryId m_HistoryId;

        /// <summary>
        /// The collection that contains all the currently selected definitions.
        /// </summary>
        [FieldIndexForHistoryTracking(GroupDefinitionIndex)]
        private readonly IDictionaryTimelineStorage<GroupRegistrationId, GroupDefinition> m_Definitions;

        /// <summary>
        /// The collection that contains all the currently selected groups.
        /// </summary>
        [FieldIndexForHistoryTracking(GroupCompositionIndex)]
        private readonly IDictionaryTimelineStorage<GroupCompositionId, GroupRegistrationId> m_Groups;

        /// <summary>
        /// The graph that determines how the different groups are connected.
        /// </summary>
        /// <design>
        /// Note that the edges point from the export to the import.
        /// </design>
        [FieldIndexForHistoryTracking(GroupConnectionIndex)]
        private readonly IBidirectionalGraphHistory<GroupCompositionId, GroupCompositionGraphEdge> m_GroupConnections;

        /// <summary>
        /// The collection that contains all the parts for the currently selected groups.
        /// </summary>
        [FieldIndexForHistoryTracking(PartCompositionIndex)]
        private readonly IDictionaryTimelineStorage<PartCompositionId, PartCompositionInfo> m_Parts;

        /// <summary>
        /// The graph that determines how the different parts are connected.
        /// </summary>
        /// <design>
        /// Note that the edges point from the export to the import.
        /// </design>
        [FieldIndexForHistoryTracking(PartConnectionIndex)]
        private readonly IBidirectionalGraphHistory<PartCompositionId, PartCompositionGraphEdge> m_PartConnections;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionLayer"/> class.
        /// </summary>
        /// <param name="id">The ID used by the timeline to uniquely identify the current object.</param>
        /// <param name="definitions">The collection containing all the known group definitions.</param>
        /// <param name="groups">The collection containing the known groups for the current dataset.</param>
        /// <param name="groupConnections">The graph that describes the connections between the groups.</param>
        /// <param name="parts">The collection that contains all the parts for the currently selected groups.</param>
        /// <param name="partConnections">The graph that determines how the different parts are connected.</param>
        private CompositionLayer(
            HistoryId id,
            IDictionaryTimelineStorage<GroupRegistrationId, GroupDefinition> definitions,
            IDictionaryTimelineStorage<GroupCompositionId, GroupRegistrationId> groups,
            IBidirectionalGraphHistory<GroupCompositionId, GroupCompositionGraphEdge> groupConnections,
            IDictionaryTimelineStorage<PartCompositionId, PartCompositionInfo> parts,
            IBidirectionalGraphHistory<PartCompositionId, PartCompositionGraphEdge> partConnections)
        {
            {
                Debug.Assert(id != null, "The ID object should not be a null reference.");
                Debug.Assert(definitions != null, "The definition collectino should not be a null reference.");
                Debug.Assert(groups != null, "The groups collection should not be a null reference.");
                Debug.Assert(groupConnections != null, "The group connection graph should not be a null reference.");
                Debug.Assert(parts != null, "The parts collection should not be a null reference.");
                Debug.Assert(partConnections != null, "The part connection graph should not be a null reference.");
            }

            m_HistoryId = id;
            m_Definitions = definitions;
            m_Groups = groups;
            m_GroupConnections = groupConnections;
            m_Parts = parts;
            m_PartConnections = partConnections;
        }

        /// <summary>
        /// Gets the ID which relates the object to the timeline.
        /// </summary>
        public HistoryId HistoryId
        {
            get
            {
                return m_HistoryId;
            }
        }

        /// <summary>
        /// Adds a new <see cref="GroupDefinition"/> to the graph and returns the ID for that group.
        /// </summary>
        /// <param name="id">The ID of the group that is being added.</param>
        /// <param name="group">The group that should be added to the graph.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="id"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="group"/> is <see langword="null" />.
        /// </exception>
        public void Add(GroupCompositionId id, GroupDefinition group)
        {
            {
                Lokad.Enforce.Argument(() => id);
                Lokad.Enforce.Argument(() => group);
            }

            if (!m_Definitions.ContainsKey(group.Id))
            {
                m_Definitions.Add(group.Id, group);
            }

            m_Groups.Add(id, group.Id);
            m_GroupConnections.AddVertex(id);

            foreach (var part in group.Parts)
            {
                m_Parts.Add(new PartCompositionId(id, part.Id), new PartCompositionInfo(part));
            }

            var parts = PartsForGroup(id);
            ConnectParts(group.InternalConnections, parts, parts);
        }

        private IEnumerable<Tuple<PartCompositionId, GroupPartDefinition>> PartsForGroup(GroupCompositionId group)
        {
            return m_Parts
                .Where(pair => pair.Key.Group.Equals(group))
                .Select(pair => new Tuple<PartCompositionId, GroupPartDefinition>(pair.Key, m_Parts[pair.Key].Definition));
        }

        private void ConnectParts(
            IEnumerable<PartImportToPartExportMap> connections,
            IEnumerable<Tuple<PartCompositionId, GroupPartDefinition>> importingParts,
            IEnumerable<Tuple<PartCompositionId, GroupPartDefinition>> exportingParts)
        {
            foreach (var map in connections)
            {
                var importingPart = importingParts.Where(p => p.Item2.RegisteredImports.Contains(map.Import)).FirstOrDefault();
                Debug.Assert(importingPart != null, "Cannot connect parts that are not registered.");

                foreach (var export in map.Exports)
                {
                    var exportingPart = exportingParts.Where(p => p.Item2.RegisteredExports.Contains(export)).FirstOrDefault();
                    m_PartConnections.AddEdge(new PartCompositionGraphEdge(importingPart.Item1, map.Import, exportingPart.Item1, export));
                }
            }
        }

        /// <summary>
        /// Removes the group that is related to the specified ID.
        /// </summary>
        /// <param name="group">The ID of the group that should be removed.</param>
        public void Remove(GroupCompositionId group)
        {
            if (group == null)
            {
                return;
            }

            if (!m_Groups.ContainsKey(group))
            {
                return;
            }

            var definitionId = m_Groups[group];
            var definition = m_Definitions[definitionId];

            foreach (var part in definition.Parts)
            {
                var key = new PartCompositionId(group, part.Id);
                Debug.Assert(m_PartConnections.ContainsVertex(key), "The part connections graph should have the given part ID.");
                m_PartConnections.RemoveVertex(key);

                Debug.Assert(m_Parts.ContainsKey(key), "The part collection should have the given part ID.");
                m_Parts.Remove(key);
            }

            Debug.Assert(m_GroupConnections.ContainsVertex(group), "The connections graph should have the given group ID.");
            m_GroupConnections.RemoveVertex(group);

            m_Groups.Remove(group);
            if (!m_Groups.Where(p => p.Value.Equals(definitionId)).Any())
            {
                m_Definitions.Remove(definitionId);
            }
        }

        /// <summary>
        /// Returns the collection of all known groups.
        /// </summary>
        /// <returns>The collection of all known groups.</returns>
        public IEnumerable<GroupCompositionId> Groups()
        {
            return m_Groups.Keys;
        }

        /// <summary>
        /// Returns the <see cref="GroupDefinition"/> that was registered with the given ID.
        /// </summary>
        /// <param name="id">The composition ID of the group.</param>
        /// <returns>The definition for the group with the given ID.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="id"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="UnknownGroupCompositionIdException">
        ///     Thrown if <paramref name="id"/> does not belong to a known group.
        /// </exception>
        public GroupDefinition Group(GroupCompositionId id)
        {
            {
                Lokad.Enforce.Argument(() => id);
                Lokad.Enforce.With<UnknownGroupCompositionIdException>(
                    m_Groups.ContainsKey(id),
                    Resources.Exceptions_Messages_UnknownGroupCompositionId);
            }

            var definitionId = m_Groups[id];

            Debug.Assert(m_Definitions.ContainsKey(definitionId), "There should be a definition with the given definition ID.");
            return m_Definitions[definitionId];
        }

        /// <summary>
        /// Connects the exporting group with the importing group via the given import.
        /// </summary>
        /// <param name="connection">The object that describes how the group import and the group export should be connected.</param>
        public void Connect(GroupConnection connection)
        {
            {
                Lokad.Enforce.Argument(() => connection);
                Lokad.Enforce.With<UnknownGroupCompositionIdException>(
                    m_Groups.ContainsKey(connection.ImportingGroup),
                    Resources.Exceptions_Messages_UnknownGroupCompositionId);
                Lokad.Enforce.With<UnknownGroupCompositionIdException>(
                    m_Groups.ContainsKey(connection.ExportingGroup),
                    Resources.Exceptions_Messages_UnknownGroupCompositionId);
            }

            m_GroupConnections.AddEdge(new GroupCompositionGraphEdge(connection.ImportingGroup, connection.GroupImport, connection.ExportingGroup));

            // Parts
            var importingParts = PartsForGroup(connection.ImportingGroup);
            var exportingParts = PartsForGroup(connection.ExportingGroup);
            ConnectParts(connection.PartConnections, importingParts, exportingParts);

            // Schedules
        }

        /// <summary>
        /// Disconnects the exporting group from the importing group.
        /// </summary>
        /// <param name="importingGroup">The composition ID of the importing group.</param>
        /// <param name="exportingGroup">The composition ID of the exporting group.</param>
        public void Disconnect(GroupCompositionId importingGroup, GroupCompositionId exportingGroup)
        {
            if ((importingGroup == null) || (exportingGroup == null))
            {
                return;
            }

            if (!m_GroupConnections.ContainsVertex(importingGroup) || (!m_GroupConnections.ContainsVertex(exportingGroup)))
            {
                return;
            }

            var importDefinition = m_GroupConnections
                .InEdges(importingGroup)
                .Where(edge => edge.Source.Equals(exportingGroup))
                .Select(edge => edge.Import)
                .FirstOrDefault();
            m_GroupConnections.RemoveInEdgeIf(importingGroup, edge => edge.Source.Equals(exportingGroup));

            DisconnectParts(importingGroup, importDefinition, exportingGroup);
        }

        private void DisconnectParts(GroupCompositionId importingGroup, GroupImportDefinition importDefinition, GroupCompositionId exportingGroup)
        {
            var definitionId = m_Groups[importingGroup];
            var importingGroupDefinition = m_Definitions[definitionId];
            var importingParts = importDefinition
                .ImportsToMatch
                .Select(id => importingGroupDefinition.Parts.PartByImport(id))
                .Join(
                    m_Parts,
                    partDef => new PartCompositionId(importingGroup, partDef.Id),
                    pair => pair.Key,
                    (partDef, pair) => pair.Key);

            definitionId = m_Groups[exportingGroup];
            var exportingGroupDefinition = m_Definitions[definitionId];
            var exportingParts = exportingGroupDefinition.GroupExport
                .ProvidedExports
                .Select(id => exportingGroupDefinition.Parts.PartByExport(id))
                .Join(
                    m_Parts,
                    partDef => new PartCompositionId(exportingGroup, partDef.Id),
                    pair => pair.Key,
                    (partDef, pair) => pair.Key);

            foreach (var importingPart in importingParts)
            {
                m_PartConnections.RemoveInEdgeIf(importingPart, edge => exportingParts.Contains(edge.Source));
            }
        }

        /// <summary>
        /// Disconnects the group from all imports and exports.
        /// </summary>
        /// <param name="group">The composition ID of the group.</param>
        public void Disconnect(GroupCompositionId group)
        {
            if ((group == null) || !m_GroupConnections.ContainsVertex(group))
            {
                return;
            }

            var matchingExports = m_GroupConnections
                .InEdges(group)
                .Select(edge => new Tuple<GroupCompositionId, GroupImportDefinition>(edge.Source, edge.Import));
            foreach (var pair in matchingExports)
            {
                DisconnectParts(group, pair.Item2, pair.Item1);
            }

            var matchingImports = m_GroupConnections
                .OutEdges(group)
                .Select(edge => new Tuple<GroupCompositionId, GroupImportDefinition>(edge.Target, edge.Import));
            foreach (var pair in matchingImports)
            {
                DisconnectParts(pair.Item1, pair.Item2, group);
            }

            m_GroupConnections.RemoveInEdgeIf(group, edge => true);
            m_GroupConnections.RemoveOutEdgeIf(group, edge => true);
        }

        /// <summary>
        /// Returns a collection of all imports owned by the specified group that have been provided with an export.
        /// </summary>
        /// <param name="importOwner">The composition ID of the group that owns the imports.</param>
        /// <returns>A collection containing all the imports with the group ID of the group providing the connected export.</returns>
        public IEnumerable<Tuple<GroupImportDefinition, GroupCompositionId>> SatisfiedImports(GroupCompositionId importOwner)
        {
            {
                Lokad.Enforce.Argument(() => importOwner);
                Lokad.Enforce.With<UnknownGroupCompositionIdException>(
                    m_Groups.ContainsKey(importOwner),
                    Resources.Exceptions_Messages_UnknownGroupCompositionId);
            }

            return m_GroupConnections.InEdges(importOwner).Select(e => new Tuple<GroupImportDefinition, GroupCompositionId>(e.Import, e.Source));
        }

        /// <summary>
        /// Returns a collection of all imports owned by the specified group that have not been provided with an export.
        /// </summary>
        /// <param name="importOwner">The composition ID of the group that owns the imports.</param>
        /// <returns>A collection containing all imports that have not been provided with an export.</returns>
        public IEnumerable<GroupImportDefinition> UnsatisfiedImports(GroupCompositionId importOwner)
        {
            {
                Lokad.Enforce.Argument(() => importOwner);
                Lokad.Enforce.With<UnknownGroupCompositionIdException>(
                    m_Groups.ContainsKey(importOwner),
                    Resources.Exceptions_Messages_UnknownGroupCompositionId);
            }

            var definitionId = m_Groups[importOwner];

            Debug.Assert(m_Definitions.ContainsKey(definitionId), "There should be a definition for the current group.");
            var definition = m_Definitions[definitionId];

            var inEdges = m_GroupConnections.InEdges(importOwner);
            return definition.GroupImports.Where(i => !inEdges.Any(e => e.Import.Equals(i)));
        }
    }
}
