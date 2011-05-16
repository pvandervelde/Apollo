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
    /// a specific <see cref="ICommandSet"/>.
    /// </summary>
    internal interface ISerializedType : IEquatable<ISerializedType>
    {
        /// <summary>
        /// Gets the assembly qualified name of the command set type.
        /// </summary>
        string AssemblyQualifiedTypeName
        {
            get;
        }
    }
}
