//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Defines the interface for objects that carry information, in serialized form, about 
    /// a method call.
    /// </summary>
    internal interface ISerializedMethodInvocation : IEquatable<ISerializedMethodInvocation>
    {
        /// <summary>
        /// Gets the command set on which the method was invoked.
        /// </summary>
        ISerializedType CommandSet
        {
            get;
        }

        /// <summary>
        /// Gets the name of the method.
        /// </summary>
        string MemberName
        {
            get;
        }

        /// <summary>
        /// Gets a collection which contains the types and values of the parameters.
        /// </summary>
        List<Tuple<ISerializedType, object>> Parameters
        {
            get;
        }
    }
}
