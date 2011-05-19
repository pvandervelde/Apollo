//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Lokad;
using Lokad.Rules;

namespace Apollo.Utilities.Licensing
{
    /// <summary>
    /// Stores information about a checksum that is used by the verification system.
    /// </summary>
    [DebuggerDisplayAttribute("Hash = {m_Base64Hash}")]
    [Serializable]
    internal partial struct Checksum : IEquatable<Checksum>
    {
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Checksum first, Checksum second)
        {
            return first.Equals(second);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Checksum first, Checksum second)
        {
            return !first.Equals(second);
        }

        /// <summary>
        /// Stores the hash (as base-64 string) of the validation, the generation
        /// time and the expiration time.
        /// </summary>
        /// <design>
        /// <para>
        /// The component ID is purposely not included in the hash because 
        /// that would make it impossible to re-use the validation result.
        /// </para>
        /// <para>
        /// Note that storing the validation result as a hash will only make things
        /// midly harder for the attacker because we need a way to calculate the same
        /// hash in the different componenents. Which means that we need to know
        /// what the success result validation value is throughout the application.
        /// </para>
        /// </design>
        private readonly string m_Base64Hash;

        /// <summary>
        /// Initializes a new instance of the <see cref="Checksum"/> struct.
        /// </summary>
        /// <param name="validationResult">The validation result.</param>
        /// <param name="generationTime">The time the validation result was computed.</param>
        /// <param name="expirationTime">The time the validation result expires.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="validationResult"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="validationResult"/> is an empty string.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown when <paramref name="generationTime"/> is equal to <see cref="DateTimeOffset.MinValue"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown when <paramref name="generationTime"/> is equal to <see cref="DateTimeOffset.MaxValue"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown when <paramref name="expirationTime"/> is equal to <see cref="DateTimeOffset.MinValue"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown when <paramref name="expirationTime"/> is equal to <see cref="DateTimeOffset.MaxValue"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown when <paramref name="generationTime"/> is larger or equal to <paramref name="expirationTime"/>.
        /// </exception>
        public Checksum(string validationResult, DateTimeOffset generationTime, DateTimeOffset expirationTime)
        {
            {
                Enforce.Argument(() => validationResult);
                Enforce.Argument(() => validationResult, StringIs.NotEmpty);

                Enforce.With<ArgumentOutOfRangeException>(!generationTime.Equals(DateTimeOffset.MinValue), SrcOnlyResources.ExceptionMessagesArgumentOutOfRangeWithArgument, generationTime);
                Enforce.With<ArgumentOutOfRangeException>(!generationTime.Equals(DateTimeOffset.MaxValue), SrcOnlyResources.ExceptionMessagesArgumentOutOfRangeWithArgument, generationTime);

                Enforce.With<ArgumentOutOfRangeException>(!expirationTime.Equals(DateTimeOffset.MinValue), SrcOnlyResources.ExceptionMessagesArgumentOutOfRangeWithArgument, expirationTime);
                Enforce.With<ArgumentOutOfRangeException>(!expirationTime.Equals(DateTimeOffset.MaxValue), SrcOnlyResources.ExceptionMessagesArgumentOutOfRangeWithArgument, expirationTime);
                Enforce.With<ArgumentOutOfRangeException>(generationTime < expirationTime, SrcOnlyResources.ExceptionMessagesArgumentOutOfRangeWithArgument, expirationTime);
            }

            m_Base64Hash = ComputeHash(validationResult, generationTime, expirationTime);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Checksum"/> struct.
        /// </summary>
        /// <param name="checksumToCopy">The checksum that should be copied.</param>
        public Checksum(Checksum checksumToCopy)
        {
            m_Base64Hash = checksumToCopy.m_Base64Hash;
        }

        /// <summary>
        /// Gets the hash code of the validation result.
        /// </summary>
        /// <value>The hash code of the validation result.</value>
        public string ValidationHash
        {
            get
            {
                return m_Base64Hash;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="Checksum"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="Checksum"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="Checksum"/> is equal to this instance; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Equals(Checksum other)
        {
            return string.Equals(m_Base64Hash, other.m_Base64Hash, StringComparison.Ordinal);
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
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            if (!(obj is Checksum))
            {
                return false;
            }

            return Equals((Checksum)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return m_Base64Hash.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return m_Base64Hash;
        }
    }
}