﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using Apollo.Core.Host.Plugins.Definitions;

namespace Apollo.Core.Host.Plugins
{
    /// <summary>
    /// Defines the interface for objects that perform scanning of plugin assemblies.
    /// </summary>
    internal interface IAssemblyScanner
    {
        /// <summary>
        /// Scans the assemblies for which the given file paths have been provided and 
        /// returns the plugin description information.
        /// </summary>
        /// <param name="assemblyFilesToScan">
        /// The collection that contains the file paths to all the assemblies to be scanned.
        /// </param>
        /// <param name="plugins">The collection that describes the plugin information in the given assembly files.</param>
        /// <param name="types">
        /// The collection that provides information about all the types which are required to complete the type hierarchy
        /// for the plugin types.
        /// </param>
        void Scan(
            IEnumerable<string> assemblyFilesToScan, 
            out IEnumerable<PluginInfo> plugins, 
            out IEnumerable<SerializedTypeDefinition> types);
    }
}
