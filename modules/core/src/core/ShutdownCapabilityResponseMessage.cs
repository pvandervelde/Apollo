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
    /// A message send by a service to indicate if it can or cannot be shut down.
    /// </summary>
    [Serializable]
    internal sealed class ShutdownCapabilityResponseMessage : MessageBody
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShutdownCapabilityResponseMessage"/> class.
        /// </summary>
        /// <param name="canShutdown">If set to <see langword="true"/> then the sending service can be shutdown.</param>
        public ShutdownCapabilityResponseMessage(bool canShutdown)
            : base(false)
        {
            CanShutdown = canShutdown;
        }

        /// <summary>
        /// Gets a value indicating whether the service that send the response can shutdown.
        /// </summary>
        /// <value>
        ///     <see langword="true"/> if the service that send the response can shutdown; otherwise, <see langword="false"/>.
        /// </value>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool CanShutdown
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
            return new ShutdownCapabilityResponseMessage(CanShutdown);
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

            var message = other as ShutdownCapabilityResponseMessage;
            return message != null && CanShutdown == message.CanShutdown;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return GetType().GetHashCode() ^ CanShutdown.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return CanShutdown ? "Can shutdown." : "Cannot shutdown.";
        }

        #endregion
    }
}
