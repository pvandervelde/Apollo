//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Apollo.Core.Base.Projects;
using Apollo.Core.Properties;
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
        private sealed class ReadOnlyDataset : MarshalByRefObject, IReadOnlyDataset
        {
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
                    Debug.Assert(idOfDataset.IsValid(), "The dataset ID should be valid.");
                }

                m_Owner = owner;
                m_IdOfDataset = idOfDataset;
            }

            /// <summary>
            /// Gets a value indicating the ID number of the dataset.
            /// </summary>
            public DatasetId Id
            {
                get
                {
                    return m_IdOfDataset;
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
                    return dataset.ReasonForExistence.CanBeDeleted;
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
                    return dataset.ReasonForExistence.CanBeAdopted;
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
                    return dataset.ReasonForExistence.CanBeCopied;
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
                    RaiseOnLoaded(new List<Machine>(RunsOn()));
                }
            }

            /// <summary>
            /// An event fired after the dataset has been distributed to one or more machines.
            /// </summary>
            public event EventHandler<DatasetLoadEventArgs> OnLoaded;

            private void RaiseOnLoaded(List<Machine> machines)
            {
                EventHandler<DatasetLoadEventArgs> local = OnLoaded;
                if (local != null)
                { 
                    local(this, new DatasetLoadEventArgs(m_IdOfDataset, machines));
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
                    RaiseOnUnloaded();
                }
            }

            /// <summary>
            /// An event fired after the dataset has been unloaded from the machines it was loaded onto.
            /// </summary>
            public event EventHandler<DatasetUnloadEventArgs> OnUnloaded;

            private void RaiseOnUnloaded()
            {
                EventHandler<DatasetUnloadEventArgs> local = OnUnloaded;
                if (local != null)
                {
                    local(this, new DatasetUnloadEventArgs(m_IdOfDataset));
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
                               select new ReadOnlyDataset(m_Owner, dataset.Id) as IReadOnlyDataset;

                return children;
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
                    return information.ReasonForExistence.CanBecomeParent;
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
        }
        
        #endregion
    }
}
