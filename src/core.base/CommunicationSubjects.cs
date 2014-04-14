//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Nuclei.Communication;

namespace Apollo.Core.Base
{
    /// <summary>
    /// Defines the communication subjects for the Apollo application.
    /// </summary>
    public static class CommunicationSubjects
    {
        /// <summary>
        /// The communication subject for Apollo applications that deal with dataset information.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "ConfigurationKey is immutable")]
        public static readonly CommunicationSubject Dataset
            = new CommunicationSubject("Apollo.Subject: Dataset");

        /// <summary>
        /// The communication subject for Apollo applications that deal with plugin information.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "ConfigurationKey is immutable")]
        public static readonly CommunicationSubject Plugins
            = new CommunicationSubject("Apollo.Subject: Plugin");
    }
}
