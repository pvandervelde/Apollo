// Copyright (c) P. van der Velde. All rights reserved.

using System;

namespace Apollo.Core
{
    /// <summary>
    /// Indicates the startup state for the <see cref="INeedStartup"/> object.
    /// </summary>
    public enum StartupState
    {
        /// <summary>
        /// The state is unknown.
        /// </summary>
        Unknown,
        /// <summary>
        /// The object has not been started yet.
        /// </summary>
        NotStarted,
        /// <summary>
        /// The object is starting up but has not finished the start process yet.
        /// </summary>
        Starting,
        /// <summary>
        /// The object has finished the start process and is running.
        /// </summary>
        Started,
        /// <summary>
        /// The object has been running and entered a non-started state.
        /// </summary>
        Other,
    }

    /// <summary>
    /// Defines the interface for classes that provide startup progress
    /// </summary>
    public interface INeedStartup
    {
        /// <summary>
        /// The event that is fired when there is an update in the startup process.
        /// </summary>
        event EventHandler<StartupProgressEventArgs> StartupProgress;

        /// <summary>
        /// Starts the startup process.
        /// </summary>
        void Start();

        /// <summary>
        /// Returns a value indicating what the state of the object is regarding
        /// the startup process.
        /// </summary>
        /// <returns>
        ///   The current startup state for the object.
        /// </returns>
        StartupState GetStartupState();
    }
}