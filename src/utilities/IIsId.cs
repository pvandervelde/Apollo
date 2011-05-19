//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Utilities
{
    /// <summary>
    /// Defines the base interface for ID numbers.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Derivative classes should define the type parameters as:
    /// </para>
    /// <example>
    /// public sealed class SomeId : IIsId&lt;SomeId&gt;
    /// </example>
    /// </remarks>
    /// <typeparam name="TId">The type of the object which is the ID.</typeparam>
    public interface IIsId<TId> : IComparable<TId>, IComparable, IEquatable<TId> where TId : IIsId<TId>
    {
        /// <summary>
        /// Clones this ID number.
        /// </summary>
        /// <returns>
        /// A copy of the current ID number.
        /// </returns>
        TId Clone();
    }
}
