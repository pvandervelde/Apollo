//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Defines a set of constant values related to communication.
    /// </summary>
    public interface ICommunicationConstants
    {
        /// <summary>
        /// Gets the time span used for time-out values in commands
        /// that should only take a very short time (i.e. command execution
        /// time is neglible compared to communication time).
        /// </summary>
        TimeSpan ShortCommandTimeout
        {
            get;
        }
    }
}
