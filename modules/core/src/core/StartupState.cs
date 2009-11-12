//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

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
}