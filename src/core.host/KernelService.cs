﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Apollo.Core.Host.Properties;
using Apollo.Utilities;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;

namespace Apollo.Core.Host
{
    /// <summary>
    /// Defines the base class for services running in the kernel of the Apollo application.
    /// </summary>
    /// <design>
    /// Because this is a MarshalByRef object you shouldn't really use
    /// public properties. You never know where the real object is so even property calls can
    /// take a long time.
    /// </design>
    internal abstract class KernelService : INeedStartup, IHaveServiceDependencies
    {
        /// <summary>
        /// The object that provides the diagnostics methods for the application.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// Stores the current startup state.
        /// </summary>
        private StartupState m_StartupState = StartupState.NotStarted;

        /// <summary>
        /// Initializes a new instance of the <see cref="KernelService"/> class.
        /// </summary>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        protected KernelService(SystemDiagnostics diagnostics)
        {
            {
                Lokad.Enforce.Argument(() => diagnostics);
            }

            m_Diagnostics = diagnostics;
        }

        /// <summary>
        /// Gets the object that provides the diagnostics methods for the application.
        /// </summary>
        protected SystemDiagnostics Diagnostics
        {
            get
            {
                return m_Diagnostics;
            }
        }

        /// <summary>
        /// The event that is fired when there is an update in the startup process.
        /// </summary>
        public event EventHandler<ProgressEventArgs> OnStartupProgress;

        /// <summary>
        /// Fires the <see cref="OnStartupProgress"/> event.
        /// </summary>
        /// <param name="progress">The progress percentage which ranges between 0 and 100.</param>
        /// <param name="currentAction">The current action which is being processed.</param>
        /// <param name="hasErrors">A flag that indicates if there are any errors in the current action.</param>
        protected void RaiseOnStartupProgress(int progress, string currentAction, bool hasErrors)
        {
            EventHandler<ProgressEventArgs> local = OnStartupProgress;
            if (local != null)
            {
                local(this, new ProgressEventArgs(progress, currentAction, hasErrors));
            }
        }

        /// <summary>
        /// Starts the service.
        /// </summary>
        public void Start()
        {
            m_Diagnostics.Log(
                LevelToLog.Trace,
                HostConstants.LogPrefix,
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.Kernel_LogMessage_StartingService_WithType,
                    GetType()));

            m_StartupState = StartupState.Starting;
            try
            {
                StartService();
                m_StartupState = StartupState.Started;

                m_Diagnostics.Log(
                    LevelToLog.Trace,
                    HostConstants.LogPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Kernel_LogMessage_ServiceStarted_WithType,
                        GetType()));
            }
            catch (Exception e)
            {
                m_StartupState = StartupState.Failed;

                m_Diagnostics.Log(
                    LevelToLog.Trace,
                    HostConstants.LogPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Kernel_LogMessage_FailedToStartService_WithTypeAndError,
                        GetType(),
                        e));

                throw;
            }
        }

        /// <summary>
        /// Provides derivative classes with a possibility to
        /// perform startup tasks.
        /// </summary>
        protected virtual void StartService()
        { 
            // Do nothing here. Derrivatives may override and implement this method.
        }

        /// <summary>
        /// Stops the service.
        /// </summary>
        public void Stop()
        {
            m_Diagnostics.Log(
                LevelToLog.Trace,
                HostConstants.LogPrefix,
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.Kernel_LogMessage_StoppingService_WithType,
                    GetType()));

            m_StartupState = StartupState.Stopping;
            try
            {
                StopService();
                m_StartupState = StartupState.Stopped;

                m_Diagnostics.Log(
                    LevelToLog.Trace,
                    HostConstants.LogPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Kernel_LogMessage_ServiceStopped_WithType,
                        GetType()));
            }
            catch (Exception e)
            {
                m_StartupState = StartupState.Failed;

                m_Diagnostics.Log(
                    LevelToLog.Trace,
                    HostConstants.LogPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Kernel_LogMessage_FailedToStopService_WithTypeAndError,
                        GetType(),
                        e));

                throw;
            }
        }

        /// <summary>
        /// Provides derivative classes with a possibility to
        /// perform shutdown tasks.
        /// </summary>
        protected virtual void StopService()
        {
            // Do nothing here. Derrivatives may override and implement this method.
        }

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
                return !IsNotStarted && !IsStarting && !IsStopping && !IsStopped && IsConnectedToAllDependencies;
            }
        }

        /// <summary>
        /// Returns a set of types indicating which services the current service
        /// needs to be linked to in order to be functional.
        /// </summary>
        /// <returns>
        ///     An <see cref="IEnumerable{Type}"/> which contains the types of services
        ///     on which this service depends.
        /// </returns>
        public virtual IEnumerable<Type> ServicesToConnectTo()
        {
            return new Type[0];
        }

        /// <summary>
        /// Provides one of the services on which the current service depends.
        /// </summary>
        /// <param name="dependency">The dependency service.</param>
        public virtual void ConnectTo(KernelService dependency)
        {
            // Do nothing here ...
        }

        /// <summary>
        /// Gets a value indicating whether this instance is connected to all dependencies.
        /// </summary>
        /// <value>
        ///     <see langword="true"/> if this instance is connected to all dependencies; otherwise, <see langword="false"/>.
        /// </value>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
          Justification = "Documentation can start with a language keyword")]
        public virtual bool IsConnectedToAllDependencies
        {
            get
            {
                return true;
            }
        }
    }
}
