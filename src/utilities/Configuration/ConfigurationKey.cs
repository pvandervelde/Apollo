//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Utilities.Configuration
{
    /// <summary>
    /// Defines the interface for objects that serve as keys for the <see cref="IConfiguration"/> collection.
    /// </summary>
    public sealed class ConfigurationKey
    {
        // Store some hierarchical key?
        // What kind of storage do we want to allow?
        // - XML
        // - SQL
        //
        // Group --> Indicates what the config is globally for. e.g. Network
        // Block --> Indicates what the config is exactly for. e.g. TCP
        // Entry --> Indicates a single setting. e.g. TCP port
    }
}
