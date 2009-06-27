// Copyright (c) P. van der Velde. All rights reserved.

using System;
using System.Collections.Generic;
using Apollo.Utils;
using System.Diagnostics.Contracts;

namespace Apollo.Core
{
    /// <summary>
    /// Defines the type of a service, either background or foreground.
    /// </summary>
    public enum ServiceType
    {
        // Foreground is the default value for a service type.
        /// <summary>
        /// The service is a foreground service. This means that it will
        /// communicate with other services and actively take part in the
        /// running of the application.
        /// </summary>
        Foreground,
        /// <summary>
        /// The service is a background service. This means that the service
        /// normally only provides data for other services.
        /// </summary>
        Background,
    }

    /// <summary>
    /// Defines the base class for services running in the kernel of the Apollo application.
    /// </summary>
    public abstract class KernelService : MarshalByRefObject, INeedStartup
    {
        // Note: Because this is a MarshalByRef object you shouldn't really use
        // properties. You never know where the real object is so even property calls can
        // take a long time.

        /// <summary>
        /// The object used to lock on.
        /// </summary>
        private readonly ILockObject m_Lock = new LockObject();

        /// <summary>
        /// Stores the current startup state.
        /// </summary>
        private StartupState m_StartupState = StartupState.NotStarted;

        /// <summary>
        /// Initializes a new instance of the <see cref="KernelService"/> class.
        /// </summary>
        protected KernelService()
        { }

        /// <summary>
        /// Gets the type of the service. Currently either a background service
        /// or a foreground service.
        /// </summary>
        /// <value>The type of the service.</value>
        public virtual ServiceType ServicePreferenceType() // <-- Needs renaming.
        {
            return Apollo.Core.ServiceType.Foreground;
        }
        
        /// <summary>
        /// The event that is fired when there is an update in the startup process.
        /// </summary>
        public event EventHandler<StartupProgressEventArgs> StartupProgress;

        /// <summary>
        /// Fires the <see cref="StartupProgress"/> event.
        /// </summary>
        /// <param name="progress">The progress percentage which ranges between 0 and 100.</param>
        /// <param name="currentAction">The current action which is being processed.</param>
        protected void OnStartupProgress(int progress, string currentAction)
        {
            // Argument validation.
            {
                Contract.Requires<ArgumentOutOfRangeException>(progress >= 0);
                Contract.Requires<ArgumentOutOfRangeException>(progress <= 100);
            }

            EventHandler<StartupProgressEventArgs> local = StartupProgress;
            if (local != null)
            { 
                local(this, new StartupProgressEventArgs(progress, currentAction));
            }
        }

        /// <summary>
        /// Starts the startup process.
        /// </summary>
        public void Start()
        {
            // Loads the different parts of the service
            lock(m_Lock)
            {
                m_StartupState = StartupState.Starting; 
            }

            try
            {
                throw new NotImplementedException();
            }
            finally
            {
                // Check the startup state.
                // If there was an error we should set the state to not started
                var finalState = StartupState.Started;
                lock (m_Lock)
                {
                    m_StartupState = finalState;
                }
            }
        }

        /// <summary>
        /// Returns a value indicating what the state of the object is regarding
        /// the startup process.
        /// </summary>
        /// <returns>
        /// The current startup state for the object.
        /// </returns>
        public StartupState GetStartupState()
        { 
            return m_StartupState; 
        }
    }
}