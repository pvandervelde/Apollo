//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using Apollo.Core.Host.Plugins.Definitions;
using Apollo.Core.Host.Properties;
using Apollo.Utilities;

namespace Apollo.Core.Host.Plugins
{
    /// <summary>
    /// Provides the methods to find and scan plugins available in assembly files.
    /// </summary>
    internal sealed class PluginDetector
    {
        /// <summary>
        /// Provides methods to forward log messages across an <c>AppDomain</c> boundary.
        /// </summary>
        private sealed class LogForwardingPipe : MarshalByRefObject, ILogMessagesFromRemoteAppdomains
        {
            /// <summary>
            /// The objects that provides the diagnostics methods for the application.
            /// </summary>
            private readonly SystemDiagnostics m_Diagnostics;

            /// <summary>
            /// Initializes a new instance of the <see cref="LogForwardingPipe"/> class.
            /// </summary>
            /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
            public LogForwardingPipe(SystemDiagnostics diagnostics)
            {
                {
                    Debug.Assert(diagnostics != null, "The diagnostics object should not be null.");
                }

                m_Diagnostics = diagnostics;
            }

            /// <summary>
            /// Logs the given message with the given severity.
            /// </summary>
            /// <param name="severity">The importance of the log message.</param>
            /// <param name="message">The message.</param>
            public void Log(LogSeverityProxy severity, string message)
            {
                m_Diagnostics.Log(severity, message);
            }
        }

        /// <summary>
        /// The object that stores information about all the known plugins.
        /// </summary>
        private readonly IPluginRepository m_Repository;

        /// <summary>
        /// The function that returns all existing files in a given directory and its sub-directories.
        /// </summary>
        private readonly Func<string, IEnumerable<string>> m_FileLocator;

        /// <summary>
        /// The function that returns a new AppDomain that has been initialized with the given
        /// name and base and private paths.
        /// </summary>
        private readonly Func<string, AppDomainPaths, AppDomain> m_AppDomainBuilder;

        /// <summary>
        /// The function that returns a reference to an assembly scanner which has been
        /// created in the given AppDomain.
        /// </summary>
        private readonly Func<AppDomain, ILogMessagesFromRemoteAppdomains, IAssemblyScanner> m_ScannerBuilder;

        /// <summary>
        /// The objects that provides the diagnostics methods for the application.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginDetector"/> class.
        /// </summary>
        /// <param name="repository">The object that stores information about all the known plugins.</param>
        /// <param name="fileLocator">The function that returns all the plugin files in a given directory and its subdirectories.</param>
        /// <param name="appDomainBuilder">The function that returns a new AppDomain that has been initialized with the given name and paths.</param>
        /// <param name="scannerBuilder">The function that is used to create an assembly scanner in the given AppDomain.</param>
        /// <param name="systemDiagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="repository"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="fileLocator"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="appDomainBuilder"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="scannerBuilder"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="systemDiagnostics"/> is <see langword="null" />.
        /// </exception>
        public PluginDetector(
            IPluginRepository repository,
            Func<string, IEnumerable<string>> fileLocator,
            Func<string, AppDomainPaths, AppDomain> appDomainBuilder,
            Func<AppDomain, ILogMessagesFromRemoteAppdomains, IAssemblyScanner> scannerBuilder,
            SystemDiagnostics systemDiagnostics)
        {
            {
                Lokad.Enforce.Argument(() => repository);
                Lokad.Enforce.Argument(() => fileLocator);
                Lokad.Enforce.Argument(() => appDomainBuilder);
                Lokad.Enforce.Argument(() => scannerBuilder);
                Lokad.Enforce.Argument(() => systemDiagnostics);
            }

            m_Repository = repository;
            m_FileLocator = fileLocator;
            m_AppDomainBuilder = appDomainBuilder;
            m_ScannerBuilder = scannerBuilder;
            m_Diagnostics = systemDiagnostics;
        }

        /// <summary>
        /// Searches the given directory for plugins.
        /// </summary>
        /// <param name="directory">The directory to search.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="directory"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">
        ///     Thrown if <paramref name="directory"/> does not exist.
        /// </exception>
        public void SearchDirectory(string directory)
        {
            {
                Lokad.Enforce.Argument(() => directory);
                Lokad.Enforce.With<DirectoryNotFoundException>(
                    Directory.Exists(directory), 
                    Resources.Exceptions_Messages_InvalidPluginDirectoryPath);
            }

            m_Diagnostics.Log(
                LogSeverityProxy.Info,
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.Plugins_LogMessage_Detector_FileScanStarted_WithDirectory,
                    directory));

            IEnumerable<string> files = Enumerable.Empty<string>();
            try
            {
                files = m_FileLocator(directory);
            }
            catch (UnauthorizedAccessException e)
            {
                // Something went wrong with the file IO. That probably means we don't have a complete list
                // so we just exit to prevent any issues from occuring.
                m_Diagnostics.Log(
                    LogSeverityProxy.Error,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Plugins_LogMessage_Detector_FileScanFailed_WithDirectoryAndException,
                        directory,
                        e));

                return;
            }
            catch (IOException e)
            {
                // Something went wrong with the file IO. That probably means we don't have a complete list
                // so we just exit to prevent any issues from occuring.
                m_Diagnostics.Log(
                    LogSeverityProxy.Error,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Plugins_LogMessage_Detector_FileScanFailed_WithDirectoryAndException,
                        directory,
                        e));

                return;
            }
            
            var knownFiles = m_Repository.KnownPluginFiles();

            var changedKnownFiles = knownFiles
                .Where(p => files.Exists(f => string.Equals(p.Path, f, StringComparison.InvariantCultureIgnoreCase)))
                .Where(p => File.GetLastWriteTimeUtc(p.Path) > p.LastWriteTimeUtc)
                .Select(p => p.Path);

            var changedFilePaths = new HashSet<string>(files);
            changedFilePaths.SymmetricExceptWith(knownFiles.Select(p => p.Path));

            var newFiles = changedFilePaths.Where(file => File.Exists(file));
            var deletedFiles = changedFilePaths.Where(file => !File.Exists(file));
            m_Repository.RemovePlugins(deletedFiles);

            IEnumerable<PluginInfo> plugins;
            IEnumerable<SerializedTypeDefinition> types;
            ScanFiles(changedKnownFiles.Concat(newFiles), out plugins, out types);
            m_Repository.Store(plugins, types);

            m_Diagnostics.Log(
                LogSeverityProxy.Info,
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.Plugins_LogMessage_Detector_FileScanCompleted,
                    directory));
        }

        private void ScanFiles(
            IEnumerable<string> filesToScan, 
            out IEnumerable<PluginInfo> plugins, 
            out IEnumerable<SerializedTypeDefinition> types)
        {
            // Create a new AppDomain to use for scanning. We'll drop that later on.
            AppDomain scanDomain = null;
            try
            {
                scanDomain = LoadAppDomain();
                var logger = new LogForwardingPipe(m_Diagnostics);
                var scanner = m_ScannerBuilder(scanDomain, logger);
                Debug.Assert(scanner != null, "Injection of the assembly scanner failed.");

                scanner.Scan(filesToScan, out plugins, out types);
            }
            finally
            {
                if (scanDomain != null)
                {
                    AppDomain.Unload(scanDomain);
                }
            }
        }

        private AppDomain LoadAppDomain()
        {
            return m_AppDomainBuilder(Resources.Plugins_PluginScanDomainName, AppDomainPaths.Core | AppDomainPaths.Plugins);
        }
    }
}
