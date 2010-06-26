//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Utils.Properties;

namespace Apollo.Utils
{
    /// <summary>
    /// Defines methods for retrieving resource strings.
    /// </summary>
    /// <design>
    /// Unlike any of the other source code in the <c>Apollo.Utils.SrcOnly</c> project
    /// this class should NOT be copied to the host project. Define a NEW class called
    /// <c>Apollo.Utils.SrcOnlyResources</c> that mimicks the current class.
    /// </design>
    internal static class SrcOnlyResources
    {
        /// <summary>
        /// Gets the string resource for an <see cref="ArgumentOutOfRangeException"/>.
        /// </summary>
        /// <value>The string resource.</value>
        public static string Exception_Messages_ArgumentOutOfRange
        {
            get
            {
                return Resources.Exception_Messages_ArgumentOutOfRange;
            }
        }

        /// <summary>
        /// Gets the string resource for an <see cref="ArgumentOutOfRangeException"/> with an argument formatter.
        /// </summary>
        /// <value>The string resource.</value>
        public static string Exception_Messages_ArgumentOutOfRange_WithArgument
        {
            get
            {
                return Resources.Exception_Messages_ArgumentOutOfRange_WithArgument;
            }
        }
    }
}
