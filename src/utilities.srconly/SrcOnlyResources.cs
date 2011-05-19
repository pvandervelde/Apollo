//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Utilities.Logging;
using Apollo.Utilities.Properties;

namespace Apollo.Utilities
{
    /// <summary>
    /// Defines methods for retrieving resource strings.
    /// </summary>
    /// <design>
    /// Unlike any of the other source code in the <c>Apollo.Utilities.SrcOnly</c> project
    /// this class should NOT be copied to the host project. Define a NEW class called
    /// <c>Apollo.Utilities.SrcOnlyResources</c> that mimicks the current class.
    /// </design>
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

        /// <summary>
        /// Gets the string resource for an internal error with an error code formatter.
        /// </summary>
        /// <value>
        /// The string resource.
        /// </value>
        public static string ExceptionMessagesInternalErrorWithCode
        {
            get 
            {
                return Resources.Exception_Messages_InternalError_WithCode;
            }
        }

        /// <summary>
        /// Gets the string resource for a license verification failure.
        /// </summary>
        /// <value>
        /// The string resource.
        /// </value>
        public static string ExceptionsMessagesVerificationFailure
        {
            get 
            {
                return Resources.Exceptions_Messages_VerificationFailure;
            }
        }
    }
}
