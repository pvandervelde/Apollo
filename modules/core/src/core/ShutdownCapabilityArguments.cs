//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core
{
    /// <summary>
    /// Defines an <see cref="INotificationArguments"/> object that stores
    /// information about the ability of a service to shut down.
    /// </summary>
    [Serializable]
    public sealed class ShutdownCapabilityArguments : INotificationArguments
    {
        /// <summary>
        /// Gets or sets a value indicating whether the service can shut down.
        /// </summary>
        public bool CanShutDown
        {
            get;
            set;
        }
    }
}
