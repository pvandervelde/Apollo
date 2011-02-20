//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Defines the base methods for classes that implement a set of commands 
    /// that can be invoked remotely through a <see cref="MessageHub"/>.
    /// </summary>
    /// <design>
    /// The <see cref="MessageHub"/> will generate a proxy object for all the command sets
    /// available on a given endpoint.
    /// </design>
    public interface ICommandSet
    {
        /// <summary>
        /// An event raised when the endpoint to which the commandset belongs
        /// becomes available or unavailable.
        /// </summary>
        /// <remarks>
        /// Note that changes in availability do not mean that the endpoint has
        /// permanently been terminated (although that may be the case). It merely
        /// means that the endpoint is temporarily not available.
        /// </remarks>
        event EventHandler<CommandSetAvailabilityEventArgs> OnAvailabilityChange;

        /// <summary>
        /// An event raised when the endpoint to which the command set belongs
        /// becomes invalid.
        /// </summary>
        event EventHandler<EventArgs> OnInvalidate;
    }
}
