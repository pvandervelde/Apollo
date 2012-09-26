﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace Apollo.Core.Host.Plugins.Definitions
{
    /// <summary>
    /// Stores information about a schedule action in a serializable form, i.e. without requiring the owning
    /// type to be loaded.
    /// </summary>
    [Serializable]
    internal sealed class SerializedScheduleActionDefinition : IEquatable<SerializedScheduleActionDefinition>
    {
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(SerializedScheduleActionDefinition first, SerializedScheduleActionDefinition second)
        {
            // Check if first is a null reference by using ReferenceEquals because
            // we overload the == operator. If first isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(first, null) && ReferenceEquals(second, null))
            {
                return true;
            }

            var nonNullObject = first;
            var possibleNullObject = second;
            if (ReferenceEquals(first, null))
            {
                nonNullObject = second;
                possibleNullObject = first;
            }

            return nonNullObject.Equals(possibleNullObject);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(SerializedScheduleActionDefinition first, SerializedScheduleActionDefinition second)
        {
            // Check if first is a null reference by using ReferenceEquals because
            // we overload the == operator. If first isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(first, null) && ReferenceEquals(second, null))
            {
                return false;
            }

            var nonNullObject = first;
            var possibleNullObject = second;
            if (ReferenceEquals(first, null))
            {
                nonNullObject = second;
                possibleNullObject = first;
            }

            return !nonNullObject.Equals(possibleNullObject);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="SerializedScheduleActionDefinition"/> class based 
        /// on the given <see cref="MethodInfo"/>.
        /// </summary>
        /// <param name="method">The method for which a serialized definition needs to be created.</param>
        /// <param name="identityGenerator">The function that creates type identities.</param>
        /// <returns>The serialized definition for the given method.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="method"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="identityGenerator"/> is <see langword="null" />.
        /// </exception>
        public static SerializedScheduleActionDefinition CreateDefinition(
            MethodInfo method,
            Func<Type, SerializedTypeIdentity> identityGenerator)
        {
            {
                Lokad.Enforce.Argument(() => method);
                Lokad.Enforce.Argument(() => identityGenerator);
            }

            return new SerializedScheduleActionDefinition(SerializedMethodDefinition.CreateDefinition(method, identityGenerator));
        }

        /// <summary>
        /// Creates a new instance of the <see cref="SerializedScheduleActionDefinition"/> class 
        /// based on the given <see cref="MethodInfo"/>.
        /// </summary>
        /// <param name="method">The method for which a serialized definition needs to be created.</param>
        /// <returns>The serialized definition for the given method.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="method"/> is <see langword="null" />.
        /// </exception>
        public static SerializedScheduleActionDefinition CreateDefinition(MethodInfo method)
        {
            return CreateDefinition(method, t => SerializedTypeIdentity.CreateDefinition(t));
        }

        /// <summary>
        /// The action method.
        /// </summary>
        private readonly SerializedMethodDefinition m_Method;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializedScheduleActionDefinition"/> class.
        /// </summary>
        /// <param name="method">The method that will perform the action.</param>
        private SerializedScheduleActionDefinition(SerializedMethodDefinition method)
        {
            {
                Debug.Assert(method != null, "The method object should not be null.");
            }

            m_Method = method;
        }

        /// <summary>
        /// Gets the method that will perform the action.
        /// </summary>
        public SerializedMethodDefinition Method
        {
            get
            {
                return m_Method;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="SerializedScheduleActionDefinition"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="SerializedScheduleActionDefinition"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="SerializedScheduleActionDefinition"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Equals(SerializedScheduleActionDefinition other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // Check if other is a null reference by using ReferenceEquals because
            // we overload the == operator. If other isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            return !ReferenceEquals(other, null) 
                && Method == other.Method;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="System.Object"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public sealed override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var id = obj as SerializedScheduleActionDefinition;
            return Equals(id);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            // As obtained from the Jon Skeet answer to:
            // http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
            // And adapted towards the Modified Bernstein (shown here: http://eternallyconfuzzled.com/tuts/algorithms/jsw_tut_hashing.aspx)
            //
            // Overflow is fine, just wrap
            unchecked
            {
                // Pick a random prime number
                int hash = 17;

                // Mash the hash together with yet another random prime number
                hash = (hash * 23) ^ Method.GetHashCode();

                return hash;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "Action on {0}",
                Method);
        }
    }
}
