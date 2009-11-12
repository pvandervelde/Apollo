//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core
{
    /// <summary>
    /// Defines the methods for objects that provide bootstrapping capabilities to the
    /// system.
    /// </summary>
    public interface IBootstrapper
    {
        /// <summary>
        /// Loads the Apollo system and starts the kernel.
        /// </summary>
        void Load();

        /// <summary>
        /// Occurs when there is a change in the progress of the system 
        /// startup.
        /// </summary>
        event EventHandler<StartupProgressEventArgs> StartupProgress;
    }
}
