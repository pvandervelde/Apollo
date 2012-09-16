//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

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
        /// <returns>The collection that describes the plugin information in the given assembly files.</returns>
        IEnumerable<PluginInfo> Scan(IEnumerable<string> assemblyFilesToScan);
    }
}
