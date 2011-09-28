//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Core.Host
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
        /// The object has failed during the startup process.
        /// </summary>
        Failed,

        /// <summary>
        /// The object was running but is in the process of stopping.
        /// </summary>
        Stopping,

        /// <summary>
        /// The object was running but is now stopped.
        /// </summary>
        Stopped,
    }
}
