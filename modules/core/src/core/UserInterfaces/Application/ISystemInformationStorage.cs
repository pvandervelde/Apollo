//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core.UserInterfaces.Application
{
    /// <summary>
    /// Defines the interface for objects that store information about the
    /// state of Apollo.
    /// </summary>
    internal interface ISystemInformationStorage
    {
        /// <summary>
        /// Gets a value indicating the version of the core assembly.
        /// </summary>
        Version CoreVersion
        {
            get;
        }

        /// <summary>
        /// Gets or sets a value indicating the startup time of the core.
        /// </summary>
        DateTimeOffset StartupTime
        {
            get;
            set;
        }
    }
}
