//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Lokad;

namespace Apollo.Utilities.Licensing
{
    /// <summary>
    /// Stores the result of a verification calculation for use by the framework.
    /// </summary>
    [Serializable]
    internal struct LicenseCheckResult : IEquatable<LicenseCheckResult>
    {
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(LicenseCheckResult first, LicenseCheckResult second)
        {
            return first.Equals(second);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(LicenseCheckResult first, LicenseCheckResult second)
        {
            return !first.Equals(second);
        }

        /// <summary>
        /// The time on which this verification result was generated.
        /// </summary>
        private readonly DateTimeOffset m_GenerationTime;

        /// <summary>
        /// The time beyond which this result is considered expired.
        /// </summary>
        private readonly DateTimeOffset m_ExpirationTime;

        /// <summary>
        /// The checksum that was passed when this result was generated.
        /// </summary>
        private readonly Checksum m_Checksum;

        /// <summary>
        /// Initializes a new instance of the <see cref="LicenseCheckResult"/> struct.
        /// </summary>
        /// <param name="generationTime">The time on which this result was generated.</param>
        /// <param name="expirationTime">The time this result expires.</param>
        /// <param name="checksum">The checksum.</param>
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
        public LicenseCheckResult(DateTimeOffset generationTime, DateTimeOffset expirationTime, Checksum checksum)
        {
            {
                Enforce.With<ArgumentOutOfRangeException>(
                    !generationTime.Equals(DateTimeOffset.MinValue), 
                    SrcOnlyResources.ExceptionMessagesArgumentOutOfRangeWithArgument, 
                    generationTime);
                Enforce.With<ArgumentOutOfRangeException>(
                    !generationTime.Equals(DateTimeOffset.MaxValue), 
                    SrcOnlyResources.ExceptionMessagesArgumentOutOfRangeWithArgument, 
                    generationTime);

                Enforce.With<ArgumentOutOfRangeException>(
                    !expirationTime.Equals(DateTimeOffset.MinValue), 
                    SrcOnlyResources.ExceptionMessagesArgumentOutOfRangeWithArgument, 
                    expirationTime);
                Enforce.With<ArgumentOutOfRangeException>(
                    !expirationTime.Equals(DateTimeOffset.MaxValue), 
                    SrcOnlyResources.ExceptionMessagesArgumentOutOfRangeWithArgument, 
                    expirationTime);
                Enforce.With<ArgumentOutOfRangeException>(
                    generationTime < expirationTime, 
                    SrcOnlyResources.ExceptionMessagesArgumentOutOfRangeWithArgument, 
                    expirationTime);
            }

            m_GenerationTime = generationTime;
            m_ExpirationTime = expirationTime;
            m_Checksum = checksum;
        }

        /// <summary>
        /// Gets the time this license check result was generated.
        /// </summary>
        /// <value>The time this license check result was generated.</value>
        public DateTimeOffset Generated
        {
            get
            {
                return m_GenerationTime;
            }
        }

        /// <summary>
        /// Gets the expiration time for this license check result.
        /// </summary>
        /// <value>The expiration time for this license check result.</value>
        public DateTimeOffset Expires
        {
            get
            {
                return m_ExpirationTime;
            }
        }

        /// <summary>
        /// Gets the checksum.
        /// </summary>
        /// <value>The checksum.</value>
        public Checksum Checksum
        {
            get
            {
                return m_Checksum;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="LicenseCheckResult"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="LicenseCheckResult"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="LicenseCheckResult"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Equals(LicenseCheckResult other)
        {
            return m_GenerationTime.Equals(other.m_GenerationTime) &&
                   m_ExpirationTime.Equals(other.m_ExpirationTime) &&
                   m_Checksum.Equals(other.m_Checksum);
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
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            if (!(obj is LicenseCheckResult))
            {
                return false;
            }

            return Equals((LicenseCheckResult)obj);
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
                hash = (hash * 23) ^ m_GenerationTime.GetHashCode();
                hash = (hash * 23) ^ m_ExpirationTime.GetHashCode();
                hash = (hash * 23) ^ m_Checksum.GetHashCode();

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
                "License validated at {0}. Validation expires: {1}.", 
                m_GenerationTime, 
                m_ExpirationTime);
        }
    }
}
