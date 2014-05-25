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
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using Apollo.Core.Base;
using Apollo.Core.Base.Activation;
using Apollo.Core.Host.Properties;
using Apollo.Utilities;
using Apollo.Utilities.History;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;

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
        /// The history index of the distribution location field.
        /// </summary>
        private const byte DistributionLocationIndex = 2;

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
                Debug.Assert(constructorArguments.Length == 4, "There should be four constructor arguments.");
            }

            IVariableTimeline<string> name = null;
            IVariableTimeline<string> summary = null;
            IVariableTimeline<NetworkIdentifier> distributionLocation = null;
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

                if (member.Item1 == DistributionLocationIndex)
                {
                    distributionLocation = member.Item2 as IVariableTimeline<NetworkIdentifier>;
                    continue;
                }

                throw new UnknownMemberException();
            }

            return new DatasetProxy(
                constructorArguments[0] as DatasetConstructionParameters,
                constructorArguments[1] as Func<DatasetOnlineInformation, DatasetStorageProxy>,
                constructorArguments[2] as ICollectNotifications,
                constructorArguments[3] as SystemDiagnostics,
                historyId,
                name,
                summary,
                distributionLocation);
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
        /// The object that collects the notifications for the user interface.
        /// </summary>
        private readonly ICollectNotifications m_Notifications;

        /// <summary>
        /// The object that provides the diagnostics methods for the application.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

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
        /// The information that describes if the dataset is activated at a given point on the
        /// timeline.
        /// </summary>
        [FieldIndexForHistoryTracking(DistributionLocationIndex)]
        private readonly IVariableTimeline<NetworkIdentifier> m_DistributionLocation;

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
        private volatile bool m_IsActivating;

        /// <summary>
        /// Indicates if the dataset has been deleted.
        /// </summary>
        private volatile bool m_HasBeenDeleted;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetProxy"/> class.
        /// </summary>
        /// <param name="constructorArgs">The object that holds all the constructor arguments that do not belong to the timeline.</param>
        /// <param name="proxyBuilder">The function that is used to create the proxy for the data inside the dataset.</param>
        /// <param name="notifications">The object that stores the notifications for the user interface.</param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <param name="historyId">The ID for use in the history system.</param>
        /// <param name="name">The data structure that stores the history information for the name of the dataset.</param>
        /// <param name="summary">The data structure that stores the history information for the summary of the dataset.</param>
        /// <param name="distributionLocation">
        /// The data structure that stores the history information indicating if the dataset is activated at any point.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="constructorArgs"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="proxyBuilder"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="notifications"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="diagnostics"/> is <see langword="null" />.
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
        ///     Thrown if <paramref name="distributionLocation"/> is <see langword="null" />.
        /// </exception>
        private DatasetProxy(
            DatasetConstructionParameters constructorArgs,
            Func<DatasetOnlineInformation, DatasetStorageProxy> proxyBuilder,
            ICollectNotifications notifications,
            SystemDiagnostics diagnostics,
            HistoryId historyId,
            IVariableTimeline<string> name,
            IVariableTimeline<string> summary,
            IVariableTimeline<NetworkIdentifier> distributionLocation)
        {
            {
                Lokad.Enforce.Argument(() => constructorArgs);
                Lokad.Enforce.Argument(() => proxyBuilder);
                Lokad.Enforce.Argument(() => notifications);
                Lokad.Enforce.Argument(() => diagnostics);
                Lokad.Enforce.Argument(() => historyId);
                Lokad.Enforce.Argument(() => name);
                Lokad.Enforce.Argument(() => summary);
                Lokad.Enforce.Argument(() => distributionLocation);
            }

            m_ConstructorArgs = constructorArgs;
            m_ProxyBuilder = proxyBuilder;
            m_Notifications = notifications;
            m_Diagnostics = diagnostics;
            m_HistoryId = historyId;
            m_Name = name;
            m_Summary = summary;
            m_DistributionLocation = distributionLocation;

            m_Name.OnExternalValueUpdate += HandleOnNameUpdate;
            m_Summary.OnExternalValueUpdate += HandleOnSummaryUpdate;
            m_DistributionLocation.OnExternalValueUpdate += HandleOnIsActivatedUpdate;
        }

        private void HandleOnNameUpdate(object sender, EventArgs e)
        {
            RaiseOnNameChanged(Name);
        }

        private void HandleOnSummaryUpdate(object sender, EventArgs e)
        {
            RaiseOnSummaryChanged(Summary);
        }

        private void HandleOnIsActivatedUpdate(object sender, EventArgs e)
        {
            // See if the restored status still matches up with the online status of the dataset
            // If not make sure that it does
            if (m_DistributionLocation.Current != null)
            {
                if (m_Connection == null)
                {
                    // For now we go with the naive approach. Distribute onto the same machine as last time
                    // and if we can't find it then grab the first machine in the selection.
                    // At some point we should make this a whole lot smarter but for now it'll do.
                    var original = m_DistributionLocation.Current;

                    m_Diagnostics.Log(
                        LevelToLog.Trace, 
                        HostConstants.LogPrefix,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.DatasetProxy_LogMessage_ReactivatingFromPreviouslyStoredLocation_WithLocation,
                            Id,
                            original));

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

                    var source = new CancellationTokenSource();
                    var activationTask = Activate(
                        DistributionLocations.All,
                        selector,
                        m_Notifications,
                        source.Token,
                        false);
                    activationTask.ContinueWith(t => source.Dispose());
                }
            }
            else
            {
                if (m_Connection != null)
                {
                    Deactivate(false);
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
            m_Name.OnExternalValueUpdate -= HandleOnNameUpdate;
            m_Summary.OnExternalValueUpdate -= HandleOnSummaryUpdate;

            if (IsActivated)
            {
                Deactivate();
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
        public bool IsActivated
        {
            get
            {
                return m_Connection != null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the dataset can be loaded onto a machine.
        /// </summary>
        public bool CanActivate
        {
            get
            {
                return !m_ConstructorArgs.IsRoot && !m_IsActivating && !IsActivated;
            }
        }

        /// <summary>
        /// Returns the machine on which the dataset is running.
        /// </summary>
        /// <returns>
        /// The machine on which the dataset is running.
        /// </returns>
        /// <exception cref="DatasetNotActivatedException">
        ///     Thrown when the dataset is not loaded onto a machine.
        /// </exception>
        public NetworkIdentifier RunsOn()
        {
            if (!IsActivated)
            {
                throw new DatasetNotActivatedException();
            }

            return m_Connection.RunsOn;
        }

        /// <summary>
        /// Activates the dataset in an asynchronous manner.
        /// </summary>
        /// <param name="preferredLocation">
        /// Indicates a preferred machine location for the dataset to be distributed to.
        /// </param>
        /// <param name="machineSelector">
        ///     The function that selects the most suitable machine for the dataset to run on.
        /// </param>
        /// <param name="token">The token that is used to cancel the activation.</param>
        /// <remarks>
        /// Note that the <paramref name="preferredLocation"/> is
        /// only a suggestion. The activator may decide to ignore the suggestion if there is a distribution
        /// plan that is better suited to the contents of the dataset.
        /// </remarks>
        /// <returns>
        ///     A <see cref="Task"/> that completes when the dataset is activated.
        /// </returns>
        /// <exception cref="ArgumentException">
        ///     Thrown when the project that owns this dataset has been closed.
        /// </exception>
        /// <exception cref="CannotActivateDatasetWithoutDistributionLocationException">
        ///     Thrown when the <paramref name="preferredLocation"/> is <see cref="DistributionLocations.None"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="machineSelector"/> is <see langword="null" />.
        /// </exception>
        public Task Activate(
            DistributionLocations preferredLocation,
            Func<IEnumerable<DistributionSuggestion>, SelectedProposal> machineSelector,
            CancellationToken token)
        {
            {
                Lokad.Enforce.With<ArgumentException>(
                    !Owner.IsClosed,
                    Resources.Exceptions_Messages_CannotUseProjectAfterClosingIt);
                Lokad.Enforce.With<CannotActivateDatasetWithoutDistributionLocationException>(
                    preferredLocation != DistributionLocations.None,
                    Resources.Exceptions_Messages_CannotActivateDatasetWithoutDistributionLocation);
                Lokad.Enforce.Argument(() => machineSelector);
            }

            return Activate(preferredLocation, machineSelector, m_Notifications, token, true);
        }

        private Task Activate(
            DistributionLocations preferredLocation,
            Func<IEnumerable<DistributionSuggestion>, SelectedProposal> machineSelector,
            ICollectNotifications notifications,
            CancellationToken token,
            bool storeLocation)
        {
            {
                Debug.Assert(!Owner.IsClosed, "The owner should not be closed.");
                Debug.Assert(preferredLocation != DistributionLocations.None, "A distribution location should be specified.");
            }

            if (IsActivated)
            {
                return Task.Factory.StartNew(
                    () => { },
                    token,
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler());
            }

            if (m_IsActivating)
            {
                var onActivated = Observable.FromEventPattern<EventArgs>(
                    h => OnActivated += h,
                    h => OnActivated -= h)
                    .Take(1);
                var onDeactivated = Observable.FromEventPattern<EventArgs>(
                    h => OnDeactivated += h,
                    h => OnDeactivated -= h)
                    .Take(1);

                return Observable.Amb(onActivated, onDeactivated)
                    .ToTask(token);
            }

            m_Diagnostics.Log(
                LevelToLog.Trace, 
                HostConstants.LogPrefix,
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.DatasetProxy_LogMessage_ActivatingDataset_WithId,
                    Id));

            m_IsActivating = true;
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
                var selection = suggestedPlans.Select(plan => new DistributionSuggestion(plan));
                var selectedPlan = machineSelector(selection);
                if (token.IsCancellationRequested || selectedPlan.WasSelectionCanceled)
                {
                    return Task.Factory.StartNew(
                        () => { },
                        token,
                        TaskCreationOptions.None,
                        new CurrentThreadTaskScheduler());
                }

                RaiseOnProgressOfCurrentAction(0, Resources.Progress_ActivatingDataset, false);
                var task = selectedPlan.Plan.Accept(token, RaiseOnProgressOfCurrentAction);
                var continuationTask = task.ContinueWith(
                    t =>
                    {
                        try
                        {
                            if (t.Exception != null)
                            {
                                m_Diagnostics.Log(
                                    LevelToLog.Error, 
                                    HostConstants.LogPrefix,
                                    string.Format(
                                        CultureInfo.InvariantCulture,
                                        Resources.DatasetProxy_LogMessage_FailedToActivateDataset_WithException,
                                        Id,
                                        t.Exception));

                                // Obviously not activated so ...
                                RaiseOnProgressOfCurrentAction(100, string.Empty, false);
                                RaiseOnDeactivated();

                                notifications.StoreNotification(Resources.Notifications_FailedToActivateDataset);
                                return;
                            }

                            var online = t.Result;
                            m_Connection = online;
                            m_Connection.OnSwitchToEditMode += HandleOnSwitchToEditMode;
                            m_Connection.OnSwitchToExecutingMode += HandleOnSwitchToExecutingMode;

                            m_DataProxy = m_ProxyBuilder(m_Connection);

                            if (storeLocation)
                            {
                                m_DistributionLocation.Current = selectedPlan.Plan.MachineToDistributeTo;
                            }

                            m_Diagnostics.Log(
                                LevelToLog.Trace, 
                                HostConstants.LogPrefix,
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    Resources.DatasetProxy_LogMessage_DatasetActivationComplete_WithId,
                                    Id));

                            RaiseOnActivated();
                        }
                        finally
                        {
                            m_IsActivating = false;
                        }
                    },
                    TaskContinuationOptions.ExecuteSynchronously);

                return continuationTask;
            }
            catch (Exception)
            {
                // Only clean this up if the whole thing falls over
                m_IsActivating = false;
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
        /// <returns>
        ///     A <see cref="Task"/> that completes when the dataset is deactivated.
        /// </returns>
        public Task Deactivate()
        {
            return Task.Factory.StartNew(() => Deactivate(true));
        }

        private void Deactivate(bool storeUnloadData)
        {
            if (!IsActivated)
            {
                return;
            }

            m_Diagnostics.Log(
                LevelToLog.Trace, 
                HostConstants.LogPrefix,
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.DatasetProxy_LogMessage_DeactivatingDataset_WithId,
                    Id));

            m_Connection.OnSwitchToEditMode -= HandleOnSwitchToEditMode;
            m_Connection.OnSwitchToExecutingMode -= HandleOnSwitchToExecutingMode;

            m_Connection.Close();
            m_Connection = null;
            m_DataProxy = null;

            if (storeUnloadData)
            {
                m_DistributionLocation.Current = null;
            }

            RaiseOnDeactivated();
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
                return IsActivated && m_Connection.IsEditMode;
            }
        }

        /// <summary>
        /// Switches the dataset to edit mode.
        /// </summary>
        public void SwitchToEditMode()
        {
            if (!IsActivated)
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
            if (!IsActivated)
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
        /// An event raised when there is progress in the current action which is being
        /// executed by the dataset.
        /// </summary>
        public event EventHandler<ProgressEventArgs> OnProgressOfCurrentAction;

        private void RaiseOnProgressOfCurrentAction(int progress, string mark, bool hasErrors)
        {
            var local = OnProgressOfCurrentAction;
            if (local != null)
            {
                local(this, new ProgressEventArgs(progress, mark, hasErrors));
            }
        }

        /// <summary>
        /// An event raised when the dataset is activated.
        /// </summary>
        public event EventHandler<EventArgs> OnActivated;

        private void RaiseOnActivated()
        {
            var local = OnActivated;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// An event raised when the dataset is deactivated.
        /// </summary>
        public event EventHandler<EventArgs> OnDeactivated;

        private void RaiseOnDeactivated()
        {
            var local = OnDeactivated;
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
