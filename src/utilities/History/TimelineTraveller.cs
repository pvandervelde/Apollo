//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;

namespace Apollo.Utilities.History
{
    /// <summary>
    /// Stores object state that should survive a roll-back or roll-forward.
    /// </summary>
    [Serializable]
    public sealed class TimelineTraveller
    {
        /// <summary>
        /// The ID number of the owner who created the time traveling information.
        /// </summary>
        private readonly HistoryId m_OwnerId;

        /// <summary>
        /// The information that should be provided to the owner after the 
        /// roll-back or roll-forward.
        /// </summary>
        private readonly Stream m_Information;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimelineTraveller"/> class.
        /// </summary>
        /// <param name="owner">The ID number of the owner who created the time traveling information.</param>
        /// <param name="information">
        /// The information that should be provided to the owner after the roll-back or roll-forward.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="owner"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="information"/> is <see langword="null" />.
        /// </exception>
        public TimelineTraveller(HistoryId owner, Stream information)
        {
            {
                Lokad.Enforce.Argument(() => owner);
                Lokad.Enforce.Argument(() => information);
            }

            m_OwnerId = owner;
            m_Information = information;
        }

        /// <summary>
        /// Gets the ID number of the owner who created the time traveling information.
        /// </summary>
        public HistoryId Owner
        {
            get
            {
                return m_OwnerId;
            }
        }

        /// <summary>
        /// Gets the stream that contains the time traveling information.
        /// </summary>
        public Stream Information
        {
            get
            {
                return m_Information;
            }
        }
    }
}
