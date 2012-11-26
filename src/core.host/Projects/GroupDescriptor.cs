//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Apollo.Core.Base.Plugins;
using Apollo.Utilities;

namespace Apollo.Core.Host.Projects
{
    /// <summary>
    /// Defines methods for selecting and connecting a group with other groups in the <see cref="IGroupCompositionGraph"/>.
    /// </summary>
    internal sealed class GroupDescriptor
    {
        /// <summary>
        /// The object used to lock on.
        /// </summary>
        private readonly ILockObject m_Lock = new LockObject();
        
        /// <summary>
        /// The object that stores the data about the group.
        /// </summary>
        private readonly GroupDefinition m_Group;

        /// <summary>
        /// The method used to add the current group to the composition graph.
        /// </summary>
        private readonly Func<GroupDefinition, Task<GroupCompositionId>> m_OnSelect;

        /// <summary>
        /// The method used to remove the current group from the composition graph.
        /// </summary>
        private readonly Func<GroupCompositionId, Task> m_OnDeselect;

        /// <summary>
        /// The method used to connect the export of the current group to an import of another group.
        /// </summary>
        private readonly Func<GroupCompositionId, GroupImportDefinition, GroupCompositionId, GroupExportDefinition, Task> m_OnConnect;

        /// <summary>
        /// The method used to disconnect the export of the current group from an import of another group.
        /// </summary>
        private readonly Func<GroupCompositionId, GroupCompositionId, Task> m_OnDisconnect;

        /// <summary>
        /// The composition ID that is provided once the group is added to the composition graph.
        /// </summary>
        private GroupCompositionId m_Id;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupDescriptor"/> class.
        /// </summary>
        /// <param name="group">The object that stores the data describing the current group.</param>
        /// <param name="onSelect">The method used to add the current group to the composition graph.</param>
        /// <param name="onDeselect">The method used to remove the current group from the composition graph.</param>
        /// <param name="onConnect">The method used to connect the export of the current group to an import of another group.</param>
        /// <param name="onDisconnect">The method used to disconnect the export of the current group from an import of another group.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="group"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="onSelect"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="onDeselect"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="onConnect"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="onDisconnect"/> is <see langword="null" />.
        /// </exception>
        public GroupDescriptor(
            GroupDefinition group, 
            Func<GroupDefinition, Task<GroupCompositionId>> onSelect, 
            Func<GroupCompositionId, Task> onDeselect,
            Func<GroupCompositionId, GroupImportDefinition, GroupCompositionId, GroupExportDefinition, Task> onConnect,
            Func<GroupCompositionId, GroupCompositionId, Task> onDisconnect)
        {
            {
                Lokad.Enforce.Argument(() => group);
                Lokad.Enforce.Argument(() => onSelect);
                Lokad.Enforce.Argument(() => onDeselect);
                Lokad.Enforce.Argument(() => onConnect);
                Lokad.Enforce.Argument(() => onDisconnect);
            }

            m_Group = group;
            m_OnSelect = onSelect;
            m_OnDeselect = onDeselect;
            m_OnConnect = onConnect;
            m_OnDisconnect = onDisconnect;
        }

        /// <summary>
        /// Selects the current group and adds it to the composition graph.
        /// </summary>
        /// <returns>The task that will return the ID for the group in the graph.</returns>
        public Task<GroupCompositionId> Select()
        {
            var task = m_OnSelect(m_Group);
            var continuationTask = task.ContinueWith(
                t =>
                {
                    lock (m_Lock)
                    {
                        m_Id = t.Result;
                    }

                    return t.Result;
                });

            return continuationTask;
        }

        /// <summary>
        /// Deselects the current group and removes it from the composition graph.
        /// </summary>
        /// <returns>The task that will finish once the current group is removed from the graph.</returns>
        public Task Deselect()
        {
            return m_OnDeselect(m_Id);
        }

        /// <summary>
        /// Connects the export of the current group to the given import of another group.
        /// </summary>
        /// <param name="group">The ID of the group containing the import.</param>
        /// <param name="importToMatch">The import.</param>
        /// <returns>The task that will finish once the give connection has been made.</returns>
        public Task ConnectTo(GroupCompositionId group, GroupImportDefinition importToMatch)
        {
            return m_OnConnect(group, importToMatch, m_Id, m_Group.GroupExport);
        }

        /// <summary>
        /// Disconnects the current group from the import of the given group.
        /// </summary>
        /// <param name="group">The ID of the importing group.</param>
        /// <returns>The task that will finish once the given connection is removed.</returns>
        public Task DisconnectFrom(GroupCompositionId group)
        {
            return m_OnDisconnect(group, m_Id);
        }
    }
}
