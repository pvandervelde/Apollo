//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Threading.Tasks;
using Apollo.Core.Base;
using Apollo.Core.Base.Communication;
using Apollo.Utilities;
using Apollo.Utilities.Configuration;

namespace Apollo.Core.Host
{
    /// <summary>
    /// Defines the commands that allow a host application to handle the requests that arrive from the dataset applications.
    /// </summary>
    internal sealed class HostApplicationCommands : IHostApplicationCommands
    {
        /// <summary>
        /// The object that provides a virtualizing layer for the file system.
        /// </summary>
        /// <remarks>
        /// This object is mostly used to that it is easier to test the current class. Note that we can't
        /// completely virtualize the file system because we're trying to load assemblies.
        /// </remarks>
        private readonly IVirtualizeFileSystems m_FileSystem;

        /// <summary>
        /// The object that stores references to all the files that are about to be uploaded or
        /// are currently being uploaded.
        /// </summary>
        private readonly IStoreUploads m_UploadCollection;

        /// <summary>
        /// The object that stores the configuration for the current application.
        /// </summary>
        private readonly IConfiguration m_Configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="HostApplicationCommands"/> class.
        /// </summary>
        /// <param name="fileSystem">The object that provides a virtualizing layer for the file system.</param>
        /// <param name="uploads">
        /// The object that stores references to all the files that are about to be uploaded or
        /// are currently being uploaded.
        /// </param>
        /// <param name="configuration">The object that stores the configuration for the current application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="fileSystem"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="uploads"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="configuration"/> is <see langword="null" />.
        /// </exception>
        public HostApplicationCommands(IVirtualizeFileSystems fileSystem, IStoreUploads uploads, IConfiguration configuration)
        {
            {
                Lokad.Enforce.Argument(() => fileSystem);
                Lokad.Enforce.Argument(() => uploads);
                Lokad.Enforce.Argument(() => configuration);
            }

            m_FileSystem = fileSystem;
            m_UploadCollection = uploads;
            m_Configuration = configuration;
        }

        /// <summary>
        /// Finds the plug-in container (i.e. the assembly) that matches the given name and prepares it for uploading to 
        /// a dataset application.
        /// </summary>
        /// <param name="name">The name of the assembly that contains the plug-in.</param>
        /// <returns>A task that will finish once the assembly is queued and ready for upload.</returns>
        public Task<UploadToken> PreparePluginContainerForTransfer(AssemblyName name)
        {
            return Task<UploadToken>.Factory.StartNew(
                () =>
                {
                    // @Todo: Note that these paths are defined in Apollo.ProjectExplorer.Utilities.UtilitiesModule
                    if (!m_Configuration.HasValueFor(CoreConfigurationKeys.PluginLocation))
                    {
                        throw new FileNotFoundException();
                    }

                    var pluginDirectories = m_Configuration.Value<List<string>>(CoreConfigurationKeys.PluginLocation);
                    var assemblyFilePath = FindAssembly(name, pluginDirectories);

                    if (assemblyFilePath == null)
                    {
                        throw new FileNotFoundException();
                    }

                    return m_UploadCollection.Register(assemblyFilePath.FullName);
                });
        }

        private FileInfo FindAssembly(AssemblyName name, IEnumerable<string> directories)
        {
            var files = new List<string>();
            foreach (var dir in directories)
            {
                files.AddRange(m_FileSystem.GetFilesInDirectory(dir, "*.dll", true));
            }

            // Store all the assemblies we can find that match the name, culture and public key. If a public key is specified
            // then there should be at most 1 of those assemblies because it should be an exact match. If however no public key
            // is specified then potentially any assembly with an equal or greater version number could be loaded.
            IDictionary<AssemblyName, string> assemblyNameToFileMap = new Dictionary<AssemblyName, string>();
            foreach (var filePath in files)
            {
                AssemblyName assemblyName = null;
                try
                {
                    assemblyName = AssemblyName.GetAssemblyName(filePath);
                }
                catch (ArgumentException)
                {
                    // Not a valid assembly name
                }
                catch (SecurityException)
                {
                    // No access
                }
                catch (BadImageFormatException)
                {
                    // Not a valid assembly
                }
                catch (FileLoadException)
                {
                    // Couldn't load file
                }

                if (assemblyName != null)
                {
                    if (name.IsMatch(assemblyName))
                    {
                        assemblyNameToFileMap.Add(assemblyName, filePath);
                    }
                }
            }

            var file = assemblyNameToFileMap.OrderByDescending(p => p.Key.Version).Select(p => p.Value).FirstOrDefault();
            return (!string.IsNullOrWhiteSpace(file)) ? new FileInfo(file) : null;
        }
    }
}
