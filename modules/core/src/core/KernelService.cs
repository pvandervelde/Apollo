//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Utils;

namespace Apollo.Core
{
    /// <summary>
    /// Defines the base class for services running in the kernel of the Apollo application.
    /// </summary>
    /// <design>
    /// Because this is a MarshalByRef object you shouldn't really use
    /// properties. You never know where the real object is so even property calls can
    /// take a long time.
    /// </design>
    public abstract class KernelService : MarshalByRefObject, INeedStartup
    {
        /// <summary>
        /// Stores the current startup state.
        /// </summary>
        private StartupState m_StartupState = StartupState.NotStarted;

        /// <summary>
        /// The event that is fired when there is an update in the startup process.
        /// </summary>
        public event EventHandler<StartupProgressEventArgs> StartupProgress;

        /// <summary>
        /// Fires the <see cref="StartupProgress"/> event.
        /// </summary>
        /// <param name="progress">The progress percentage which ranges between 0 and 100.</param>
        /// <param name="currentAction">The current action which is being processed.</param>
        protected void OnStartupProgress(int progress, IProgressMark currentAction)
        {
            EventHandler<StartupProgressEventArgs> local = StartupProgress;
            if (local != null)
            { 
                local(this, new StartupProgressEventArgs(progress, currentAction));
            }
        }

        /// <summary>
        /// Starts the service.
        /// </summary>
        public void Start()
        {
            m_StartupState = StartupState.Starting;
            try
            {
                StartService();

                // If we get here then we have started successfully
                m_StartupState = StartupState.Started;
            }
            catch (Exception)
            {
                m_StartupState = StartupState.Failed;

                throw;
            }
        }

        /// <summary>
        /// Provides derivative classes with a possibility to
        /// perform startup tasks.
        /// </summary>
        protected abstract void StartService();

        /// <summary>
        /// Stops the service.
        /// </summary>
        public void Stop()
        {
            m_StartupState = StartupState.Stopping;
            try
            {
                StopService();

                // If we get here then we have started successfully
                m_StartupState = StartupState.Stopped;
            }
            catch (Exception)
            {
                m_StartupState = StartupState.Failed;

                throw;
            }
        }

        /// <summary>
        /// Provides derivative classes with a possibility to
        /// perform shutdown tasks.
        /// </summary>
        protected abstract void StopService();

        /// <summary>
        /// Returns a value indicating what the state of the object is regarding
        /// the startup process.
        /// </summary>
        /// <returns>
        /// The current startup state for the object.
        /// </returns>
        /// <design>
        /// This is a method and not a property because the most of the main objects
        /// in the kernel space will live in their own AppDomain. This means that
        /// access of properties can be slow and could possibly have side effects.
        /// </design>
        public StartupState GetStartupState()
        { 
            return m_StartupState; 
        }
    }
}