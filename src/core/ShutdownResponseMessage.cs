//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Messaging;

namespace Apollo.Core
{
    /// <summary>
    /// The response to a <see cref="ShutdownRequestMessage"/>.
    /// </summary>
    [Serializable]
    internal sealed class ShutdownResponseMessage : MessageBody
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShutdownResponseMessage"/> class.
        /// </summary>
        /// <param name="wasGranted">If set to <see langword="true"/> then the request for a shut down was granted.</param>
        public ShutdownResponseMessage(bool wasGranted)
            : base(false)
        {
            WasRequestGranted = wasGranted;
        }

        /// <summary>
        /// Gets a value indicating whether the request for a shut down was granted.
        /// </summary>
        /// <value>
        ///     <see langword="true"/> if the request for a shut down was granted; otherwise, <see langword="false"/>.
        /// </value>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool WasRequestGranted
        {
            get;
            private set;
        }

        #region Overrides of MessageBody

        /// <summary>
        /// Copies this instance.
        /// </summary>
        /// <returns>
        /// A new copy of the current <c>MessageBody</c>.
        /// </returns>
        public override MessageBody Copy()
        {
            return new ShutdownResponseMessage(WasRequestGranted);
        }

        /// <summary>
        /// Determines whether the specified <see cref="MessageBody"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="MessageBody"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="MessageBody"/> is equal to this instance; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public override bool Equals(MessageBody other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            var message = other as ShutdownResponseMessage;
            return message != null && WasRequestGranted == message.WasRequestGranted;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            // As obtained from the Jon Skeet answer to:  http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
            // And adapted towards the Modified Bernstein (shown here: http://eternallyconfuzzled.com/tuts/algorithms/jsw_tut_hashing.aspx)
            //
            // Overflow is fine, just wrap
            unchecked
            {
                // Pick a random prime number
                int hash = 17;

                // Mash the hash together with yet another random prime number
                hash = (hash * 23) ^ GetType().GetHashCode();
                hash = (hash * 23) ^ WasRequestGranted.GetHashCode();

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
            return WasRequestGranted ? "Shutdown request granted." : "Shutdown request denied.";
        }

        #endregion
    }
}
