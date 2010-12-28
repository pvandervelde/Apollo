//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Apollo.Core.Base.Projects;
using Apollo.Core.Properties;
using Apollo.Utils;
using Lokad;

namespace Apollo.Core.Projects
{
    /// <content>
    /// Defines the implementation of <see cref="IReadOnlyDataset"/>.
    /// </content>
    internal sealed partial class Project
    {
        #region Internal class - ReadOnlyDataset

        /// <summary>
        /// Mirrors the storage of dataset information.
        /// </summary>
        [DebuggerDisplay("ReadonlyDataset: [m_IdOfDataset]")]
        private sealed class ReadOnlyDataset : MarshalByRefObject, IReadOnlyDataset
        {
            /// <summary>
            /// Implements the operator ==.
            /// </summary>
            /// <param name="first">The first object.</param>
            /// <param name="second">The second object.</param>
            /// <returns>The result of the operator.</returns>
            public static bool operator ==(ReadOnlyDataset first, ReadOnlyDataset second)
            {
                // Check if first is a null reference by using ReferenceEquals because
                // we overload the == operator. If first isn't actually null then
                // we get an infinite loop where we're constantly trying to compare to null.
                if (ReferenceEquals(first, null) && ReferenceEquals(second, null))
                {
                    return true;
                }

                var nonNullObject = first;
                var possibleNullObject = second;
                if (ReferenceEquals(first, null))
                {
                    nonNullObject = second;
                    possibleNullObject = first;
                }

                return nonNullObject.Equals(possibleNullObject);
            }

            /// <summary>
            /// Implements the operator !=.
            /// </summary>
            /// <param name="first">The first object.</param>
            /// <param name="second">The second object.</param>
            /// <returns>The result of the operator.</returns>
            public static bool operator !=(ReadOnlyDataset first, ReadOnlyDataset second)
            {
                // Check if first is a null reference by using ReferenceEquals because
                // we overload the == operator. If first isn't actually null then
                // we get an infinite loop where we're constantly trying to compare to null.
                if (ReferenceEquals(first, null) && ReferenceEquals(second, null))
                {
                    return false;
                }

                var nonNullObject = first;
                var possibleNullObject = second;
                if (ReferenceEquals(first, null))
                {
                    nonNullObject = second;
                    possibleNullObject = first;
                }

                return !nonNullObject.Equals(possibleNullObject);
            }

            /// <summary>
            /// The owner which stores all the data.
            /// </summary>
            private readonly Project m_Owner;

            /// <summary>
            /// The ID number of the dataset that is being mirrored.
            /// </summary>
            private readonly DatasetId m_IdOfDataset;

            /// <summary>
            /// Initializes a new instance of the <see cref="ReadOnlyDataset"/> class.
            /// </summary>
            /// <param name="owner">The owner which holds all the dataset information.</param>
            /// <param name="idOfDataset">The ID number of the dataset that is being mirrored.</param>
            public ReadOnlyDataset(Project owner, DatasetId idOfDataset)
            {
                {
                    Debug.Assert(owner != null, "The owner object should not be a null reference.");
                    Debug.Assert(idOfDataset != null, "The dataset ID should not be a null reference.");
                    Debug.Assert(owner.IsValid(idOfDataset), "The dataset ID should be valid.");
                }

                m_Owner = owner;
                m_IdOfDataset = idOfDataset;
            }

            /// <summary>
            /// Gets a value indicating the ID number of the dataset.
            /// </summary>
            public DatasetId Id
            {
                [DebuggerStepThrough]
                get
                {
                    return m_IdOfDataset;
                }
            }

            /// <summary>
            /// Gets a value indicating whether the current object is valid. 
            /// </summary>
            /// <remarks>
            /// The object can become invalid when:
            /// <list type="bullet">
            /// <item>The project is closed.</item>
            /// <item>The dataset is deleted.</item>
            /// </list>
            /// </remarks>
            public bool IsValid
            {
                get
                {
                    return m_Owner.IsValid(m_IdOfDataset);
                }
            }

            /// <summary>
            /// Gets a value indicating whether the new dataset can be deleted from the
            /// project.
            /// </summary>
            public bool CanBeDeleted
            {
                get
                {
                    var dataset = m_Owner.OfflineInformation(m_IdOfDataset);
                    return dataset.CanBeDeleted;
                }
            }

            /// <summary>
            /// Gets a value indicating whether the new dataset can be moved from one parent
            /// to another parent.
            /// </summary>
            /// <design>
            /// Datasets created by the user are normally movable. Datasets created by the system
            /// may not be movable because it doesn't make sense to move a dataset whose only purpose
            /// is to provide information to the parent.
            /// </design>
            public bool CanBeAdopted
            {
                get
                {
                    var dataset = m_Owner.OfflineInformation(m_IdOfDataset);
                    return dataset.CanBeAdopted;
                }
            }

