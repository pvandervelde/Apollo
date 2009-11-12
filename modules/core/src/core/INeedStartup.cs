﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;

namespace Apollo.Core
{
    /// <summary>
    /// Defines the interface for classes that provide startup progress.
    /// </summary>
    public interface INeedStartup
    {
        /// <summary>
        /// The event that is fired when there is an update in the startup process.
        /// </summary>
        event EventHandler<StartupProgressEventArgs> StartupProgress;

        /// <summary>
        /// Starts the process.
        /// </summary>
        void Start();

        /// <summary>
        /// Returns a value indicating what the state of the object is regarding
        /// the startup process.
        /// </summary>
        /// <returns>
        ///   The current startup state for the object.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", 
            Justification="See: Framework Design Guidelines; Section 5.1, page 136.")]
        StartupState GetStartupState();
    }
}