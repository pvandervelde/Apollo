//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core
{
    /// <summary>
    /// Defines a private bin path option for <see cref="KernelService"/> classes.
    /// </summary>
    public enum PrivateBinPathOption
    {
        /// <summary>
        /// Indicates that the core set of paths should be provided.
        /// </summary>
        Core,

        /// <summary>
        /// Indicates that the log set of paths should be provided.
        /// </summary>
        Log,
        
        /// <summary>
        /// Indicates that the persistence set of paths should be provided.
        /// </summary>
        Persistence,
        
        /// <summary>
        /// Indicates that the project set of paths should be provided.
        /// </summary>
        Project,
        
        /// <summary>
        /// Indicates that the user interface set of paths should be provided.
        /// </summary>
        UserInterface,
        
        /// <summary>
        /// Indicates that the plugin set of paths should be provided.
        /// </summary>
        PlugIns,
    }
}