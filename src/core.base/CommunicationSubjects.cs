//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

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
        public static readonly CommunicationSubject Dataset
            = new CommunicationSubject("Apollo.Subject: Dataset");
    }
}
