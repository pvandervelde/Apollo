//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Remoting;
using Apollo.Core.Messaging;

namespace Apollo.Core.Projects
{
    /// <summary>
    /// A message send in response to the requesting of a project. Contains the 
    /// current project or <see langword="null" /> if there is no current project.
    /// </summary>
    [Serializable]
    internal sealed class ProjectRequestResponseMessage : MessageBody
    {
        /// <summary>
        /// The project that was requested.
        /// </summary>
        /// <design>
        /// This <c>ObjRef</c> needs to be created explicitly through a call to 
        /// <c>RemotingServices.Marshal</c> in order to make sure we get the reference.
        /// If we try to send the project directly (or an implicity ObjRef) then 
        /// the proxy will be created in the first AppDomain where the ObjRef is deserialized.
        /// That is not what we want because there are multiple AppDomains involved in sending 
        /// messages in the application.
        /// </design>
        private readonly ObjRef m_ProjectReference;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectRequestResponseMessage"/> class.
        /// </summary>
        /// <param name="projectReference">The <see cref="ObjRef"/> object that will create the proxy to the <see cref="IProject"/>.</param>
        public ProjectRequestResponseMessage(ObjRef projectReference)
            : base(false)
        {
            m_ProjectReference = projectReference;
        }

        /// <summary>
        /// Gets a value indicating the currently active project.
        /// </summary>
        public ObjRef ProjectReference
        {
            get
            {
                return m_ProjectReference;
            }
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
            return new ProjectRequestResponseMessage(m_ProjectReference);
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

            var msg = other as ProjectRequestResponseMessage;
            return (msg != null) && (msg.m_ProjectReference == m_ProjectReference);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return GetType().GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "Returning the remoting URI.";
        }

        #endregion
    }
}
