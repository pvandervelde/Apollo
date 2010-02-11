//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Lokad;
using Lokad.Rules;

namespace Apollo.Utils
{
    /// <summary>
    /// An attribute used to indicate that the element decorated by the 
    /// attribute should be excluded for unit testing coverage.
    /// </summary>
    [AttributeUsage(
       AttributeTargets.Class |
       AttributeTargets.Struct |
       AttributeTargets.Constructor |
       AttributeTargets.Event |
       AttributeTargets.Method |
       AttributeTargets.Property,
       AllowMultiple = false,
       Inherited = false)]
    public sealed class ExcludeFromCoverageAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExcludeFromCoverageAttribute"/> class.
        /// </summary>
        /// <param name="reason">The reason for excluding the specified element from coverage.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="reason"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="reason"/> is an empty string.
        /// </exception>
        public ExcludeFromCoverageAttribute(string reason)
        {
            {
                Enforce.Argument(() => reason);
                Enforce.Argument(() => reason, StringIs.NotEmpty);
            }

            Reason = reason;
        }

        /// <summary>
        /// Gets the reason for excluding the specified element from coverage.
        /// </summary>
        /// <value>The reason for excluding the specified element from coverage.</value>
        public string Reason
        {
            get;
            private set;
        }
    }
}
