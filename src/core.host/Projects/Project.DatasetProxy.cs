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
using System.Threading;
using Apollo.Core.Base;
using Apollo.Core.Base.Loaders;
using Apollo.Core.Host.Properties;
using Apollo.Utilities;
using Lokad;

namespace Apollo.Core.Host.Projects
{
    /// <content>
    /// Defines the implementation of <see cref="IProxyDataset"/>.
    /// </content>
    internal sealed partial class Project
    {
        #region Internal class - DatasetProxy

        /// <summary>
        /// Mirrors the storage of dataset information.
        /// </summary>
        /// <design>
        /// It would be much nicer if this class was private, however if we want to test equality through
        /// the equality contract verifiers then we need a reference to the class.
        /// </design>
        [DebuggerDisplay("ReadonlyDataset: [m_IdOfDataset]")]
        internal sealed class DatasetProxy : IOwnedProxyDataset, IEquatable<DatasetProxy>, IEquatable<IProxyDataset>
        {
            /// <summary>
            /// Implements the operator ==.
            /// </summary>
            /// <param name="first">The first object.</param>
            /// <param name="second">The second object.</param>
            /// <returns>The result of the operator.</returns>
            public static bool operator ==(DatasetProxy first, DatasetProxy second)
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
            public static bool operator !=(DatasetProxy first, DatasetProxy second)
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
            /// Initializes a new instance of the <see cref="DatasetProxy"/> class.
            /// </summary>
            /// <param name="owner">The owner which holds all the dataset information.</param>
            /// <param name="idOfDataset">The ID number of the dataset that is being mirrored.</param>
            public DatasetProxy(Project owner, DatasetId idOfDataset)
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
            /// Deletes the current dataset and all its children.
            /// </summary>
            /// <exception cref="ArgumentException">
            ///     Thrown when the owning project is closed.
            /// </exception>
            public void Delete()
            {
                {
                    Enforce.With<ArgumentException>(!m_Owner.IsClosed, Resources_NonTranslatable.Exception_Messages_CannotUseProjectAfterClosingIt);
                }

                // Note that after this action the current object is no longer 'valid'
                // i.e. it can't be used to connect to the owner anymore other than to
                // check validity.
                //
                // Also note that the delete event is only called indirectly. The parent project
                // will handle that because the current dataset may be deleted because its parent
                // is deleted.
                m_Owner.DeleteDatasetAndChildren(m_IdOfDataset);
            }

            /// <summary>
            /// A method called by the owner when the owner is about to delete the dataset.
            /// </summary>
            void IOwnedProxyDataset.OwnerHasDeletedDataset()
            {
                RaiseOnDeleted();
            }

            /// <summary>
            /// An event raised when the dataset is deleted.
            /// </summary>
            public event EventHandler<EventArgs> OnDeleted;

