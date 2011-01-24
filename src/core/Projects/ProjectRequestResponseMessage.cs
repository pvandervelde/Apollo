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
        /// The project that was requested.
        /// </summary>
        private readonly IProject m_Project;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectRequestResponseMessage"/> class.
        /// </summary>
        /// <param name="project">The <see cref="IProject"/>.</param>
        public ProjectRequestResponseMessage(IProject project)
            : base(false)
        {
            m_Project = project;
        }

        /// <summary>
        /// Gets a value indicating the currently active project.
        /// </summary>
        public IProject ProjectReference
        {
            get
            {
                return m_Project;
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
            return new ProjectRequestResponseMessage(m_Project);
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
            return (msg != null) && (msg.m_Project == m_Project);
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
            return "Returning the project.";
        }

        #endregion
    }
}
