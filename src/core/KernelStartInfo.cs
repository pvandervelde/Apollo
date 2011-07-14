//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using Apollo.Utilities;

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
    [ExcludeFromCodeCoverage]
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
            var localPath = assembly.LocalFilePath();
            return new FileInfo(localPath);
        }

        /// <summary>
        /// The collection that holds all the assemblies which are required
        /// for the core to function. These assemblies are the ones that
        /// the fusion loader should be able to find.
        /// </summary>
        private readonly List<FileInfo> m_CoreAssemblies;

        /// <summary>
        /// Initializes a new instance of the <see cref="KernelStartInfo"/> class.
        /// </summary>
        protected KernelStartInfo()
        {
            m_CoreAssemblies = new List<FileInfo>
                {
                    // Apollo.Core.Base
                    DetermineAssemblyPath(typeof(Apollo.Core.Base.ICanClose).Assembly),

                    // Apollo.Core
                    DetermineAssemblyPath(typeof(KernelStartInfo).Assembly),

                    // Apollo.Utilities
                    DetermineAssemblyPath(typeof(ILockObject).Assembly),

                    // Autofac
                    DetermineAssemblyPath(typeof(Autofac.IContainer).Assembly),

                    // Lokad.Shared
                    DetermineAssemblyPath(typeof(Lokad.Enforce).Assembly),

                    // QuickGraph
                    DetermineAssemblyPath(typeof(QuickGraph.GraphExtensions).Assembly),
                };
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
        public abstract IEnumerable<DirectoryInfo> PluginDirectories
        {
            get;
        }
    }
}
