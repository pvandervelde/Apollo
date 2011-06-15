//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Apollo.Core;

namespace Apollo.ProjectExplorer
{
    /// <summary>
    /// Defines the locations of the different system assemblies and directories.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal sealed class BootstrapperStartInfo : KernelStartInfo
    {
        /// <summary>
        /// The collection that holds all the assemblies which are required
        /// for the user interface to function.
        /// </summary>
        private readonly List<FileInfo> m_UserInterfaceAssemblies;

        /// <summary>
        /// The collection that holds all paths for the plug-ins.
        /// </summary>
        private readonly List<DirectoryInfo> m_PlugInDirectories;

        /// <summary>
        /// Initializes a new instance of the <see cref="BootstrapperStartInfo"/> class.
        /// </summary>
        public BootstrapperStartInfo()
            : base()
        {
            m_UserInterfaceAssemblies = new List<FileInfo>
                {
                    // Apollo.ProjectExplorer
                    DetermineAssemblyPath(typeof(BootstrapperStartInfo).Assembly),

                    // @todo: Apollo.Common.Ui

                    // Autofac
                    DetermineAssemblyPath(typeof(Autofac.IContainer).Assembly),

                    // AutofacContrib.CommonServiceLocator

                    // Microsoft.Practises.Composite

                    // Microsoft.Practises.Composite.Presentation
                };

            m_PlugInDirectories = new List<DirectoryInfo>();
        }

        #region Implementation of IKernelStartInfo

        /// <summary>
        /// Gets the collection of assemblies that are used for
        /// the user interface.
        /// </summary>
        /// <value>The user interface assemblies.</value>
        public override IEnumerable<FileInfo> UserInterfaceAssemblies
        {
            get
            {
                return m_UserInterfaceAssemblies;
            }
        }

        /// <summary>
        /// Gets the collection of directories which holds the plugin
        /// assemblies.
        /// </summary>
        /// <value>The plugin directories.</value>
        public override IEnumerable<DirectoryInfo> PluginDirectories
        {
            get
            {
                return m_PlugInDirectories;
            }
        }

        #endregion
    }
}
