//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Apollo.Core.Base.Loaders;
using Lokad;
using Utilities.Communication;
using Utilities.Diagnostics;
using Utilities.Diagnostics.Logging;

namespace Apollo.Core.Base
{
    /// <summary>
    /// Stores non-persistent information about a dataset. This includes information
    /// about the communication channels that can be used to communicate with the dataset
    /// and information about the commands that the dataset provides.
    /// </summary>
    public sealed class DatasetOnlineInformation
    {
        /// <summary>
        /// The object that handles sending commands to the remote endpoints.
        /// </summary>
        private readonly ISendCommandsToRemoteEndpoints m_CommandHub;

        /// <summary>
        /// The object that handles the event notifications for remote endpoints.
        /// </summary>
        private readonly INotifyOfRemoteEndpointEvents m_NotificationHub;

        /// <summary>
        /// The object that provides the diagnostics methods for the system.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// Indicates if the dataset is in edit mode or not.
        /// </summary>
        private volatile bool m_IsEditMode;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetOnlineInformation"/> class.
        /// </summary>
        /// <param name="id">The ID number of the dataset.</param>
        /// <param name="endpoint">The ID number of the endpoint that has the actual dataset loaded.</param>
        /// <param name="networkId">The network identifier of the machine on which the dataset runs.</param>
        /// <param name="commandHub">The object that handles sending commands to the remote endpoint.</param>
        /// <param name="notificationHub">The object that handles the event notifications for remote endpoints.</param>
        /// <param name="systemDiagnostics">The object that provides the diagnostics methods for the system.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="id"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="endpoint"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="networkId"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="commandHub"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="notificationHub"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="systemDiagnostics"/> is <see langword="null" />.
        /// </exception>
        public DatasetOnlineInformation(
            DatasetId id, 
            EndpointId endpoint,
            NetworkIdentifier networkId,
            ISendCommandsToRemoteEndpoints commandHub,
            INotifyOfRemoteEndpointEvents notificationHub,
            SystemDiagnostics systemDiagnostics)
        {
            {
                Enforce.Argument(() => id);
                Enforce.Argument(() => endpoint);
                Enforce.Argument(() => networkId);
                Enforce.Argument(() => commandHub);
                Enforce.Argument(() => notificationHub);
                Enforce.Argument(() => systemDiagnostics);
            }

            Id = id;
            Endpoint = endpoint;
            RunsOn = networkId;
            m_CommandHub = commandHub;
            m_Diagnostics = systemDiagnostics;

            m_NotificationHub = notificationHub;
            {
                Debug.Assert(
                    m_NotificationHub.HasNotificationFor(Endpoint, typeof(IDatasetApplicationNotifications)), 
                    "Missing essential notification set.");

                var notifications = m_NotificationHub.NotificationsFor<IDatasetApplicationNotifications>(Endpoint);
                notifications.OnSwitchToEditingMode += 
                    (s, e) => 
                    {
                        m_IsEditMode = true;
                        RaiseOnSwitchToEditMode();
                    };
                notifications.OnSwitchToExecutingMode += 
                    (s, e) => 
                    {
                        m_IsEditMode = false;
                        RaiseOnSwitchToExecutingMode();
                    };
                notifications.OnTimelineUpdate +=
                    (s, e) =>
                    {
                        RaiseOnTimelineUpdate();
                    };
            }
        }

        /// <summary>
        /// Gets the ID number of the dataset for 
        /// which the non-persistence information is stored.
        /// </summary>
        public DatasetId Id
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the ID number of the endpoint which has the actual
        /// dataset loaded.
        /// </summary>
        public EndpointId Endpoint
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the machine on which the dataset is running.
        /// </summary>
        public NetworkIdentifier RunsOn
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether the dataset is in edit mode or not.
        /// </summary>
        public bool IsEditMode
        {
            get
            {
                return m_IsEditMode;
            }
        }

