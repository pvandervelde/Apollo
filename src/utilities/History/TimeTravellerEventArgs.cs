//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Utilities.History
{
    /// <summary>
    /// Defines an <see cref="EventArgs"/> object that stores <see cref="TimelineTraveller"/> information.
    /// </summary>
    public sealed class TimeTravellerEventArgs : EventArgs
    {
        /// <summary>
        /// The object that stores the time travelling information.
        /// </summary>
        private readonly TimelineTraveller m_Traveller;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeTravellerEventArgs"/> class.
        /// </summary>
        /// <param name="traveller">The object that stores the time travelling information.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="traveller"/> is <see langword="null" />.
        /// </exception>
        public TimeTravellerEventArgs(TimelineTraveller traveller)
        {
            {
                Lokad.Enforce.Argument(() => traveller);
            }

            m_Traveller = traveller;
        }

        /// <summary>
        /// Gets the object that stores the time travelling information.
        /// </summary>
        public TimelineTraveller Traveller
        {
            get 
            {
                return m_Traveller;
            }
        }
    }
}
