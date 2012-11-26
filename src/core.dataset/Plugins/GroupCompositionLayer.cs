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
using Apollo.Utilities.History;

namespace Apollo.Core.Dataset.Plugins
{
    /// <summary>
    /// Defines the graph of components groups that describes how the different
    /// groups are connected.
    /// </summary>
    internal class GroupCompositionLayer : IAmHistoryEnabled
    {
        /// <summary>
        /// The history index of the groups field.
        /// </summary>
        private const byte GroupCollectionIndex = 0;

        /// <summary>
        /// The history index of the group connections field.
        /// </summary>
        private const byte GroupConnectionIndex = 1;

        /// <summary>
        /// Creates a new instance of the <see cref="GroupCompositionLayer"/> class with the given 
        /// history information.
        /// </summary>
        /// <param name="id">The history ID for the dataset storage.</param>
        /// <param name="members">The collection that holds all the members for the current object.</param>
        /// <param name="constructorArguments">The optional constructor arguments.</param>
        /// <returns>A new instance of the <see cref="GroupCompositionLayer"/> class.</returns>
        internal static GroupCompositionLayer Build(
            HistoryId id,
            IEnumerable<Tuple<byte, IStoreTimelineValues>> members,
            params object[] constructorArguments)
        {
            {
                Debug.Assert(members.Count() == 2, "There should only be 2 members.");
            }

            IDictionaryTimelineStorage<GroupCompositionId, GroupDefinition> groups = null;
            BidirectionalGraphHistory<GroupCompositionId, GroupCompositionGraphEdge> graph = null;
            foreach (var member in members)
            {
                if (member.Item1 == GroupCollectionIndex)
                {
                    groups = member.Item2 as IDictionaryTimelineStorage<GroupCompositionId, GroupDefinition>;
                    continue;
                }

                if (member.Item1 == GroupConnectionIndex)
                {
                    graph = member.Item2 as BidirectionalGraphHistory<GroupCompositionId, GroupCompositionGraphEdge>;
                    continue;
                }

                throw new UnknownMemberException();
            }

            return new GroupCompositionLayer(id, groups, graph);
        }

        /// <summary>
        /// The ID used by the timeline to uniquely identify the current object.
        /// </summary>
        private readonly HistoryId m_HistoryId;

        /// <summary>
        /// The collection that contains all the currently selected groups.
        /// </summary>
        [FieldIndexForHistoryTracking(GroupCollectionIndex)]
        private readonly IDictionaryTimelineStorage<GroupCompositionId, GroupDefinition> m_Groups;

        /// <summary>
        /// The graph that determines how the different groups are connected.
        /// </summary>
        /// <design>
        /// Note that the edges point from the export to the import.
        /// </design>
        [FieldIndexForHistoryTracking(GroupConnectionIndex)]
        private readonly IBidirectionalGraphHistory<GroupCompositionId, GroupCompositionGraphEdge> m_GroupConnections;

        // Parts?
        // Where are we going to track the different objects that belong to the current dataset?
        // How are we going to decide which objects to delete when stuff changes?

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupCompositionLayer"/> class.
        /// </summary>
        /// <param name="id">The ID used by the timeline to uniquely identify the current object.</param>
        /// <param name="groups">The collection containing the known groups for the current dataset.</param>
        /// <param name="connections">The graph that describes the connections between the groups.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="id"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="groups"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="connections"/> is <see langword="null" />.
        /// </exception>
        private GroupCompositionLayer(
            HistoryId id,
            IDictionaryTimelineStorage<GroupCompositionId, GroupDefinition> groups,
            IBidirectionalGraphHistory<GroupCompositionId, GroupCompositionGraphEdge> connections)
        {
            {
                Lokad.Enforce.Argument(() => id);
                Lokad.Enforce.Argument(() => groups);
                Lokad.Enforce.Argument(() => connections);
            }

            m_HistoryId = id;
            m_Groups = groups;
            m_GroupConnections = connections;
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
        public void Add(GroupCompositionId id, GroupDefinition group)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the group that is related to the specified ID.
        /// </summary>
        /// <param name="group">The ID of the group that should be removed.</param>
        public void Remove(GroupCompositionId group)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the collection of all known groups.
        /// </summary>
        /// <returns>The collection of all known groups.</returns>
        public IEnumerable<GroupCompositionId> Groups()
        {
            throw new NotImplementedException();
        }

        public GroupDefinition Group(GroupCompositionId id)
        {
            throw new NotImplementedException();
        }

        public void Connect(GroupCompositionId importingGroup, GroupImportDefinition importDefinition, GroupCompositionId exportingGroup)
        {
            throw new NotImplementedException();
        }

        public void Disconnect(GroupCompositionId importingGroup, GroupCompositionId exportingGroup)
        {
            throw new NotImplementedException();
        }

        public void Disconnect(GroupCompositionId group)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Tuple<GroupImportDefinition, GroupCompositionId>> SatisfiedImports(GroupCompositionId importOwner)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<GroupImportDefinition> UnsatisfiedImports(GroupCompositionId importingGroup)
        {
            throw new NotImplementedException();
        }
    }
}
