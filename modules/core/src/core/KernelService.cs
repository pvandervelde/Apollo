﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Utils;

namespace Apollo.Core
{
    /// <summary>
    /// Defines the base class for services running in the kernel of the Apollo application.
    /// </summary>
    /// <design>
    /// Because this is a MarshalByRef object you shouldn't really use
    /// public properties. You never know where the real object is so even property calls can
    /// take a long time.
    /// </design>
    internal abstract class KernelService : INeedStartup
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
        /// Gets a value indicating what the state of the object is regarding
        /// the startup process.
        /// </summary>
        public StartupState StartupState
        {
            get
            {
                return m_StartupState;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this service is not started.
        /// </summary>
        /// <value>
        ///     <see langword="true"/> if this service is not started; otherwise, <see langword="false"/>.
        /// </value>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        protected bool IsNotStarted
        {
            get
            {
                return m_StartupState == StartupState.NotStarted;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this service is starting.
        /// </summary>
        /// <value>
        ///     <see langword="true"/> if this service is starting; otherwise, <see langword="false"/>.
        /// </value>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        protected bool IsStarting
        {
            get
            {
                return m_StartupState == StartupState.Starting;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this service is stopped.
        /// </summary>
        /// <value>
        ///     <see langword="true"/> if this service is stopped; otherwise, <see langword="false"/>.
        /// </value>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        protected bool IsStopped
        {
            get
            {
                return m_StartupState == StartupState.Stopped;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this service is stopping.
        /// </summary>
        /// <value>
        ///     <see langword="true"/> if this service is stopping; otherwise, <see langword="false"/>.
        /// </value>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        protected bool IsStopping
        {
            get
            {
                return m_StartupState == StartupState.Stopping;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this service is capable of performing all its functions. This
        /// is the case if the service is not starting or stopping.
        /// </summary>
        /// <value>
        ///     <see langword="true"/> if this service is capable of performing all its functions; otherwise, <see langword="false"/>.
        /// </value>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        protected virtual bool IsFullyFunctional
        {
            get
            {
                return !IsNotStarted && !IsStarting && !IsStopping && !IsStopped;
            }
        }
    }
}