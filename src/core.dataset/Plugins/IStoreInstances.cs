﻿//-----------------------------------------------------------------------
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
        /// <param name="importConnections">The collection mapping the part imports to the exports and the parts that provide those exports.</param>
        /// <returns>The ID of the newly created part.</returns>
        PartInstanceId Construct(
            GroupPartDefinition partDefinition,
            IEnumerable<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>> importConnections);

        /// <summary>
        /// Updates the connections of an existing part if they don't match the provided collection. This may result 
        /// in the part object being recreated.
        /// </summary>
        /// <param name="instance">The ID of the part that should be updated.</param>
        /// <param name="importConnections">
        /// The collection mapping the part imports to the exports and the parts that provides those exports.
        /// </param>
        /// <returns>A collection containing the state information for all instances that were touched.</returns>
        IEnumerable<InstanceUpdate> UpdateIfRequired(
            PartInstanceId instance, 
            IEnumerable<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>> importConnections);

        /// <summary>
        /// Removes the part instance with the given ID from the layer.
        /// </summary>
        /// <remarks>
        /// Note that removing a part may also remove other parts if the current part was used as a constructor parameter for those parts.
        /// </remarks>
        /// <param name="instance">The ID of the part instance.</param>
        /// <returns>A collection containing the state information for all instances that were touched.</returns>
        IEnumerable<InstanceUpdate> Release(PartInstanceId instance);

        /// <summary>
        /// Removes all the part instances of which the IDs are given by the collection.
        /// </summary>
        /// <remarks>
        /// Note that removing a part may also remove other parts if the current part was used as a constructor parameter for those parts.
        /// </remarks>
        /// <param name="instances">The collection containing all the instance IDs of the instances that should be removed.</param>
        /// <returns>A collection containing the state information for all instances that were touched.</returns>
        IEnumerable<InstanceUpdate> Release(IEnumerable<PartInstanceId> instances);
    }
}
