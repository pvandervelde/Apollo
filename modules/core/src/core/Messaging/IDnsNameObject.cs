﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Core.Messaging
{
    /// <summary>
    /// Defines the interface for objects which have a unique <see cref=" DnsName"/> with 
    /// which they can be identified.
    /// </summary>
    public interface IDnsNameObject
    {
        /// <summary>
        /// Gets the identifier of the object.
        /// </summary>
        /// <value>The identifier.</value>
        DnsName Name { get; }
    }
}