            /// <summary>
            /// Gets a value indicating whether the new dataset can be copied to another
            /// dataset.
            /// </summary>
            public bool CanBeCopied
            {
                get
                {
                    var dataset = m_Owner.OfflineInformation(m_IdOfDataset);
                    return dataset.CanBeCopied;
                }
            }

            /// <summary>
            /// Gets a value indicating who created the dataset.
            /// </summary>
            public DatasetCreator CreatedBy
            {
                get
                {
                    var dataset = m_Owner.OfflineInformation(m_IdOfDataset);
                    return dataset.CreatedBy;
                }
            }

            /// <summary>
            /// Gets a value indicating from where the datset is or will be 
            /// loaded.
            /// </summary>
            public IPersistenceInformation StoredAt
            {
                get
                {
                    var dataset = m_Owner.OfflineInformation(m_IdOfDataset);
                    return dataset.StoredAt;
                }
            }

            /// <summary>
            /// Gets a value indicating whether the dataset is loaded on the local machine
            /// or a remote machine.
            /// </summary>
            public bool IsLoaded
            {
                get
                {
                    return m_Owner.IsLoaded(m_IdOfDataset);
                }
            }

            /// <summary>
            /// Returns a collection containing information on all the machines
            /// the dataset is distributed over.
            /// </summary>
            /// <returns>
            /// A collection containing information about all the machines the
            /// dataset is distributed over.
            /// </returns>
            public IEnumerable<Machine> RunsOn()
            {
                // Needs to be a list!
                throw new NotImplementedException();
            }

            /// <summary>
            /// Loads the dataset onto a machine.
            /// </summary>
            /// <param name="preferredLocation">
            /// Indicates a preferred machine location for the dataset to be loaded onto.
            /// </param>
            /// <param name="range">
            /// The number of machines over which the data set should be distributed.
            /// </param>
            /// <remarks>
            /// Note that the <paramref name="preferredLocation"/> and the <paramref name="range"/> are
            /// only suggestions. The loader may deside to ignore the suggestions if there is a distribution
            /// plan that is better suited to the contents of the dataset.
            /// </remarks>
            public void LoadOntoMachine(LoadingLocation preferredLocation, MachineDistributionRange range)
            {
                {
                    Enforce.With<CannotLoadDatasetWithoutLoadingLocationException>(
                        preferredLocation != LoadingLocation.None, 
                        Resources_NonTranslatable.Exception_Messages_CannotLoadDatasetWithoutLoadingLocation);
                    Enforce.Argument(() => range);
                }

                if (IsLoaded)
                {
                    return;
                }

                m_Owner.LoadOntoMachine(m_IdOfDataset, preferredLocation, range);
                if (IsLoaded)
                {
                    var observers = m_Owner.Observers(m_IdOfDataset);
                    foreach (var observer in observers)
                    {
                        observer.DatasetLoaded(new List<Machine>(RunsOn()));
                    }
                }
            }

            /// <summary>
            /// Unloads the dataset from the machine it is currently loaded onto.
            /// </summary>
            public void UnloadFromMachine()
            {
                if (!IsLoaded)
                {
                    return;
                }

                m_Owner.UnloadFromMachine(m_IdOfDataset);
                if (!IsLoaded)
                {
                    var observers = m_Owner.Observers(m_IdOfDataset);
                    foreach (var observer in observers)
                    {
                        observer.DatasetUnloaded();
                    }
                }
            }

            /// <summary>
            /// Returns the collection of sub-datasets.
            /// </summary>
            /// <returns>
            /// The collection of sub-datasets.
            /// </returns>
            public IEnumerable<IReadOnlyDataset> Children()
            {
                var children = from dataset in m_Owner.Children(m_IdOfDataset)
                               select m_Owner.ObtainProxyFor(dataset.Id);

                return new List<IReadOnlyDataset>(children);
            }

            /// <summary>
            /// Gets a value indicating whether the current dataset is allowed to request the 
            /// creation of its own children.
            /// </summary>
            /// <design>
            /// Normally all datasets created by the user are allowed to create their own 
            /// children. In some cases datasets created by the system are blocked from 
            /// creating their own children.
            /// </design>
            public bool CanBecomeParent
            {
                get
                {
                    var information = m_Owner.OfflineInformation(m_IdOfDataset);
                    return information.CanBecomeParent;
                }
            }

            /// <summary>
            /// Creates a new child dataset and returns the ID number of the child.
            /// </summary>
            /// <param name="newChild">The information required to create the new child.</param>
            /// <returns>
            /// The ID number of the child.
            /// </returns>
            /// <exception cref="DatasetCannotBecomeParentException">
            /// Thrown when the current dataset cannot become a parent.
            /// </exception>
            public IReadOnlyDataset CreateNewChild(DatasetCreationInformation newChild)
            {
                {
                    Enforce.With<DatasetCannotBecomeParentException>(CanBecomeParent, Resources_NonTranslatable.Exception_Messages_DatasetCannotBecomeParent_WithId, m_IdOfDataset);
                    Enforce.Argument(() => newChild);
                }

                var id = m_Owner.CreateDataset(m_IdOfDataset, newChild);
                return new ReadOnlyDataset(m_Owner, id);
            }

