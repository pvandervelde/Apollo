//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Apollo.Service.Repository.Plugins;
using Apollo.Service.Repository.Properties;
using Autofac;
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;

namespace Apollo.Service.Repository
{
    internal sealed class ServiceEntryPoint : IDisposable
    {
        /// <summary>
        /// The object used to lock on.
        /// </summary>
        private readonly object m_Lock = new object();

        /// <summary>
        /// The IOC container for the service.
        /// </summary>
        private IContainer m_Container;

        /// <summary>
        /// The object that provides the diagnostics methods for the application.
        /// </summary>
        private SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// The task that is used to handle plugin discovery.
        /// </summary>
        private Task m_PluginDiscoveryTask;

        /// <summary>
        /// A flag that indicates that the application has been stopped.
        /// </summary>
        private volatile bool m_HasBeenStopped;

        /// <summary>
        /// A flag that indicates if the service has been disposed or not.
        /// </summary>
        private volatile bool m_IsDisposed;

        /// <summary>
        /// When implemented in a derived class, executes when a Start command is sent
        /// to the service by the Service Control Manager (SCM) or when the operating
        /// system starts (for a service that starts automatically). Specifies actions
        /// to take when the service starts.
        /// </summary>
        public void OnStart()
        {
            lock(m_Lock)
            {
                m_Container = DependencyInjection.CreateContainer();
                m_Diagnostics = m_Container.Resolve<SystemDiagnostics>();

                m_PluginDiscoveryTask = Task.Factory.StartNew(DiscoverPlugins);
            }

            // Start the communication layer

            // Start the package repository stuff
        }

        private void DiscoverPlugins()
        {
            var configuration = m_Container.Resolve<IConfiguration>();
            if (!configuration.HasValueFor(RepositoryServiceConfigurationKeys.PluginLocation))
            {
                return;
            }

            var pluginDirectories = configuration.Value<List<string>>(RepositoryServiceConfigurationKeys.PluginLocation);

            var detector = m_Container.Resolve<PluginDetector>();
            foreach (var dir in pluginDirectories)
            {
                detector.SearchDirectory(dir);
            }
        }

        /// <summary>
        /// When implemented in a derived class, executes when a Stop command is sent
        /// to the service by the Service Control Manager (SCM). Specifies actions to
        /// take when a service stops running.
        /// </summary>
        public void OnStop()
        {
            bool hasBeenStopped;
            lock (m_Lock)
            {
                hasBeenStopped = m_HasBeenStopped;
            }

            if (hasBeenStopped)
            {
                return;
            }

            try
            {
                lock(m_Lock)
                {
                    if (!m_PluginDiscoveryTask.IsCompleted)
                    {
                        // Cancel the task here ..
                    }

                    m_Diagnostics.Log(
                        LevelToLog.Info,
                        RepositoryServiceConstants.LogPrefix,
                        Resources.Log_Messages_ServiceStopped);

                    if (m_Container != null)
                    {
                        m_Container.Dispose();
                        m_Container = null;
                    }
                }
            }
            finally
            {
                lock (m_Lock)
                {
                    m_HasBeenStopped = true;
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (!m_IsDisposed)
            {
                OnStop();

                m_IsDisposed = true;
            }
        }
    }
}
