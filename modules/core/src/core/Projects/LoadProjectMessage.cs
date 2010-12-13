//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base;
using Apollo.Core.Messaging;
using Lokad;

namespace Apollo.Core.Projects
{
    /// <summary>
    /// A message send to request the loading of a project. The response message will
    /// be a <see cref="ProjectRequestResponseMessage"/>.
    /// </summary>
    [Serializable]
    internal sealed class LoadProjectMessage : MessageBody
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoadProjectMessage"/> class.
        /// </summary>
        /// <param name="persistedProject">The persistence information for the persisted project.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="persistedProject"/> is <see langword="null" />.
        /// </exception>
        public LoadProjectMessage(IPersistenceInformation persistedProject)
            : base(true)
        {
            {
                Enforce.Argument(() => persistedProject);
            }

            PersistedProject = persistedProject;
        }

        /// <summary>
        /// Gets a value indicating the persistence information from which the 
        /// project should be restored.
        /// </summary>
        public IPersistenceInformation PersistedProject
        {
            get;
            private set;
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
            return new LoadProjectMessage(PersistedProject);
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

            var msg = other as LoadProjectMessage;
            return (msg != null) && (msg.PersistedProject == PersistedProject);
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
            return "Request loading of project.";
        }

        #endregion
    }
}
