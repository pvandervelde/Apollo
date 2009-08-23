//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace Apollo.Core.Messages
{
    /// <summary>
    /// Defines the interface for classes which link <see cref="DnsName"/> data 
    /// to <see cref="IDnsNameObject"/> objects.
    /// </summary>
    public interface INameService : IEnumerable<IDnsNameObject>
    {
        /// <summary>
        /// Adds the <c>IDnsNameObject</c> object to the collection.
        /// </summary>
        /// <param name="obj">The object to add.</param>
        void Add(IDnsNameObject obj);

        /// <summary>
        /// Removes the <c>IDnsNameObject</c> with the specified
        /// <c>DnsName</c> from the collection.
        /// </summary>
        /// <param name="name">
        ///     The name of the object which must be removed.
        /// </param>
        void Remove(DnsName name);

        /// <summary>
        /// Returns a value indicating if an object is registered with
        /// the specified <c>DnsName</c>.
        /// </summary>
        /// <param name="name">
        ///     The name of an <c>IDnsNameObject</c> object.
        /// </param>
        /// <returns>
        ///     <see langword="true"/> if an object is registered with the specified 
        ///     <c>DnsName</c>; otherwise <see langword="false" />.
        /// </returns>
        bool HasName(DnsName name);
    }
}
