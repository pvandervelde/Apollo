//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Apollo.Core.Base.Plugins;
using Apollo.Core.Extensions.Plugins;
using Apollo.Utilities;
using Apollo.Utilities.History;

namespace Apollo.Core.Dataset.Plugins
{
    /// <summary>
    /// Stores the part instances and their connections.
    /// </summary>
    internal sealed class PartInstanceLayer : IStorePartInstances, IAmHistoryEnabled
    {
        /// <summary>
        /// The history index of the history object collection field.
        /// </summary>
        private const byte HistoryObjectCollectionIndex = 0;

        /// <summary>
        /// The history index of the non-history object collection field.
        /// </summary>
        private const byte NonHistoryObjectDefinitionCollectionIndex = 1;

        /// <summary>
        /// Creates a new instance of the <see cref="PartInstanceLayer"/> class with the given 
        /// history information.
        /// </summary>
        /// <param name="id">The history ID for the part instance layer.</param>
        /// <param name="members">The collection that holds all the members for the current object.</param>
        /// <param name="constructorArguments">The optional constructor arguments.</param>
        /// <returns>A new instance of the <see cref="PartInstanceLayer"/> class.</returns>
        internal static PartInstanceLayer Build(
            HistoryId id,
            IEnumerable<Tuple<byte, IStoreTimelineValues>> members,
            params object[] constructorArguments)
        {
            {
                Debug.Assert(members.Count() == 2, "There should only be 2 members.");
            }

            IDictionaryTimelineStorage<PartCompositionId, IAmHistoryEnabled> historyObjects = null;
            IDictionaryTimelineStorage<PartCompositionId, GroupPartDefinition> nonHistoryObjectDefinitions = null;
            foreach (var member in members)
            {
                if (member.Item1 == HistoryObjectCollectionIndex)
                {
                    historyObjects = member.Item2 as IDictionaryTimelineStorage<PartCompositionId, IAmHistoryEnabled>;
                    continue;
                }

                if (member.Item1 == NonHistoryObjectDefinitionCollectionIndex)
                {
                    nonHistoryObjectDefinitions = member.Item2 as IDictionaryTimelineStorage<PartCompositionId, GroupPartDefinition>;
                    continue;
                }

                throw new UnknownMemberException();
            }

            return new PartInstanceLayer(id, historyObjects, nonHistoryObjectDefinitions);
        }

        /// <summary>
        /// The object used to lock on.
        /// </summary>
        private readonly ILockObject m_Lock = new LockObject();

        /// <summary>
        /// The collection of part instances that should not be tracked in history. All part instances in this
        /// collection will be disposed of when history roll-back or roll-forward occurs.
        /// </summary>
        private readonly IDictionary<PartCompositionId, object> m_NonHistoryObjects
            = new Dictionary<PartCompositionId, object>();

        /// <summary>
        /// The collection of part instances that are tracked in history.
        /// </summary>
        [FieldIndexForHistoryTracking(HistoryObjectCollectionIndex)]
        private readonly IDictionaryTimelineStorage<PartCompositionId, IAmHistoryEnabled> m_HistoryObjects;

        /// <summary>
        /// The collection contraining the part definitions for the parts that do not participate in history tracking.
        /// </summary>
        [FieldIndexForHistoryTracking(NonHistoryObjectDefinitionCollectionIndex)]
        private readonly IDictionaryTimelineStorage<PartCompositionId, GroupPartDefinition> m_NonHistoryObjectDefinitions;

        /// <summary>
        /// The ID used by the timeline to uniquely identify the current object.
        /// </summary>
        private readonly HistoryId m_HistoryId;

        /// <summary>
        /// Initializes a new instance of the <see cref="PartInstanceLayer"/> class.
        /// </summary>
        /// <param name="id">The ID used by the timeline to uniquely identify the current object.</param>
        /// <param name="historyObjects">The collection that stores the part instances that partake in history tracking.</param>
        /// <param name="nonHistoryObjectDefinitions">
        /// The collection of part definitions of the parts that do not partake in history tracking.
        /// </param>
        private PartInstanceLayer(
            HistoryId id,
            IDictionaryTimelineStorage<PartCompositionId, IAmHistoryEnabled> historyObjects,
            IDictionaryTimelineStorage<PartCompositionId, GroupPartDefinition> nonHistoryObjectDefinitions)
        {
            {
                Debug.Assert(id != null, "The history ID object should not be a null reference.");
                Debug.Assert(historyObjects != null, "The histor object collection should not be a null reference.");
                Debug.Assert(nonHistoryObjectDefinitions != null, "The construction definition collection should not be a null reference.");
            }

            m_HistoryId = id;
            m_HistoryObjects = historyObjects;
            m_NonHistoryObjectDefinitions = nonHistoryObjectDefinitions;
            m_NonHistoryObjectDefinitions.OnExternalValueUpdate += 
                (s, e) => 
                {
                    lock (m_Lock)
                    {
                        // Create all the objects again and connect them(?)
                        m_NonHistoryObjects.Clear();
                    }
                };
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
        /// Adds a new part definition to the layer and creates an instance of that part with the given constructor parameters.
        /// </summary>
        /// <param name="partDefinition">The part definition of which an instance should be created.</param>
        /// <param name="constructorParameters">The constructor parameters for the new part instance.</param>
        /// <returns>The ID of the newly created part.</returns>
        public PartCompositionId CreateInstanceOf(
            GroupPartDefinition partDefinition, 
            params Tuple<ImportRegistrationId, PartCompositionId>[] constructorParameters)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the part instance with the given ID from the layer.
        /// </summary>
        /// <remarks>
        /// Note that removing a part may also remove other parts if the current part was used as a constructor parameter for those parts.
        /// </remarks>
        /// <param name="part">The ID of the part instance.</param>
        public void Remove(PartCompositionId part)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes all the part instances of which the IDs are given by the collection.
        /// </summary>
        /// <remarks>
        /// Note that removing a part may also remove other parts if the current part was used as a constructor parameter for those parts.
        /// </remarks>
        /// <param name="parts">The collection containing all the instance IDs of the instances that should be removed.</param>
        public void Remove(IEnumerable<PartCompositionId> parts)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Connects the exporting part instance with the importing part instance.
        /// </summary>
        /// <param name="importingObject">The ID of the importing part instance.</param>
        /// <param name="importingMember">The ID of the import definition which will receive the export.</param>
        /// <param name="export">The ID of the exporting part instance.</param>
        /// <param name="exportingMember">The ID of the export definition which will be used to satisfy the import.</param>
        public void Connect(
            PartCompositionId importingObject, 
            ImportRegistrationId importingMember, 
            PartCompositionId export, 
            ExportRegistrationId exportingMember)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///  Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <returns>
        /// The element in the collection at the current position of the enumerator.
        /// </returns>
        public IEnumerator<PartCompositionId> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An System.Collections.IEnumerator object that can be used to iterate through 
        /// the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
