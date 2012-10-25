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
        /// The object that stores information about all the known plugins.
        /// </summary>
        private readonly IPluginRepository m_Repository;

        /// <summary>
        /// The function that returns a reference to an assembly scanner which has been
        /// created in the given AppDomain.
        /// </summary>
        private readonly Func<IAssemblyScanner> m_ScannerBuilder;

        /// <summary>
        /// The abstraction layer for the file system.
        /// </summary>
        private readonly IVirtualizeFileSystems m_FileSystem;

        /// <summary>
        /// The objects that provides the diagnostics methods for the application.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginDetector"/> class.
        /// </summary>
        /// <param name="repository">The object that stores information about all the known plugins.</param>
        /// <param name="scannerBuilder">The function that is used to create an assembly scanner.</param>
        /// <param name="fileSystem">The abstraction layer for the file system.</param>
        /// <param name="systemDiagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="repository"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="scannerBuilder"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="fileSystem"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="systemDiagnostics"/> is <see langword="null" />.
        /// </exception>
        public PluginDetector(
            IPluginRepository repository,
            Func<IAssemblyScanner> scannerBuilder,
            IVirtualizeFileSystems fileSystem,
            SystemDiagnostics systemDiagnostics)
        {
            {
                Lokad.Enforce.Argument(() => repository);
                Lokad.Enforce.Argument(() => scannerBuilder);
                Lokad.Enforce.Argument(() => fileSystem);
                Lokad.Enforce.Argument(() => systemDiagnostics);
            }

            m_Repository = repository;
            m_ScannerBuilder = scannerBuilder;
            m_FileSystem = fileSystem;
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
                files = m_FileSystem.GetFilesInDirectory(directory, "*.dll", true);
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
                .Where(p => m_FileSystem.FileLastWriteTimeUtc(p.Path) > p.LastWriteTimeUtc)
                .Select(p => p.Path);

            var changedFilePaths = new HashSet<string>(files);
            changedFilePaths.SymmetricExceptWith(knownFiles.Select(p => p.Path));

            var newFiles = changedFilePaths.Where(file => m_FileSystem.DoesFileExist(file));
            
            RemoveDeletedPlugins(changedFilePaths);
            StorePlugins(changedKnownFiles.Concat(newFiles));

            m_Diagnostics.Log(
                LogSeverityProxy.Info,
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.Plugins_LogMessage_Detector_FileScanCompleted,
                    directory));
        }

        private void RemoveDeletedPlugins(IEnumerable<string> changedFilePaths)
        {
            var deletedFiles = changedFilePaths.Where(file => !m_FileSystem.DoesFileExist(file));
            m_Repository.RemovePlugins(deletedFiles);
        }

        private void StorePlugins(IEnumerable<string> filesToScan)
        {
            if (!filesToScan.Any())
            {
                return;
            }

            var scanner = m_ScannerBuilder();

            IEnumerable<PluginInfo> plugins;
            IEnumerable<SerializedTypeDefinition> types;
            scanner.Scan(filesToScan, out plugins, out types);

            m_Repository.Store(plugins, types);
        }
    }
}
