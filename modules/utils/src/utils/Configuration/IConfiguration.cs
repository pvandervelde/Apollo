﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Utils.Configuration
{
    /// <summary>
    /// Defines the interface for the configuration settings object for the application.
    /// </summary>
    public interface IConfiguration
    {
        /// <summary>
        /// Returns the value for the given configuration key.
        /// </summary>
        /// <typeparam name="T">The type of the return value.</typeparam>
        /// <param name="key">The configuration key.</param>
        /// <returns>
        /// The desired value.
        /// </returns>
        T Value<T>(IConfigurationKey key);
    }
}