            /// <summary>
            /// Creates a set of child datasets and returns a collection containing the ID numbers
            /// of the newly created children.
            /// </summary>
            /// <param name="newChildren">The collection containing the information to create the child datasets.</param>
            /// <returns>
            /// A collection containing the ID numbers of the newly created children.
            /// </returns>
            public IEnumerable<IReadOnlyDataset> CreateNewChildren(IEnumerable<DatasetCreationInformation> newChildren)
            {
                {
                    Enforce.With<DatasetCannotBecomeParentException>(CanBecomeParent, Resources_NonTranslatable.Exception_Messages_DatasetCannotBecomeParent_WithId, m_IdOfDataset);
                    Enforce.Argument(() => newChildren);
                    Enforce.With<ArgumentException>(newChildren.Any(), Resources_NonTranslatable.Exception_Messages_MissingCreationInformation);
                }

                var result = from child in newChildren
                                let newDataset = m_Owner.CreateDataset(m_IdOfDataset, child)
                             select new ReadOnlyDataset(m_Owner, newDataset) as IReadOnlyDataset;

                return new List<IReadOnlyDataset>(result);
            }

            /// <summary>
            /// Gets a value indicating the set of commands that apply to the current dataset.
            /// </summary>
            public ProxyCommandSet Commands
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            /// <summary>
            /// Registers the given object for change notifications.
            /// </summary>
            /// <param name="observer">The object that wants to receive change notifications.</param>
            public void RegisterForEvents(INotifyOnDatasetChange observer)
            {
                {
                    Enforce.Argument(() => observer);
                }

                m_Owner.RegisterForEvents(m_IdOfDataset, observer);
            }

            /// <summary>
            /// Unregisters the given object for change notifications.
            /// </summary>
            /// <param name="observer">The object that is registered for change notifications.</param>
            public void UnregisterForEvents(INotifyOnDatasetChange observer)
            {
                {
                    Enforce.Argument(() => observer);
                }

                m_Owner.UnregisterForEvents(m_IdOfDataset, observer);
            }

            /// <summary>
            /// Obtains a lifetime service object to control the lifetime policy for this instance.
            /// </summary>
            /// <returns>
            /// An object of type <see cref="T:System.Runtime.Remoting.Lifetime.ILease"/> used to control the lifetime policy for this instance. This is the current lifetime service object for this instance if one exists; otherwise, a new lifetime service object initialized to the value of the <see cref="P:System.Runtime.Remoting.Lifetime.LifetimeServices.LeaseManagerPollTime"/> property.
            /// </returns>
            /// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission. 
            /// </exception>
            /// <filterpriority>2</filterpriority>
            /// <PermissionSet>
            ///     <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="RemotingConfiguration, Infrastructure"/>
            /// </PermissionSet>
            public override object InitializeLifetimeService()
            {
                // We don't really want the system to GC our object at random times...
                return null;
            }

            /// <summary>
            /// Determines whether the specified <see cref="IReadOnlyDataset"/> is equal to this instance.
            /// </summary>
            /// <param name="other">The <see cref="IReadOnlyDataset"/> to compare with this instance.</param>
            /// <returns>
            ///     <see langword="true"/> if the specified <see cref="IReadOnlyDataset"/> is equal to this instance; otherwise, <see langword="false"/>.
            /// </returns>
            [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
                Justification = "Documentation can start with a language keyword")]
            public bool Equals(IReadOnlyDataset other)
            {
                if (other == null)
                {
                    return false;
                }

                if (ReferenceEquals(this, other))
                {
                    return true;
                }

                var dataset = other as ReadOnlyDataset;
                return (dataset != null) && dataset.m_IdOfDataset.Equals(m_IdOfDataset);
            }

            /// <summary>
            /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
            /// </summary>
            /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
            /// <returns>
            ///     <see langword="true"/> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <see langword="false"/>.
            /// </returns>
            [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
                Justification = "Documentation can start with a language keyword")]
            public override bool Equals(object obj)
            {
                if (ReferenceEquals(this, obj))
                {
                    return true;
                }

                var dataset = obj as IReadOnlyDataset;
                return Equals(dataset);
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <returns>
            /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
            /// </returns>
            public override int GetHashCode()
            {
                return m_IdOfDataset.GetHashCode();
            }

            /// <summary>
            /// Returns a <see cref="System.String"/> that represents this instance.
            /// </summary>
            /// <returns>
            /// A <see cref="System.String"/> that represents this instance.
            /// </returns>
            public override string ToString()
            {
                return string.Format(CultureInfo.InvariantCulture, "Read-only dataset front for dataset with ID: {0}", m_IdOfDataset);
            }
        }
        
        #endregion
    }
}
