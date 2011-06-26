//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Globalization;

namespace Apollo.Core.Base
{
    /// <summary>
    /// Provides extension methods for the <see cref="DatasetId"/> class.
    /// </summary>
    public static class DatasetIdExtensions
    {
        /// <summary>
        /// Deserializes an <see cref="EndpointId"/> from a string.
        /// </summary>
        /// <param name="serializedId">The string containing the serialized <see cref="DatasetId"/> information.</param>
        /// <returns>A new <see cref="DatasetId"/> based on the given <paramref name="serializedId"/>.</returns>
        public static DatasetId Deserialize(string serializedId)
        {
            return new DatasetId(int.Parse(serializedId, NumberStyles.Integer, CultureInfo.InvariantCulture));
        }
    }
}
