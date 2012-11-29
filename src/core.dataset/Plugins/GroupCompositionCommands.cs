//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apollo.Core.Base.Plugins;

namespace Apollo.Core.Dataset.Plugins
{
    /// <summary>
    /// Defines the commands that allow changing and connecting the currently loaded
    /// part groups.
    /// </summary>
    internal sealed class GroupCompositionCommands : IGroupCompositionCommands
    {
        /// <summary>
        /// The object used to lock the dataset for reading or writing.
        /// </summary>
        private readonly ITrackDatasetLocks m_DatasetLock;

        /// <summary>
        /// The object that stores all the selected groups and their connections.
        /// </summary>
        private readonly IStoreGroupsAndConnections m_Groups;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupCompositionCommands"/> class.
        /// </summary>
        /// <param name="datasetLock">The object used to lock the dataset for reading or writing.</param>
        /// <param name="groups">The object that stores all the selected groups and their connections.</param>
        public GroupCompositionCommands(
            ITrackDatasetLocks datasetLock,
            IStoreGroupsAndConnections groups)
        {
            {
                Lokad.Enforce.Argument(() => datasetLock);
                Lokad.Enforce.Argument(() => groups);
            }

            m_DatasetLock = datasetLock;
            m_Groups = groups;
        }

        /// <summary>
        /// Adds a new <see cref="GroupDefinition"/> to the graph and returns the ID for that group.
        /// </summary>
        /// <param name="id">The ID of the group.</param>
        /// <param name="group">The group that should be added to the graph.</param>
        /// <returns>A task that will finish when the action has completed.</returns>
        public Task Add(GroupCompositionId id, GroupDefinition group)
        {
            var globalTask = Task.Factory.StartNew(
                () =>
                {
                    var key = m_DatasetLock.LockForWriting();
                    try
                    {
                        m_Groups.Add(id, group);
                    }
                    finally
                    {
                        m_DatasetLock.RemoveWriteLock(key);
                    }
                });

            return globalTask;
        }

        /// <summary>
        /// Removes the <see cref="GroupDefinition"/> related to the given <paramref name="id"/> from the graph.
        /// </summary>
        /// <param name="id">The ID of the group that should be removed.</param>
        /// <returns>A task that will finish when the action has completed.</returns>
        public Task Remove(GroupCompositionId id)
        {
            var globalTask = Task.Factory.StartNew(
                () =>
                {
                    var key = m_DatasetLock.LockForWriting();
                    try
                    {
                        m_Groups.Remove(id);
                    }
                    finally
                    {
                        m_DatasetLock.RemoveWriteLock(key);
                    }
                });

            return globalTask;
        }

        /// <summary>
        /// Connects a group import to a group export with the given part and schedule connections described
        /// by the <paramref name="connection"/> object.
        /// </summary>
        /// <param name="connection">The object that describes how the group import and the group export should be connected.</param>
        /// <returns>A task that will finish when the connection action has completed.</returns>
        public Task Connect(GroupConnection connection)
        {
            var globalTask = Task.Factory.StartNew(
                () =>
                {
                    var key = m_DatasetLock.LockForWriting();
                    try
                    {
                        m_Groups.Connect(connection.ImportingGroup, connection.GroupImport, connection.ExportingGroup);
                    }
                    finally
                    {
                        m_DatasetLock.RemoveWriteLock(key);
                    }
                });

            return globalTask;
        }

        /// <summary>
        /// Disconnects the given groups from each other.
        /// </summary>
        /// <param name="importingGroup">The ID of the importing group.</param>
        /// <param name="exportingGroup">The ID of the exporting group.</param>
        /// <returns>A task that will finish when the disconnection action has completed.</returns>
        public Task Disconnect(GroupCompositionId importingGroup, GroupCompositionId exportingGroup)
        {
            var globalTask = Task.Factory.StartNew(
                () =>
                {
                    var key = m_DatasetLock.LockForWriting();
                    try
                    {
                        m_Groups.Disconnect(importingGroup, exportingGroup);
                    }
                    finally
                    {
                        m_DatasetLock.RemoveWriteLock(key);
                    }
                });

            return globalTask;
        }

        /// <summary>
        /// Disconnects the given group from all imports and exports.
        /// </summary>
        /// <param name="group">The group that should be disconnected.</param>
        /// <returns>A task that will finish when the disconnection action has completed.</returns>
        public Task Disconnect(GroupCompositionId group)
        {
            var globalTask = Task.Factory.StartNew(
                () =>
                {
                    var key = m_DatasetLock.LockForWriting();
                    try
                    {
                        m_Groups.Disconnect(group);
                    }
                    finally
                    {
                        m_DatasetLock.RemoveWriteLock(key);
                    }
                });

            return globalTask;
        }

        /// <summary>
        /// Returns a collection containing all the group imports that have not been satisfied.
        /// </summary>
        /// <param name="includeOptionalImports">A flag that indicates if the optional imports should be included.</param>
        /// <returns>
        /// A task that will return the collection of unsatisfied imports.
        /// </returns>
        public Task<IEnumerable<Tuple<GroupCompositionId, GroupImportDefinition>>> NonSatisfiedImports(bool includeOptionalImports)
        {
            var globalTask = Task<IEnumerable<Tuple<GroupCompositionId, GroupImportDefinition>>>.Factory.StartNew(
                () =>
                {
                    var key = m_DatasetLock.LockForReading();
                    try
                    {
                        var groups = m_Groups.Groups()
                            .SelectMany(
                                id => m_Groups.UnsatisfiedImports(id)
                                    .Select(import => new Tuple<GroupCompositionId, GroupImportDefinition>(id, import)))
                            .ToList();

                        return groups;
                    }
                    finally
                    {
                        m_DatasetLock.RemoveReadLock(key);
                    }
                });

            return globalTask;
        }

        /// <summary>
        /// Returns an object containing the current state of the composition graph.
        /// </summary>
        /// <returns>A task that will return the current state of the composition graph.</returns>
        public Task<GroupCompositionState> CurrentState()
        {
            var globalTask = Task<GroupCompositionState>.Factory.StartNew(
                () => 
                {
                    var key = m_DatasetLock.LockForReading();
                    try
                    {
                        var groups = m_Groups.Groups()
                                    .Select(id => new Tuple<GroupCompositionId, GroupDefinition>(id, m_Groups.Group(id)))
                                    .ToList();

                        var connections = m_Groups.Groups()
                            .SelectMany(
                                id => m_Groups.SatisfiedImports(id)
                                    .Select(
                                        import => new Tuple<GroupCompositionId, GroupImportDefinition, GroupCompositionId>(
                                            id,
                                            import.Item1,
                                            import.Item2)))
                            .ToList();

                        return new GroupCompositionState(groups, connections);
                    }
                    finally
                    {
                        m_DatasetLock.RemoveReadLock(key);
                    }
                });

            return globalTask;
        }
    }
}
