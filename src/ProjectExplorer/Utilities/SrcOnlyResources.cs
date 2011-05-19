//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.ProjectExplorer.Properties;
using Apollo.Utilities.Logging;

namespace Apollo.Utilities
{
    /// <summary>
    /// Defines methods for retrieving resource strings.
    /// </summary>
    /// <design>
    /// This class handles the resource retrieval for the code that is linked from 
    /// <c>Apollo.Utilities.SrcOnly</c> project.
    /// </design>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Src",
        Justification = "This class is only used internally.")]
    internal static class SrcOnlyResources
    {
        /// <summary>
        /// Gets the string resource for an <see cref="ArgumentOutOfRangeException"/>.
        /// </summary>
        /// <value>The string resource.</value>
        public static string ExceptionMessagesArgumentOutOfRange
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
        public static string ExceptionMessagesArgumentOutOfRangeWithArgument
        {
            get
            {
                return Resources.Exception_Messages_ArgumentOutOfRange_WithArgument;
            }
        }

        /// <summary>
        /// Gets the string resource for an <see cref="Exception"/> that is thrown if a user tries to set
        /// the <see cref="ILogMessage.Level"/> to <see cref="LevelToLog.None"/>.
        /// </summary>
        public static string ExceptionMessagesCannotLogMessageWithLogLevelSetToNone
        {
            get
            {
                return Resources.Exception_Messages_CannotLogMessageWithLogLevelSetToNone;
            }
        }
    }
}
