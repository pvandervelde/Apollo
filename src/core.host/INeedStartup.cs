﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Utilities;

namespace Apollo.Core.Host
{
    /// <summary>
    /// Defines the interface for classes that provide startup progress.
    /// </summary>
    public interface INeedStartup
    {
        /// <summary>
        /// The event that is fired when there is an update in the startup process.
        /// </summary>
        event EventHandler<ProgressEventArgs> OnStartupProgress;

        /// <summary>
        /// Starts the process.
        /// </summary>
        void Start();

        /// <summary>
        /// Gets a value indicating what the state of the object is regarding
        /// the startup process.
        /// </summary>
        StartupState StartupState
        {
            get;
        }
    }
}
