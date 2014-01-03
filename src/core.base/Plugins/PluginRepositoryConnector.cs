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
    }
}
