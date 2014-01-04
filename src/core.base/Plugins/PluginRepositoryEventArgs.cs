//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core.Base.Plugins
{
    /// <summary>
    /// Defines an <see cref="EventArgs"/> class that stores information about a plug-in repository
    /// that raised the event.
    /// </summary>
    public sealed class PluginRepositoryEventArgs : EventArgs
    {
        /// <summary>
        /// The ID of the repository that raised the event.
        /// </summary>
        private readonly PluginRepositoryId m_Id;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginRepositoryEventArgs"/> class.
        /// </summary>
        /// <param name="id">The ID of the repository.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="id"/> is <see langword="null" />.
        /// </exception>
        public PluginRepositoryEventArgs(PluginRepositoryId id)
        {
            {
                Lokad.Enforce.Argument(() => id);
            }

            m_Id = id;
        }

        /// <summary>
        /// Gets the ID of the repository that raised the event.
        /// </summary>
        public PluginRepositoryId Repository
        {
            get
            {
                return m_Id;
            }
        }
    }
}
