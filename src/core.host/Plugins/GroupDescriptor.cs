//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Core.Base.Plugins;

namespace Apollo.Core.Host.Plugins
{
    /// <summary>
    /// Defines methods for selecting and connecting a group with other groups in the <see cref="GroupCompositionGraph"/>.
    /// </summary>
    internal sealed class GroupDescriptor
    {
        /// <summary>
        /// The object that stores the data about the group.
        /// </summary>
        private readonly GroupDefinition m_Group;

        /// <summary>
        /// The method used to add the current group to the composition graph.
        /// </summary>
        private readonly Func<GroupDefinition, GroupCompositionId> m_OnSelect;

        /// <summary>
        /// The method used to remove the current group from the composition graph.
        /// </summary>
        private readonly Action<GroupCompositionId> m_OnDeselect;

        /// <summary>
        /// The method used to connect the export of the current group to an import of another group.
        /// </summary>
        private readonly Action<GroupCompositionId, GroupImportDefinition, GroupCompositionId, GroupExportDefinition> m_OnConnect;

        /// <summary>
        /// The method used to disconnect the export of the current group from an import of another group.
        /// </summary>
        private readonly Action<GroupCompositionId, GroupCompositionId> m_OnDisconnect;

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
            Func<GroupDefinition, GroupCompositionId> onSelect, 
            Action<GroupCompositionId> onDeselect,
            Action<GroupCompositionId, GroupImportDefinition, GroupCompositionId, GroupExportDefinition> onConnect,
            Action<GroupCompositionId, GroupCompositionId> onDisconnect)
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
        /// <returns>The ID for the group in the graph.</returns>
        public GroupCompositionId Select()
        {
            m_Id = m_OnSelect(m_Group);
            return m_Id;
        }

        /// <summary>
        /// Deselects the current group and removes it from the composition graph.
        /// </summary>
        public void Deselect()
        {
            m_OnDeselect(m_Id);
        }

        /// <summary>
        /// Connects the export of the current group to the given import of another group.
        /// </summary>
        /// <param name="group">The ID of the group containing the import.</param>
        /// <param name="importToMatch">The import.</param>
        public void ConnectTo(GroupCompositionId group, GroupImportDefinition importToMatch)
        {
            m_OnConnect(group, importToMatch, m_Id, m_Group.GroupExport);
        }

        /// <summary>
        /// Disconnects the current group from the import of the given group.
        /// </summary>
        /// <param name="group">The ID of the importing group.</param>
        public void DisconnectFrom(GroupCompositionId group)
        {
            m_OnDisconnect(group, m_Id);
        }
    }
}
