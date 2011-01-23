//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Apollo.UI.Common
{
    /// <summary>
    /// A base class for parameters, which are arguments for a presenter.
    /// </summary>
    public abstract class Parameter : Observable
    {
        /// <summary>
        /// The collection of properties for the current parameter.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "The variable needs to hold a collection of functions. Unfortunately nested generics is unavoidable.")]
        private readonly List<Func<object>> m_Properties = new List<Func<object>>();

        /// <summary>
        /// Reuses the view by.
        /// </summary>
        /// <param name="properties">The properties.</param>
        protected void ReuseViewBy(params Func<object>[] properties)
        {
            m_Properties.Clear();
            m_Properties.AddRange(properties);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Parameter"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="Parameter"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="Parameter"/> is equal to this instance; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Equals(Parameter other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return other.GetHashCode() == GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public sealed override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((Parameter)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public sealed override int GetHashCode()
        {
            unchecked
            {
                var result = 0;
                foreach (var property in m_Properties)
                {
                    var value = property();
                    result = (result * 397) ^ (value != null ? value.GetHashCode() : 0);
                }

                return result;
            }
        }
    }
}