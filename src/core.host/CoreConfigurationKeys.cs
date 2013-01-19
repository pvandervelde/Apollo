//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Utilities.Configuration;

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
        public static readonly ConfigurationKey PluginLocation = new ConfigurationKey();
    }
}
