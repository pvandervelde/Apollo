//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Provides extension methods for the <see cref="ConversationToken"/> class.
    /// </summary>
    public static class ConversationTokenExtensions
    {
        /// <summary>
        /// Deserializes an <see cref="ConversationToken"/> from a string.
        /// </summary>
        /// <param name="serializedToken">The string containing the serialized <see cref="ConversationToken"/> information.</param>
        /// <returns>A new <see cref="ConversationToken"/> based on the given <paramref name="serializedToken"/>.</returns>
        public static ConversationToken Deserialize(string serializedToken)
        {
            // @todo: do we check that this string has the right format?
            return new ConversationToken(serializedToken);
        }
    }
}
