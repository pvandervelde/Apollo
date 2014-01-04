//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Nuclei.Configuration;

namespace Apollo.Core.Host
{
    /// <summary>
    /// Defines the <see cref="ConfigurationKey"/> objects for the core.
    /// </summary>
    internal static class CoreConfigurationKeys
    {
        /// <summary>
        /// The <see cref="ConfigurationKey"/> that is used to retrieve the location or locations of
        /// the plugin files on the disk or network.
        /// </summary>
        public static readonly ConfigurationKey PluginLocation
            = new ConfigurationKey("PluginLocation", typeof(string));

        /// <summary>
        /// The <see cref="ConfigurationKey"/> that is used to retrieve the maximum time plug-in information
        /// may be cached without getting updates from a plug-in repository.
        /// </summary>
        public static readonly ConfigurationKey RepositoryCacheExpirationTimeInMilliSeconds
            = new ConfigurationKey("RepositoryCacheExpirationTimeInMilliSeconds", typeof(int));
    }
}
