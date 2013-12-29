//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Nuclei.Configuration;

namespace Apollo.Service.Repository
{
    /// <summary>
    /// Defines the <see cref="ConfigurationKey"/> objects for the repository service.
    /// </summary>
    internal static class RepositoryServiceConfigurationKeys
    {
        /// <summary>
        /// The <see cref="ConfigurationKey"/> that is used to retrieve the location or locations of
        /// the plugin files on the disk or network.
        /// </summary>
        public static readonly ConfigurationKey PluginLocation
            = new ConfigurationKey("PluginLocation", typeof(string));
    }
}
