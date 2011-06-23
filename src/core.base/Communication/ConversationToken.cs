//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using Apollo.Utilities;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Defines a token used to link parts of a 'conversation' that takes place between to 
    /// applications.
    /// </summary>
    [Serializable]
    public sealed class ConversationToken : Id<ConversationToken, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConversationToken"/> class.
        /// </summary>
        /// <param name="endpoint">The ID of the endpoint from which this token originates.</param>
        internal ConversationToken(EndpointId endpoint)
            : this(endpoint, Guid.NewGuid())
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConversationToken"/> class.
        /// </summary>
        /// <param name="endpoint">The ID of the endpoint from which this token originates.</param>
        /// <param name="token">The <see cref="Guid"/> that identifies this token.</param>
        internal ConversationToken(EndpointId endpoint, Guid token)
            : this(string.Format(CultureInfo.InvariantCulture, "{0}:{1}", endpoint, token))
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConversationToken"/> class.
        /// </summary>
        /// <param name="tokenText">The text for the token.</param>
        internal ConversationToken(string tokenText)
            : base(tokenText)
        { 
        }

        /// <summary>
        /// Performs the actual act of creating a copy of the current ID number.
        /// </summary>
        /// <param name="value">The internally stored value.</param>
        /// <returns>
        /// A copy of the current ID number.
        /// </returns>
        protected override ConversationToken Clone(string value)
        {
            return new ConversationToken(value);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return InternalValue;
        }
    }
}
