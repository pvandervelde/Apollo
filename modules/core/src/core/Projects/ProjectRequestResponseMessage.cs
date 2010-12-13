//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
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
        /// The project that is was requested.
        /// </summary>
        /// <design>
        /// This field is not serialized by the default serializer because we need to do
        /// some special work to serialize it. <c>Project</c> is a <c>MarshalByRefObject</c>
        /// and is not serializable. We could capture an <c>ObjRef</c> object by calling
        /// <c>RemotingServices.Marshal(MarshalByRefObject)</c> but once the <c>ObjRef</c>
        /// object is deserialized it wants to turn into a project (as proxy) and not
        /// an <c>ObjRef</c>. So we'll do the serialization ourselves.
        /// </design>
        private readonly string m_RemotingUri;

        // Could send back just a string and then push the project into a 'tunnel'. Where the
        // tunnel is the object that stores ObjRef objects until they get collected.

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectRequestResponseMessage"/> class.
        /// </summary>
        /// <param name="remotingUri">The string that was used to provide the <see cref="IProject"/> to the remoting infrastructure.</param>
        public ProjectRequestResponseMessage(string remotingUri)
            : base(false)
        {
            m_RemotingUri = remotingUri;
        }

        /// <summary>
        /// Gets a value indicating the currently active project.
        /// </summary>
        public string ProjectRemotingUri
        {
            get
            {
                return m_RemotingUri;
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
            return new ProjectRequestResponseMessage(m_RemotingUri);
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
            return (msg != null) && (msg.m_RemotingUri == m_RemotingUri);
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
