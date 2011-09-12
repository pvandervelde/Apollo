//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Defines the interface for objects that carry information, in serialized form, about 
    /// an event.
    /// </summary>
    internal interface ISerializedEventRegistration : IEquatable<ISerializedEventRegistration>
    {
        /// <summary>
        /// Gets the command set on which the method was invoked.
        /// </summary>
        ISerializedType Type
        {
            get;
        }

        /// <summary>
        /// Gets the name of the event.
        /// </summary>
        string MemberName
        {
            get;
        }
    }
}
