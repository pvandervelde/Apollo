//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Apollo.Core
{
    /// <summary>
    /// Stores information used by the starting process of the kernel.
    /// </summary>
    /// <design>
    /// All collections should define all the assemblies required for a specific
    /// function, even if that means there are duplicate assemblies or paths. 
    /// The reason is that there is no guarantuee that other collections
    /// will be used to provide assembly references.
    /// </design>
    public abstract class KernelStartInfo
    {
        /// <summary>
        /// Determines the assembly path.
        /// </summary>
        /// <param name="assembly">The assembly for which the path needs to be determined.</param>
        /// <returns>
        /// The file path of the assembly.
        /// </returns>
        protected static FileInfo DetermineAssemblyPath(Assembly assembly)
        {
            // Get the location of the assembly before it was shadow-copied
            // Note that Assembly.Codebase gets the path to the manifest-containing
            // file, not necessarily the path to the file that contains a
            // specific type.
            var uncPath = assembly.CodeBase;

            // Get the local path. This may not work if the assembly isn't
            // local. For now we assume it is.
            var localPath = new Uri(uncPath).LocalPath;
            return new FileInfo(localPath);
        }

        /// <summary>
        /// The collection that holds all the assemblies which are required
        /// for the core to function.
        /// </summary>
        private readonly List<FileInfo> m_CoreAssemblies;

        /// <summary>
        /// The collection that holds all the assemblies which are required
        /// for the log capabilities to function.
        /// </summary>
        private readonly List<FileInfo> m_LogAssemblies;

        /// <summary>
        /// The collection that holds all the assemblies which are required
        /// for the persistence capabilities to function.
        /// </summary>
        private readonly List<FileInfo> m_PersistenceAssemblies;

        /// <summary>
        /// The collection that holds all the assemblies which are required
        /// for the project capabilities to function.
        /// </summary>
        private readonly List<FileInfo> m_ProjectAssemblies;

        /// <summary>
        /// Initializes a new instance of the <see cref="KernelStartInfo"/> class.
        /// </summary>
        protected KernelStartInfo()
        {
            m_CoreAssemblies = new List<FileInfo>
                {
                    // Apollo.Core
                    DetermineAssemblyPath(typeof(KernelStartInfo).Assembly),

                    // Apollo.Utils
                    DetermineAssemblyPath(typeof(Utils.ILockObject).Assembly),

                    // Autofac
                    DetermineAssemblyPath(typeof(Autofac.IContainer).Assembly),

                    // Lokad.Shared
                    DetermineAssemblyPath(typeof(Lokad.Enforce).Assembly),

                    // QuickGraph
                    DetermineAssemblyPath(typeof(QuickGraph.GraphExtensions).Assembly),

                    // System.CoreEx
                    DetermineAssemblyPath(typeof(Property).Assembly),

                    // System.Threading
                    DetermineAssemblyPath(typeof(System.Threading.Tasks.Task).Assembly),
                };

            m_LogAssemblies = new List<FileInfo>();

            m_PersistenceAssemblies = new List<FileInfo>();

            m_ProjectAssemblies = new List<FileInfo>();
        }

        /// <summary>
        /// Gets the collection of assemblies that make up the core of the
        /// system.
        /// </summary>
        /// <value>The core assemblies.</value>
        public IEnumerable<FileInfo> CoreAssemblies
        {
            get
            {
                return m_CoreAssemblies.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets the collection of assemblies that are used for logging
        /// purposes.
        /// </summary>
        /// <value>The log assemblies.</value>
        public IEnumerable<FileInfo> LogAssemblies
        {
            get
            {
                return m_LogAssemblies.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets the collection of assemblies that are used for the
        /// persistence.
        /// </summary>
        /// <value>The persistence assemblies.</value>
        public IEnumerable<FileInfo> PersistenceAssemblies
        {
            get
            {
                return m_PersistenceAssemblies.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets the collection of assemblies that are used for the
        /// project system.
        /// </summary>
        /// <value>The project assemblies.</value>
        public IEnumerable<FileInfo> ProjectAssemblies
        {
            get
            {
                return m_ProjectAssemblies.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets the collection of assemblies that are used for
        /// the user interface.
        /// </summary>
        /// <value>The user interface assemblies.</value>
        public abstract IEnumerable<FileInfo> UserInterfaceAssemblies
        {
            get;
        }

        /// <summary>
        /// Gets the collection of directories which holds the plugin
        /// assemblies.
        /// </summary>
        /// <value>The plugin directories.</value>
        public abstract IEnumerable<DirectoryInfo> PlugInDirectories
        {
            get;
        }
    }
}
