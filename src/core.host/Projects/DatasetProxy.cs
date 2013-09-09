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
using System.Threading.Tasks;
using Apollo.Core.Base;
using Apollo.Core.Base.Activation;
using Apollo.Core.Host.Properties;
using Apollo.Utilities;
using Apollo.Utilities.History;

namespace Apollo.Core.Host.Projects
{
    /// <summary>
    /// Mirrors the storage of dataset information.
    /// </summary>
    internal sealed class DatasetProxy
        : IProxyDataset,
          IAmHistoryEnabled,
          INeedNotificationOnHistoryChange,
          IEquatable<DatasetProxy>
    {
        /// <summary>
        /// The history index of the name field.
        /// </summary>
        private const byte NameIndex = 0;

        /// <summary>
        /// The history index of the summary field.
        /// </summary>
        private const byte SummaryIndex = 1;

        /// <summary>
        /// The history index of the IsLoaded field.
        /// </summary>
        private const byte LoadedLocationIndex = 2;

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
        /// Creates a new instance of the <see cref="DatasetProxy"/> class with the given data that was stored in the history system.
        /// </summary>
        /// <param name="historyId">The ID of the current object as used by the timeline.</param>
        /// <param name="members">The collection of member fields that are stored in the timeline.</param>
        /// <param name="constructorArguments">The arguments needed to create the proxy.</param>
        /// <returns>A new instance of the <see cref="DatasetProxy"/> class.</returns>
        /// <exception cref="UnknownMemberException">
        ///     Thrown if <paramref name="members"/> contains member data for an unknown member.
        /// </exception>
        internal static DatasetProxy CreateInstance(
            HistoryId historyId,
            IEnumerable<Tuple<byte, IStoreTimelineValues>> members,
            params object[] constructorArguments)
        {
            {
                Debug.Assert(members.Count() == 3, "There should only be three members.");
                Debug.Assert(constructorArguments.Length == 2, "There should be only two constructor arguments.");
            }

            IVariableTimeline<string> name = null;
            IVariableTimeline<string> summary = null;
            IVariableTimeline<NetworkIdentifier> loadedLocation = null;
            foreach (var member in members)
            {
                if (member.Item1 == NameIndex)
                {
                    name = member.Item2 as IVariableTimeline<string>;
                    continue;
                }

                if (member.Item1 == SummaryIndex)
                {
                    summary = member.Item2 as IVariableTimeline<string>;
                    continue;
                }

                if (member.Item1 == LoadedLocationIndex)
                {
                    loadedLocation = member.Item2 as IVariableTimeline<NetworkIdentifier>;
                    continue;
                }

                throw new UnknownMemberException();
            }

            return new DatasetProxy(
                constructorArguments[0] as DatasetConstructionParameters,
                constructorArguments[1] as Func<DatasetOnlineInformation, DatasetStorageProxy>,
                historyId,
                name,
                summary,
                loadedLocation);
        }

        /// <summary>
        /// The constructor arguments.
        /// </summary>
        private readonly DatasetConstructionParameters m_ConstructorArgs;

        /// <summary>
        /// The function that is used to create the proxy for the data inside the dataset.
        /// </summary>
        private readonly Func<DatasetOnlineInformation, DatasetStorageProxy> m_ProxyBuilder;

        /// <summary>
        /// The ID used by the timeline to uniquely identify the current object.
        /// </summary>
        private readonly HistoryId m_HistoryId;

        /// <summary>
        /// The name of the project.
        /// </summary>
        [FieldIndexForHistoryTracking(NameIndex)]
        private readonly IVariableTimeline<string> m_Name;

        /// <summary>
        /// The summary for the project.
        /// </summary>
        [FieldIndexForHistoryTracking(SummaryIndex)]
        private readonly IVariableTimeline<string> m_Summary;

        /// <summary>
        /// The information that describes if the dataset is loaded at a given point on the
        /// timeline.
        /// </summary>
        [FieldIndexForHistoryTracking(LoadedLocationIndex)]
        private readonly IVariableTimeline<NetworkIdentifier> m_LoadedLocation;

        /// <summary>
        /// The data that describes the online state of the dataset.
        /// </summary>
        private DatasetOnlineInformation m_Connection;

        /// <summary>
        /// The object that provides the proxy for the actual data stored in the dataset.
        /// </summary>
        private DatasetStorageProxy m_DataProxy;

        /// <summary>
        /// Indicates if the dataset is currently loading.
        /// </summary>
        private volatile bool m_IsLoading;

        /// <summary>
        /// Indicates if the dataset has been deleted.
        /// </summary>
        private volatile bool m_HasBeenDeleted;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetProxy"/> class.
        /// </summary>
        /// <param name="constructorArgs">The object that holds all the constructor arguments that do not belong to the timeline.</param>
        /// <param name="proxyBuilder">The function that is used to create the proxy for the data inside the dataset.</param>
        /// <param name="historyId">The ID for use in the history system.</param>
        /// <param name="name">The data structure that stores the history information for the name of the dataset.</param>
        /// <param name="summary">The data structure that stores the history information for the summary of the dataset.</param>
        /// <param name="loadedLocation">
        /// The data structure that stores the history information indicating if the dataset is loaded at any point.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="constructorArgs"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="proxyBuilder"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="historyId"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="name"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="summary"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="loadedLocation"/> is <see langword="null" />.
        /// </exception>
        private DatasetProxy(
            DatasetConstructionParameters constructorArgs,
            Func<DatasetOnlineInformation, DatasetStorageProxy> proxyBuilder,
            HistoryId historyId,
            IVariableTimeline<string> name,
            IVariableTimeline<string> summary,
            IVariableTimeline<NetworkIdentifier> loadedLocation)
        {
            {
                Lokad.Enforce.Argument(() => constructorArgs);
                Lokad.Enforce.Argument(() => proxyBuilder);
                Lokad.Enforce.Argument(() => historyId);
                Lokad.Enforce.Argument(() => name);
                Lokad.Enforce.Argument(() => summary);
                Lokad.Enforce.Argument(() => loadedLocation);
            }

            m_ConstructorArgs = constructorArgs;
            m_ProxyBuilder = proxyBuilder;
            m_HistoryId = historyId;
            m_Name = name;
            m_Summary = summary;
            m_LoadedLocation = loadedLocation;

            m_Name.OnExternalValueUpdate += new EventHandler<EventArgs>(HandleOnNameUpdate);
            m_Summary.OnExternalValueUpdate += new EventHandler<EventArgs>(HandleOnSummaryUpdate);
            m_LoadedLocation.OnExternalValueUpdate += new EventHandler<EventArgs>(HandleOnIsLoadedUpdate);
        }

        private void HandleOnNameUpdate(object sender, EventArgs e)
        {
            RaiseOnNameChanged(Name);
        }

        private void HandleOnSummaryUpdate(object sender, EventArgs e)
        {
            RaiseOnSummaryChanged(Summary);
        }

        private void HandleOnIsLoadedUpdate(object sender, EventArgs e)
        {
            // See if the restored status still matches up with the online status of the dataset
            // If not make sure that it does
            if (m_LoadedLocation.Current != null)
            {
                if (m_Connection == null)
                {
                    // For now we go with the naive approach. Load on the same machine as last time
                    // and if we can't find it then grab the first machine in the selection.
                    // At some point we should make this a whole lot smarter but for now it'll do.
                    var original = m_LoadedLocation.Current;
                    Func<IEnumerable<DistributionSuggestion>, SelectedProposal> selector =
                        c =>
                        {
                            var selected = from suggestion in c
                                           where suggestion.Plan.MachineToDistributeTo.Equals(original)
                                           select suggestion;

                            DistributionPlan plan = selected.Any()
                                ? selected.First().Plan
                                : c.Any()
                                    ? c.First().Plan
                                    : null;

                            return new SelectedProposal(plan);
                        };

                    LoadOntoMachine(
                        DistributionLocations.All,
                        selector,
                        new CancellationTokenSource().Token,
                        false);
                }
            }
            else
            {
                if (m_Connection != null)
                {
                    UnloadFromMachine(false);
                }
            }
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
        /// Provides implementing classes with the ability to clean-up before
        /// the object is removed from history.
        /// </summary>
        public void BeforeRemoval()
        {
            m_Name.OnExternalValueUpdate -= new EventHandler<EventArgs>(HandleOnNameUpdate);
            m_Summary.OnExternalValueUpdate -= new EventHandler<EventArgs>(HandleOnSummaryUpdate);

            if (IsLoaded)
            {
                UnloadFromMachine();
            }

            m_HasBeenDeleted = true;
            RaiseOnDeleted();

            m_ConstructorArgs.OnRemoval(Id);
        }

        /// <summary>
        /// Gets a value indicating the ID number of the dataset for 
        /// which the persistence information is stored.
        /// </summary>
        public DatasetId Id
        {
            get
            {
                return m_ConstructorArgs.Id;
            }
        }

        /// <summary>
        /// Gets a value indicating who created the dataset.
        /// </summary>
        public DatasetCreator CreatedBy
        {
            get
            {
                return m_ConstructorArgs.CreatedOnRequestOf;
            }
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
                return m_ConstructorArgs.CanBecomeParent;
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
                return m_ConstructorArgs.CanBeDeleted;
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
                return m_ConstructorArgs.CanBeAdopted;
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
                return m_ConstructorArgs.CanBeCopied;
            }
        }

        /// <summary>
        /// Gets a value indicating where and how the dataset was persisted.
        /// </summary>
        public IPersistenceInformation StoredAt
        {
            get
            {
                return m_ConstructorArgs.LoadFrom;
            }
        }

        /// <summary>
        /// Gets the owner of the dataset.
        /// </summary>
        private IProject Owner
        {
            get
            {
                return m_ConstructorArgs.Owner;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the name of the dataset.
        /// </summary>
        public string Name
        {
            get
            {
                return m_Name.Current;
            }

            set
            {
                if (!string.Equals(m_Name.Current, value))
                {
                    m_Name.Current = value;
                    RaiseOnNameChanged(value);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value describing the dataset.
        /// </summary>
        public string Summary
        {
            get
            {
                return m_Summary.Current;
            }

            set
            {
                if (!string.Equals(m_Summary.Current, value))
                {
                    m_Summary.Current = value;
                    RaiseOnSummaryChanged(value);
                }
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
                return !Owner.IsClosed && !m_HasBeenDeleted;
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
                Lokad.Enforce.With<ArgumentException>(!Owner.IsClosed, Resources.Exceptions_Messages_CannotUseProjectAfterClosingIt);
            }

            // Note that after this action the current object is no longer 'valid'
            // i.e. it can't be used to connect to the owner anymore other than to
            // check validity.
            //
            // Also note that the delete event is only called indirectly. The parent project
            // will handle that because the current dataset may be deleted because its parent
            // is deleted.
            Owner.DeleteDatasetAndChildren(Id);
        }

        /// <summary>
        /// Gets a value indicating whether the dataset is loaded on the local machine
        /// or a remote machine.
        /// </summary>
        public bool IsLoaded
        {
            get
            {
                return m_Connection != null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the dataset can be loaded onto a machine.
        /// </summary>
        public bool CanLoad
        {
            get
            {
                return !m_ConstructorArgs.IsRoot && !m_IsLoading && !IsLoaded;
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

            return m_Connection.RunsOn;
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
        /// only a suggestion. The loader may decide to ignore the suggestion if there is a distribution
        /// plan that is better suited to the contents of the dataset.
        /// </remarks>
        /// <exception cref="ArgumentException">
        ///     Thrown when the project that owns this dataset has been closed.
        /// </exception>
        /// <exception cref="CannotLoadDatasetWithoutLoadingLocationException">
        ///     Thrown when the <paramref name="preferredLocation"/> is <see cref="DistributionLocations.None"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="machineSelector"/> is <see langword="null" />.
        /// </exception>
        public void LoadOntoMachine(
            DistributionLocations preferredLocation,
            Func<IEnumerable<DistributionSuggestion>, SelectedProposal> machineSelector,
            CancellationToken token)
        {
            {
                Lokad.Enforce.With<ArgumentException>(
                    !Owner.IsClosed,
                    Resources.Exceptions_Messages_CannotUseProjectAfterClosingIt);
                Lokad.Enforce.With<CannotLoadDatasetWithoutLoadingLocationException>(
                    preferredLocation != DistributionLocations.None,
                    Resources.Exceptions_Messages_CannotLoadDatasetWithoutLoadingLocation);
                Lokad.Enforce.Argument(() => machineSelector);
            }

            LoadOntoMachine(preferredLocation, machineSelector, token, true);
        }

        private void LoadOntoMachine(
            DistributionLocations preferredLocation,
            Func<IEnumerable<DistributionSuggestion>, SelectedProposal> machineSelector,
            CancellationToken token,
            bool storeLocation)
        {
            {
                Debug.Assert(!Owner.IsClosed, "The owner should not be closed.");
                Debug.Assert(preferredLocation != DistributionLocations.None, "A loading location should be specified.");
            }

            if (IsLoaded)
            {
                return;
            }

            if (m_IsLoading)
            {
                return;
            }

            m_IsLoading = true;
            try
            {
                var request = new DatasetActivationRequest
                {
                    DatasetToActivate = this,
                    PreferredLocations = preferredLocation,
                    ExpectedLoadPerMachine = new ExpectedDatasetLoad
                    {
                        OnDiskSizeInBytes = StoredAt.StoredSizeInBytes(),
                        InMemorySizeInBytes = StoredAt.StoredSizeInBytes(),
                        RelativeMemoryExpansionWhileRunning = 2.0,
                        RelativeOnDiskExpansionAfterRunning = 2.0,
                    },
                };

                var suggestedPlans = m_ConstructorArgs.DistributionPlanGenerator(request, token);
                var selection = from plan in suggestedPlans
                                select new DistributionSuggestion(plan);

                // Ask the user where they would like their dataset loaded.
                var selectedPlan = machineSelector(selection);
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                if (selectedPlan.WasSelectionCanceled)
                {
                    return;
                }

                RaiseOnProgressOfCurrentAction(0, Resources.Progress_LoadingDataset);
                var task = selectedPlan.Plan.Accept(token, RaiseOnProgressOfCurrentAction);
                task.ContinueWith(
                    t =>
                    {
                        if (t.Exception == null)
                        {
                            var online = t.Result;
                            m_Connection = online;
                            m_Connection.OnSwitchToEditMode += HandleOnSwitchToEditMode;
                            m_Connection.OnSwitchToExecutingMode += HandleOnSwitchToExecutingMode;

                            m_DataProxy = m_ProxyBuilder(m_Connection);

                            if (storeLocation)
                            {
                                m_LoadedLocation.Current = selectedPlan.Plan.MachineToDistributeTo;
                            }

                            RaiseOnLoaded();
                        }

                        m_IsLoading = false;
                    },
                    TaskContinuationOptions.ExecuteSynchronously);
            }
            catch (Exception)
            {
                // Only clean this up if the whole thing falls over
                m_IsLoading = false;
                throw;
            }
        }

        private void HandleOnSwitchToEditMode(object sender, EventArgs e)
        {
            RaiseOnSwitchToEditMode();
        }

        private void HandleOnSwitchToExecutingMode(object sender, EventArgs e)
        {
            RaiseOnSwitchToExecutingMode();
        }

        /// <summary>
        /// Unloads the dataset from the machine it is currently loaded onto.
        /// </summary>
        public void UnloadFromMachine()
        {
            UnloadFromMachine(true);
        }

        private void UnloadFromMachine(bool storeUnloadData)
        {
            if (!IsLoaded)
            {
                return;
            }

            m_Connection.OnSwitchToEditMode -= HandleOnSwitchToEditMode;
            m_Connection.OnSwitchToExecutingMode -= HandleOnSwitchToExecutingMode;

            m_Connection.Close();
            m_Connection = null;
            m_DataProxy = null;

            if (storeUnloadData)
            {
                m_LoadedLocation.Current = null;
            }

            RaiseOnUnloaded();
        }

        /// <summary>
        /// Returns the collection of sub-datasets.
        /// </summary>
        /// <returns>
        /// The collection of sub-datasets.
        /// </returns>
        public IEnumerable<IProxyDataset> Children()
        {
            var children = (from dataset in Owner.Children(Id)
                            select dataset).ToList();

            return children;
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
                Lokad.Enforce.With<ArgumentException>(
                    !Owner.IsClosed,
                    Resources.Exceptions_Messages_CannotUseProjectAfterClosingIt);
                Lokad.Enforce.With<DatasetCannotBecomeParentException>(
                    CanBecomeParent,
                    Resources.Exceptions_Messages_DatasetCannotBecomeParent_WithId,
                    Id);
                Lokad.Enforce.Argument(() => newChild);
            }

            return Owner.CreateDataset(Id, newChild);
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
                Lokad.Enforce.With<ArgumentException>(
                    !Owner.IsClosed,
                    Resources.Exceptions_Messages_CannotUseProjectAfterClosingIt);
                Lokad.Enforce.With<DatasetCannotBecomeParentException>(
                    CanBecomeParent,
                    Resources.Exceptions_Messages_DatasetCannotBecomeParent_WithId,
                    Id);
                Lokad.Enforce.Argument(() => newChildren);
                Lokad.Enforce.With<ArgumentException>(
                    newChildren.Any(),
                    Resources.Exceptions_Messages_MissingCreationInformation);
            }

            return (from child in newChildren
                    let newDataset = Owner.CreateDataset(Id, child)
                    select newDataset).ToList();
        }

        /// <summary>
        /// Gets a value indicating whether the dataset is in edit mode or not.
        /// </summary>
        public bool IsEditMode
        {
            get
            {
                return IsLoaded && m_Connection.IsEditMode;
            }
        }

        /// <summary>
        /// Switches the dataset to edit mode.
        /// </summary>
        public void SwitchToEditMode()
        {
            if (!IsLoaded)
            {
                return;
            }

            m_Connection.SwitchToEditMode();
        }

        /// <summary>
        /// Switches the dataset to executing mode.
        /// </summary>
        public void SwitchToExecutingMode()
        {
            if (!IsLoaded)
            {
                return;
            }

            m_Connection.SwitchToExecutingMode();
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
        /// An event raised when there is progress in the loading of the dataset.
        /// </summary>
        public event EventHandler<ProgressEventArgs> OnProgressOfCurrentAction;

        private void RaiseOnProgressOfCurrentAction(int progress, string mark)
        {
            var local = OnProgressOfCurrentAction;
            if (local != null)
            {
                local(this, new ProgressEventArgs(progress, mark));
            }
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
        /// An event fired when the dataset is switched to edit mode.
        /// </summary>
        public event EventHandler<EventArgs> OnSwitchToEditMode;

        private void RaiseOnSwitchToEditMode()
        {
            var local = OnSwitchToEditMode;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// An event fired when the dataset is switched to executing mode.
        /// </summary>
        public event EventHandler<EventArgs> OnSwitchToExecutingMode;

        private void RaiseOnSwitchToExecutingMode()
        {
            var local = OnSwitchToExecutingMode;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets the object that provides access to the data stored in the dataset.
        /// </summary>
        public DatasetStorageProxy Data
        {
            get
            {
                return m_DataProxy;
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

            return other.Id.Equals(Id);
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
            return Id.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Dataset with ID: {0}", Id);
        }
    }
}
