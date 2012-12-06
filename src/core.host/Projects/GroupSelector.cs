//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Apollo.Core.Base.Plugins;
using Apollo.Core.Extensions.Plugins;
using Apollo.Core.Host.Plugins;

namespace Apollo.Core.Host.Projects
{
    /// <summary>
    /// Defines the interface for objects that provide the ability to select suitable part groups based on a set of selection
    /// criteria.
    /// </summary>
    internal sealed class GroupSelector
    {
        /// <summary>
        /// The object that matches group imports with group exports.
        /// </summary>
        private readonly IConnectGroups m_GroupImportEngine;

        /// <summary>
        /// The graph of connected groups.
        /// </summary>
        private readonly ICompositionLayer m_Graph;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupSelector"/> class.
        /// </summary>
        /// <param name="groupImportEngine">The object that matches group imports with group exports.</param>
        /// <param name="compositionGraph">The object that stores the connections between the different selected groups.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="groupImportEngine"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="compositionGraph"/> is <see langword="null" />.
        /// </exception>
        public GroupSelector(IConnectGroups groupImportEngine, ICompositionLayer compositionGraph)
        {
            {
                Lokad.Enforce.Argument(() => groupImportEngine);
                Lokad.Enforce.Argument(() => compositionGraph);
            }

            m_GroupImportEngine = groupImportEngine;
            m_Graph = compositionGraph;
        }

        /// <summary>
        /// Returns a value indicating if the given export definition can be linked to the given import.
        /// </summary>
        /// <param name="importingGroup">The ID of the group that owns the import.</param>
        /// <param name="importDefinition">The import.</param>
        /// <param name="exportingGroup">The ID of the group that owns the export.</param>
        /// <returns>
        ///     <see langword="true" /> if the export can be linked to the import; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool CanConnectTo(GroupCompositionId importingGroup, GroupImportDefinition importDefinition, GroupCompositionId exportingGroup)
        {
            return CanConnectTo(importingGroup, importDefinition, new Dictionary<string, object>(), exportingGroup);
        }

        /// <summary>
        /// Returns a value indicating if the given export definition can be linked to the given import.
        /// </summary>
        /// <param name="importingGroup">The ID of the group that owns the import.</param>
        /// <param name="importDefinition">The import.</param>
        /// <param name="selectionCriteria">The metadata dictionary that will be used to match the export against.</param>
        /// <param name="exportingGroup">The ID of the group that owns the export.</param>
        /// <returns>
        ///     <see langword="true" /> if the export can be linked to the import; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool CanConnectTo(
            GroupCompositionId importingGroup,
            GroupImportDefinition importDefinition, 
            IDictionary<string, object> selectionCriteria, 
            GroupCompositionId exportingGroup)
        {
            if (!m_Graph.Contains(importingGroup))
            {
                return false;
            }

            if (!m_Graph.Contains(exportingGroup))
            {
                return false;
            }

            if (m_Graph.IsConnected(importingGroup, importDefinition))
            {
                return false;
            }

            var group = m_Graph.Group(exportingGroup);
            return m_GroupImportEngine.Accepts(importDefinition, group.GroupExport) 
                && m_GroupImportEngine.ExportPassesSelectionCriteria(group.GroupExport, selectionCriteria);
        }

        /// <summary>
        /// Returns a collection containing all the groups provide an export which matches the given selection criteria.
        /// </summary>
        /// <param name="selectionCriteria">The collection containing all the selection filters.</param>
        /// <returns>A collection containing all the groups that provide an export which matches the export filters.</returns>
        public IEnumerable<GroupDescriptor> MatchingGroups(IDictionary<string, object> selectionCriteria)
        {
            return MatchingGroups(null, selectionCriteria);
        }

        /// <summary>
        /// Returns a collection containing all the groups which provide an export that can satisfy the given group import.
        /// </summary>
        /// <param name="groupToLinkTo">The import definition which should be satisfied.</param>
        /// <returns>A collection containing all the groups which satisfy the import condition.</returns>
        public IEnumerable<GroupDescriptor> MatchingGroups(GroupImportDefinition groupToLinkTo)
        {
            return MatchingGroups(groupToLinkTo, new Dictionary<string, object>());
        }

        /// <summary>
        /// Returns a collection containing all the groups which satisfy the given group import and match the given
        /// selection criteria.
        /// </summary>
        /// <param name="groupToLinkTo">The import definition which should be satisfied.</param>
        /// <param name="selectionCriteria">The collection containing all the selection filters.</param>
        /// <returns>
        /// A collection containing all the groups which satisfy the given group import and match the given
        /// selection criteria.
        /// </returns>
        public IEnumerable<GroupDescriptor> MatchingGroups(GroupImportDefinition groupToLinkTo, IDictionary<string, object> selectionCriteria)
        {
            return m_GroupImportEngine.MatchingGroups(groupToLinkTo, selectionCriteria)
                .Select(
                    g => new GroupDescriptor(
                        g, 
                        OnGroupSelect, 
                        OnGroupDeselect,
                        OnGroupConnect,
                        OnGroupDisconnect));
        }

        private Task<GroupCompositionId> OnGroupSelect(GroupDefinition exportingGroup)
        {
            return m_Graph.Add(exportingGroup);
        }

        private Task OnGroupDeselect(GroupCompositionId id)
        {
            return m_Graph.Remove(id);
        }

        private Task OnGroupConnect(
            GroupCompositionId importingGroup,
            GroupImportDefinition importDefinition,
            GroupCompositionId exportingGroup,
            GroupExportDefinition exportDefinition)
        {
            if (!m_GroupImportEngine.Accepts(importDefinition, exportDefinition))
            {
                throw new CannotMapExportToImportException();
            }

            return m_Graph.Connect(importingGroup, importDefinition, exportingGroup);
        }

        private Task OnGroupDisconnect(GroupCompositionId importingGroup, GroupCompositionId exportingGroup)
        {
            return m_Graph.Disconnect(importingGroup, exportingGroup);
        }
    }
}
