//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Apollo.Core.Base.Plugins;
using Apollo.Core.Extensions.Plugins;

namespace Apollo.Core.Dataset.Plugins
{
    /// <summary>
    /// Defines the interface for objects that manage part instances.
    /// </summary>
    internal interface IStoreInstances : IEnumerable<PartInstanceId>
    {
        /// <summary>
        /// Adds a new part definition to the layer and creates an instance of that part with the given constructor parameters.
        /// </summary>
        /// <param name="partDefinition">The part definition of which an instance should be created.</param>
        /// <param name="constructorParameters">The constructor parameters for the new part instance.</param>
        /// <returns>The ID of the newly created part.</returns>
        PartInstanceId CreateInstanceOf(
            GroupPartDefinition partDefinition,
            params Tuple<ImportRegistrationId, PartInstanceId>[] constructorParameters);

        /// <summary>
        /// Removes the part instance with the given ID from the layer.
        /// </summary>
        /// <remarks>
        /// Note that removing a part may also remove other parts if the current part was used as a constructor parameter for those parts.
        /// </remarks>
        /// <param name="part">The ID of the part instance.</param>
        void Remove(PartInstanceId part);

        /// <summary>
        /// Removes all the part instances of which the IDs are given by the collection.
        /// </summary>
        /// <remarks>
        /// Note that removing a part may also remove other parts if the current part was used as a constructor parameter for those parts.
        /// </remarks>
        /// <param name="parts">The collection containing all the instance IDs of the instances that should be removed.</param>
        void Remove(IEnumerable<PartInstanceId> parts);

        /// <summary>
        /// Connects the exporting part instance with the importing part instance.
        /// </summary>
        /// <param name="importingObject">The ID of the importing part instance.</param>
        /// <param name="importingMember">The ID of the import definition which will receive the export.</param>
        /// <param name="export">The ID of the exporting part instance.</param>
        /// <param name="exportingMember">The ID of the export definition which will be used to satisfy the import.</param>
        void Connect(
            PartInstanceId importingObject,
            ImportRegistrationId importingMember,
            PartInstanceId export,
            ExportRegistrationId exportingMember);
    }
}
