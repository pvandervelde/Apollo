//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;

namespace Apollo.Core
{
    /// <summary>
    /// Stores information used by the starting process of the kernel.
    /// </summary>
    /// <design>
    /// The base assembly paths are hard-coded in the configuration file.
    /// The assembly paths for the plug-ins are passed on by the user
    /// interface.
    /// </design>
    public interface IKernelStartInfo
    {
        /// <summary>
        /// Gets the collection of assemblies that make up the core of the
        /// system.
        /// </summary>
        /// <value>The core assemblies.</value>
        IEnumerable<FileInfo> CoreAssemblies { get; }

        /// <summary>
        /// Gets the collection of assemblies that are used for logging
        /// purposes.
        /// </summary>
        /// <value>The log assemblies.</value>
        IEnumerable<FileInfo> LogAssemblies { get; }

        /// <summary>
        /// Gets the collection of assemblies that are used for the
        /// persistence.
        /// </summary>
        /// <value>The persistence assemblies.</value>
        IEnumerable<FileInfo> PersistenceAssemblies { get; }

        /// <summary>
        /// Gets the collection of assemblies that are used for the
        /// project system.
        /// </summary>
        /// <value>The project assemblies.</value>
        IEnumerable<FileInfo> ProjectAssemblies { get; }

        /// <summary>
        /// Gets the collection of assemblies that are used for
        /// the user interface.
        /// </summary>
        /// <value>The user interface assemblies.</value>
        IEnumerable<FileInfo> UserInterfaceAssemblies { get; }

        /// <summary>
        /// Gets the collection of directories which holds the plugin
        /// assemblies.
        /// </summary>
        /// <value>The plugin directories.</value>
        IEnumerable<DirectoryInfo> PluginDirectories { get; }
    }
}
