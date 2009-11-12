//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Globalization;
using Lokad;

namespace Apollo.Core.Messages
{
    /// <summary>
    /// Defines the name of a service.
    /// </summary>
    public struct DnsName : IEquatable<DnsName>, IComparable<DnsName>, IComparable
    {
        // RENAME THIS!!!!!

        /// <summary>
        /// The unique identifier for the combination of all services.
        /// </summary>
        private readonly static DnsName s_AllServices = new DnsName(@"AllServices");

        /// <summary>
        /// The unique identifier for none of the services.
        /// </summary>
        private readonly static DnsName s_Nobody = new DnsName(@"Nobody");

        /// <summary>
        /// Returns the <c>DnsName</c> for the combination of all services.
        /// Any messages send with this <c>DnsName</c> as recipient will
        /// be send to all active services, except for the originating 
        /// service.
        /// </summary>
        /// <value>The <c>DnsName</c> for the combination of all services.</value>
        public static DnsName AllServices
        {
            get 
            {
                return s_AllServices;
            }
        }

        /// <summary>
        /// Returns the <c>DnsName</c> for none of the services.
        /// Any messages send with this <c>DnsName</c> as recipient will
        /// be removed by the message pipeline.
        /// </summary>
        /// <value>The <c>DnsName</c> for none of the services.</value>
        public static DnsName Nobody
        {
            get 
            {
                return s_Nobody;
            }
        }

        /// <summary>
        /// The identifier of the <c>DnsName</c>.
        /// </summary>
        private readonly string m_Name;

        /// <summary>
        /// Initializes a new instance of the <see cref="DnsName"/> struct.
        /// </summary>
        /// <param name="name">The name.</param>
        public DnsName(string name)
        { 
            {
                Enforce.That(!string.IsNullOrEmpty(name), "The DNS name should be specified.");
            }

            m_Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DnsName"/> struct.
        /// </summary>
        /// <param name="nameToCopy">The <c>DnsName</c> which should be copied.</param>
        public DnsName(DnsName nameToCopy) : this(nameToCopy.m_Name)
        { }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="other">An object to compare with this instance.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has these meanings:
        /// Value
        /// Meaning
        /// Less than zero
        /// This instance is less than <paramref name="other"/>.
        /// Zero
        /// This instance is equal to <paramref name="other"/>.
        /// Greater than zero
        /// This instance is greater than <paramref name="other"/>.
        /// </returns>
        public int CompareTo(DnsName other)
        {
            return other.m_Name.CompareTo(m_Name);
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has these meanings:
        /// Value
        /// Meaning
        /// Less than zero
        /// This instance is less than <paramref name="obj"/>.
        /// Zero
        /// This instance is equal to <paramref name="obj"/>.
        /// Greater than zero
        /// This instance is greater than <paramref name="obj"/>.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        /// 	<paramref name="obj"/> is not the same type as this instance.
        /// </exception>
        int IComparable.CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            if (!(obj is DnsName))
            { 
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, @"Cannot compare these objects: {0} and {1}", this, obj));
            }
            return CompareTo((DnsName)obj);
        }

        /// <summary>
        /// Determines whether the specified <c>DnsName</c> is equal to this instance.
        /// </summary>
        /// <param name="other">The <c>DnsName</c> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <c>DnsName</c> is equal to this instance; otherwise, <see langword="false"/>.
        /// </returns>
        public bool Equals(DnsName other)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <see langword="false"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is DnsName)
            {
                return Equals((DnsName)obj);
            }
            return false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return m_Name.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, @"DnsName: {0}", m_Name);
        }


    }
}