            private void RaiseOnDeleted()
            {
                var local = OnDeleted;
                if (local != null)
                {
                    local(this, EventArgs.Empty);
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
            /// Gets or sets a value indicating the name of the dataset.
            /// </summary>
            public string Name
            {
                get
                {
                    var dataset = m_Owner.OfflineInformation(m_IdOfDataset);
                    return dataset.Name;
                }

                set
                {
                    var dataset = m_Owner.OfflineInformation(m_IdOfDataset);
                    if (!string.Equals(dataset.Name, value))
                    {
                        dataset.Name = value;
                        RaiseOnNameChanged(value);
                    }
                }
            }

            /// <summary>
            /// An event raised when the name of a dataset is changed.
            /// </summary>
            public event EventHandler<ValueChangedEventArgs<string>> OnNameChanged;

            private void RaiseOnNameChanged(string newName)
            {
                var local = OnNameChanged;
                if (local != null)
                {
                    local(this, new ValueChangedEventArgs<string>(newName));
                }
            }

            /// <summary>
            /// Gets or sets a value describing the dataset.
            /// </summary>
            public string Summary
            {
                get
                {
                    var dataset = m_Owner.OfflineInformation(m_IdOfDataset);
                    return dataset.Summary;
                }

                set
                {
                    var dataset = m_Owner.OfflineInformation(m_IdOfDataset);
                    if (!string.Equals(dataset.Summary, value))
                    {
                        dataset.Summary = value;
                        RaiseOnSummaryChanged(value);
                    }
                }
            }

            /// <summary>
            /// An event raised when the summary of a dataset is changed.
            /// </summary>
            public event EventHandler<ValueChangedEventArgs<string>> OnSummaryChanged;

            private void RaiseOnSummaryChanged(string newSummary)
            {
                var local = OnSummaryChanged;
                if (local != null)
                {
                    local(this, new ValueChangedEventArgs<string>(newSummary));
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
            /// Gets a value indicating whether the dataset can be loaded onto a machine.
            /// </summary>
            public bool CanLoad
            {
                get
                {
                    return m_Owner.CanLoad(m_IdOfDataset);
                }
            }

            /// <summary>
            /// Returns the machine on which the dataset is running.
            /// </summary>
            /// <returns>
            /// The machine on which the dataset is running.
            /// </returns>
            /// <exception cref="DatasetNotLoadedException">
            ///     Thrown when the dataset is not loaded onto a machine.
            /// </exception>
            public NetworkIdentifier RunsOn()
            {
                if (!IsLoaded)
                {
                    throw new DatasetNotLoadedException();
                }

                var online = m_Owner.OnlineInformation(m_IdOfDataset);
                return online.RunsOn;
            }

            /// <summary>
            /// Loads the dataset onto a machine.
            /// </summary>
            /// <param name="preferredLocation">
            /// Indicates a preferred machine location for the dataset to be loaded onto.
            /// </param>
            /// <param name="machineSelector">
            ///     The function that selects the most suitable machine for the dataset to run on.
            /// </param>
            /// <param name="token">The token that is used to cancel the loading.</param>
            /// <remarks>
            /// Note that the <paramref name="preferredLocation"/> is
            /// only a suggestion. The loader may deside to ignore the suggestion if there is a distribution
            /// plan that is better suited to the contents of the dataset.
            /// </remarks>
            /// <exception cref="ArgumentException">
            ///     Thrown when the project that owns this dataset has been closed.
            /// </exception>
            /// <exception cref="CannotLoadDatasetWithoutLoadingLocationException">
            ///     Thrown when the <paramref name="preferredLocation"/> is <see cref="LoadingLocations.None"/>.
            /// </exception>
            /// <exception cref="ArgumentNullException">
            ///     Thrown when <paramref name="machineSelector"/> is <see langword="null" />.
            /// </exception>
            public void LoadOntoMachine(
                LoadingLocations preferredLocation,
                Func<IEnumerable<DistributionSuggestion>, SelectedProposal> machineSelector,
                CancellationToken token)
            {
                {
                    Enforce.With<ArgumentException>(!m_Owner.IsClosed, Resources_NonTranslatable.Exception_Messages_CannotUseProjectAfterClosingIt);
                    Enforce.With<CannotLoadDatasetWithoutLoadingLocationException>(
                        preferredLocation != LoadingLocations.None, 
                        Resources_NonTranslatable.Exception_Messages_CannotLoadDatasetWithoutLoadingLocation);
                    Enforce.Argument(() => machineSelector);
                }

                if (IsLoaded)
                {
                    return;
                }

                m_Owner.LoadOntoMachine(
                    m_IdOfDataset, 
                    preferredLocation,
                    machineSelector,
                    token);
            }

            /// <summary>
            /// A method called by the owner when the owner has progress to report for the
            /// current action that the dataset is executing.
            /// </summary>
            /// <param name="progress">The progress percentage, ranging from 0 to 100.</param>
            /// <param name="mark">The action that is currently being processed.</param>
            /// <param name="estimatedTime">
            ///     The amount of time it will take to finish the entire task from start to finish. Can be negative 
            ///     if no time is known.
            /// </param>
            void IOwnedProxyDataset.OwnerReportsDatasetCurrentActionProgress(int progress, IProgressMark mark, TimeSpan estimatedTime)
            {
                RaiseOnProgressOfCurrentAction(progress, mark, estimatedTime);
            }

            /// <summary>
            /// An event raised when there is progress in the loading of the datset.
            /// </summary>
            public event EventHandler<ProgressEventArgs> OnProgressOfCurrentAction;

            private void RaiseOnProgressOfCurrentAction(int progress, IProgressMark mark, TimeSpan estimatedTime)
            {
                var local = OnProgressOfCurrentAction;
                if (local != null)
                {
                    local(this, new ProgressEventArgs(progress, mark, estimatedTime));
                }
            }

            /// <summary>
            /// Called when the owner has successfully loaded the dataset onto one or more machines.
            /// </summary>
            void IOwnedProxyDataset.OwnerHasLoadedDataset()
            {
                RaiseOnLoaded();
            }

            /// <summary>
            /// An event raised when the dataset is loaded onto one or more machines.
            /// </summary>
            public event EventHandler<EventArgs> OnLoaded;

            private void RaiseOnLoaded()
            {
                var local = OnLoaded;
                if (local != null)
                {
                    local(this, EventArgs.Empty);
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
            /// Called when the owner has successfully unloaded the dataset from the machines it was loaded onto.
            /// </summary>
            void IOwnedProxyDataset.OwnerHasUnloadedDataset()
            {
                RaiseOnLoaded();
            }

            /// <summary>
            /// An event raised when the dataset is unloaded from the machines it was loaded onto.
            /// </summary>
            public event EventHandler<EventArgs> OnUnloaded;

            private void RaiseOnUnloaded()
            {
                var local = OnUnloaded;
                if (local != null)
                {
                    local(this, EventArgs.Empty);
                }
            }

            /// <summary>
            /// Returns the collection of sub-datasets.
            /// </summary>
            /// <returns>
            /// The collection of sub-datasets.
            /// </returns>
            public IEnumerable<IProxyDataset> Children()
            {
                var children = from dataset in m_Owner.Children(m_IdOfDataset)
                               select m_Owner.ObtainProxyFor(dataset.Id);

                return new List<IProxyDataset>(children);
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
            /// <exception cref="ArgumentException">
            ///     Thrown when the owning project has been closed.
            /// </exception>
            /// <exception cref="DatasetCannotBecomeParentException">
            ///     Thrown when the current dataset cannot become a parent.
            /// </exception>
            /// <exception cref="ArgumentNullException">
            ///     Thrown when <paramref name="newChild"/> is <see langword="null" />.
            /// </exception>
            public IProxyDataset CreateNewChild(DatasetCreationInformation newChild)
            {
                {
                    Enforce.With<ArgumentException>(
                        !m_Owner.IsClosed, 
                        Resources_NonTranslatable.Exception_Messages_CannotUseProjectAfterClosingIt);
                    Enforce.With<DatasetCannotBecomeParentException>(
                        CanBecomeParent, 
                        Resources_NonTranslatable.Exception_Messages_DatasetCannotBecomeParent_WithId, 
                        m_IdOfDataset);
                    Enforce.Argument(() => newChild);
                }

                var id = m_Owner.CreateDataset(m_IdOfDataset, newChild);
                return m_Owner.ObtainProxyFor(id);
            }

            /// <summary>
            /// Creates a set of child datasets and returns a collection containing the ID numbers
            /// of the newly created children.
            /// </summary>
            /// <param name="newChildren">The collection containing the information to create the child datasets.</param>
            /// <returns>
            /// A collection containing the ID numbers of the newly created children.
            /// </returns>
            /// <exception cref="ArgumentException">
            ///     Thrown when the owning project has been closed.
            /// </exception>
            /// <exception cref="DatasetCannotBecomeParentException">
            ///     Thrown when the current dataset is not allowed to have child datasets.
            /// </exception>
            /// <exception cref="ArgumentNullException">
            ///     Thrown when <paramref name="newChildren"/> is <see langword="null" />.
            /// </exception>
            /// <exception cref="ArgumentException">
            ///     Thrown when <paramref name="newChildren"/> is an empty collection.
            /// </exception>
            public IEnumerable<IProxyDataset> CreateNewChildren(IEnumerable<DatasetCreationInformation> newChildren)
            {
                {
                    Enforce.With<ArgumentException>(
                        !m_Owner.IsClosed, 
                        Resources_NonTranslatable.Exception_Messages_CannotUseProjectAfterClosingIt);
                    Enforce.With<DatasetCannotBecomeParentException>(
                        CanBecomeParent, 
                        Resources_NonTranslatable.Exception_Messages_DatasetCannotBecomeParent_WithId, 
                        m_IdOfDataset);
                    Enforce.Argument(() => newChildren);
                    Enforce.With<ArgumentException>(
                        newChildren.Any(), 
                        Resources_NonTranslatable.Exception_Messages_MissingCreationInformation);
                }

                var result = from child in newChildren
                                let newDataset = m_Owner.CreateDataset(m_IdOfDataset, child)
                             select m_Owner.ObtainProxyFor(newDataset);

                return new List<IProxyDataset>(result);
            }

            /// <summary>
            /// Gets a value indicating the set of commands that apply to the current dataset.
            /// </summary>
            public IProxyCommandSet Commands
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            /// <summary>
            /// Determines whether the specified <see cref="DatasetProxy"/> is equal to this instance.
            /// </summary>
            /// <param name="other">The <see cref="DatasetProxy"/> to compare with this instance.</param>
            /// <returns>
            ///     <see langword="true"/> if the specified <see cref="DatasetProxy"/> is equal to this instance; otherwise, <see langword="false"/>.
            /// </returns>
            [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
                Justification = "Documentation can start with a language keyword")]
            public bool Equals(DatasetProxy other)
            {
                if (other == null)
                {
                    return false;
                }

                if (ReferenceEquals(this, other))
                {
                    return true;
                }

                return other.m_IdOfDataset.Equals(m_IdOfDataset);
            }

            /// <summary>
            /// Determines whether the specified <see cref="IProxyDataset"/> is equal to this instance.
            /// </summary>
            /// <param name="other">The <see cref="IProxyDataset"/> to compare with this instance.</param>
            /// <returns>
            ///     <see langword="true"/> if the specified <see cref="IProxyDataset"/> is equal to this instance; otherwise, <see langword="false"/>.
            /// </returns>
            [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
                Justification = "Documentation can start with a language keyword")]
            public bool Equals(IProxyDataset other)
            {
                if (other == null)
                {
                    return false;
                }

                if (ReferenceEquals(this, other))
                {
                    return true;
                }

                var dataset = other as DatasetProxy;
                return Equals(dataset);
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

                var dataset = obj as IProxyDataset;
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
