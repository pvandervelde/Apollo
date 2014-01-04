//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Apollo.Core.Base.Plugins
{
    /// <summary>
    /// Provides a connection to one or more plug-in repositories.
    /// </summary>
    internal sealed class PluginRepositoryConnector : IProvideConnectionToRepositories
    {
        /// <summary>
        /// Returns a value indicating whether the specified repository can be contacted.
        /// </summary>
        /// <param name="id">The ID of the repository.</param>
        /// <returns>
        /// <see langword="true" /> if the specified repository can be contacted; otherwise, <see langword="false" />.
        /// </returns>
        public bool IsConnectedToRepository(PluginRepositoryId id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a collection containing the IDs of all the repositories that can be contacted.
        /// </summary>
        /// <returns>A collection containing the IDs of all the connected repositories.</returns>
        public IEnumerable<PluginRepositoryId> Repositories()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a value indicating if the repository with the given <paramref name="id"/> has information about a type 
        /// with the given <paramref name="fullyQualifiedName"/>.
        /// </summary>
        /// <param name="id">The ID of the plug-in repository.</param>
        /// <param name="fullyQualifiedName">The assembly qualified name of the type.</param>
        /// <returns>
        /// <see langword="true" /> if the specified repository has information about the given type; otherwise, <see langword="false" />.
        /// </returns>
        public bool HasTypeInformation(PluginRepositoryId id, string fullyQualifiedName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the plug-in information from the specified repository.
        /// </summary>
        /// <param name="id">The ID of the repository from which the plug-in information should be obtained.</param>
        /// <returns>The information about all the plug-ins stored by the repository.</returns>
        public RepositoryPluginInformation PluginInformationFrom(PluginRepositoryId id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// An event raised if a new plug-in repository connects.
        /// </summary>
        public event EventHandler<PluginRepositoryEventArgs> OnRepositoryConnected;

        private void RaiseOnRepositoryConnected(PluginRepositoryId id)
        {
            var local = OnRepositoryConnected;
            if (local != null)
            {
                local(this, new PluginRepositoryEventArgs(id));
            }
        }

        /// <summary>
        /// An event raised if a plug-in repository indicates that it has updated one or 
        /// more plug-in definitions.
        /// </summary>
        public event EventHandler<PluginRepositoryEventArgs> OnRepositoryUpdated;

        private void RaiseOnRepositoryUpdated(PluginRepositoryId id)
        {
            var local = OnRepositoryUpdated;
            if (local != null)
            {
                local(this, new PluginRepositoryEventArgs(id));
            }
        }

        /// <summary>
        /// An event raised if a plug-in repository disconnects.
        /// </summary>
        public event EventHandler<PluginRepositoryEventArgs> OnRepositoryDisconnected;

        private void RaiseOnRepositoryDisconnected(PluginRepositoryId id)
        {
            var local = OnRepositoryDisconnected;
            if (local != null)
            {
                local(this, new PluginRepositoryEventArgs(id));
            }
        }
    }
}