        /// <summary>
        /// Switches the dataset to edit mode.
        /// </summary>
        public void SwitchToEditMode()
        {
            if (!m_IsEditMode)
            {
                Debug.Assert(m_CommandHub.HasCommandFor(Endpoint, typeof(IDatasetApplicationCommands)), "Missing essential command set.");
                var commands = m_CommandHub.CommandsFor<IDatasetApplicationCommands>(Endpoint);
                var result = commands.SwitchToEditMode();
                result.ContinueWith(
                        t =>
                        {
                            if (t.Exception != null)
                            {
                                m_Diagnostics.Log(
                                    LevelToLog.Error,
                                    string.Format(
                                        CultureInfo.InvariantCulture,
                                        "The begin edit dataset task threw an exception. Exception details: {0}",
                                        t.Exception));
                            }
                        });
            }
        }

        /// <summary>
        /// Switches the dataset to executing mode.
        /// </summary>
        public void SwitchToExecutingMode()
        {
            if (m_IsEditMode)
            {
                Debug.Assert(m_CommandHub.HasCommandFor(Endpoint, typeof(IDatasetApplicationCommands)), "Missing essential command set.");
                var commands = m_CommandHub.CommandsFor<IDatasetApplicationCommands>(Endpoint);
                var result = commands.SwitchToExecuteMode();
                result.ContinueWith(
                        t =>
                        {
                            if (t.Exception != null)
                            {
                                m_Diagnostics.Log(
                                    LevelToLog.Error,
                                    string.Format(
                                        CultureInfo.InvariantCulture,
                                        "The begin edit dataset task threw an exception. Exception details: {0}",
                                        t.Exception));
                            }
                        });
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
        /// An event fired when the timeline on the dataset is rolled back or rolled forward.
        /// </summary>
        public event EventHandler<EventArgs> OnTimelineUpdate;

        private void RaiseOnTimelineUpdate()
        {
            var local = OnTimelineUpdate;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Returns a collection containing all the commands that the dataset has
        /// defined.
        /// </summary>
        /// <returns>
        ///     The collection with all the commands that the dataset has defined.
        /// </returns>
        public IEnumerable<ICommandSet> AvailableCommands()
        {
            return from commandType in m_CommandHub.AvailableCommandsFor(Endpoint)
                   where (commandType.GetCustomAttributes(typeof(InternalCommandAttribute), true).Length == 0)
                   select m_CommandHub.CommandsFor(Endpoint, commandType);
        }

        /// <summary>
        /// Returns the command of the given type.
        /// </summary>
        /// <typeparam name="TCommand">The type of the desired command.</typeparam>
        /// <returns>
        ///     The command of the given type or <see langword="null" /> if that command doesn't exist.
        /// </returns>
        public TCommand Command<TCommand>() where TCommand : class, ICommandSet
        {
            return m_CommandHub.CommandsFor<TCommand>(Endpoint);
        }

        /// <summary>
        /// Returns a collection containing all the notifications that a dataset has
        /// defined.
        /// </summary>
        /// <returns>The collection with all the notifications that the dataset has defined.</returns>
        public IEnumerable<INotificationSet> AvailableNotifications()
        {
            return from notificationType in m_NotificationHub.AvailableNotificationsFor(Endpoint)
                   where (notificationType.GetCustomAttributes(typeof(InternalNotificationAttribute), true).Length == 0)
                   select m_NotificationHub.NotificationsFor(Endpoint, notificationType);
        }

        /// <summary>
        /// Returns the notification of the given type.
        /// </summary>
        /// <typeparam name="TNotification">The type of the desired notification.</typeparam>
        /// <returns>The notification of the given type or <see langword="null" /> if that notification doesn't exist.</returns>
        public TNotification Notification<TNotification>() where TNotification : class, INotificationSet
        {
            return m_NotificationHub.NotificationsFor<TNotification>(Endpoint);
        }

        /// <summary>
        /// Closes the remote dataset application.
        /// </summary>
        public void Close()
        {
            Debug.Assert(m_CommandHub.HasCommandFor(Endpoint, typeof(IDatasetApplicationCommands)), "Missing essential command set.");
            var commands = m_CommandHub.CommandsFor<IDatasetApplicationCommands>(Endpoint);
            var result = commands.Close();
            result.ContinueWith(
                t =>
                {
                    if (t.Exception != null)
                    {
                        m_Diagnostics.Log(
                            LevelToLog.Error,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "The close dataset task threw an exception. Exception details: {0}",
                                t.Exception));
                    }
                },
                TaskContinuationOptions.None);
        }
    }
}
