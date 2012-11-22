//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
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
    internal class GroupCompositionGraph : IGroupCompositionGraph, IAmHistoryEnabled, ILoadHistoryPatches, ICreateHistoryPatches
    {
        /// <summary>
        /// Adds a new <see cref="GroupDefinition"/> to the graph and returns the ID for that group.
        /// </summary>
        /// <param name="group">The group that should be added to the graph.</param>
        /// <returns>
        /// The ID for the group.
        /// </returns>
        public GroupCompositionId Add(GroupDefinition group)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the group that is related to the specified ID.
        /// </summary>
        /// <param name="group">The ID of the group that should be removed.</param>
        public void Remove(GroupCompositionId group)
        {
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the <see cref="GroupDefinition"/> which is related to the given ID.
        /// </summary>
        /// <param name="id">The ID of the requested group.</param>
        /// <returns>The requested group.</returns>
        public GroupDefinition Group(GroupCompositionId id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the collection of all known groups.
        /// </summary>
        /// <returns>The collection of all known groups.</returns>
        public IEnumerable<GroupDefinition> Groups()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Connects the given export to the given import.
        /// </summary>
        /// <param name="importingGroup">The ID of the group that owns the import.</param>
        /// <param name="importDefinition">The import.</param>
        /// <param name="exportingGroup">The ID of the group that owns the export.</param>
        public void Connect(
            GroupCompositionId importingGroup,
            GroupImportDefinition importDefinition,
            GroupCompositionId exportingGroup)
        {
            // How are we going to do this?
            // We should really use the PartImportEngine for this
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
        public void Disconnect(GroupCompositionId importingGroup, GroupCompositionId exportingGroup)
        {
        }

        /// <summary>
        /// Disconnects all connection to and from the given group.
        /// </summary>
        /// <param name="group">The ID of the group.</param>
        public void Disconnect(GroupCompositionId group)
        {
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the ID which relates the object to the timeline.
        /// </summary>
        public HistoryId HistoryId
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Creates a patch containing all the available changes.
        /// </summary>
        /// <returns>The patch that contains all the history changes for the current object.</returns>
        public HistoryPatch ToPatch()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a patch containing all the changes since the given start time.
        /// </summary>
        /// <param name="start">The start time.</param>
        /// <param name="includeBase">A flag indicating if the patch should include the complete state at the start or not.</param>
        /// <returns>The patch that contains all the history changes for the current object.</returns>
        public HistoryPatch ToPatch(TimeMarker start, bool includeBase)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a patch containing only the current state of the object.
        /// </summary>
        /// <returns>The patch that contains all the history changes for the current object.</returns>
        public HistoryPatch LatestToPatch()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Loads the patch and resets the state of the current object to the 
        /// state found in the patch.
        /// </summary>
        /// <param name="patch">The patch to load.</param>
        public void LoadFromPatch(HistoryPatch patch)
        {
            throw new NotImplementedException();
        }
    }
}
