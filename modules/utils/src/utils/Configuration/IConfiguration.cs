using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        /// <param name="key">The key.</param>
        /// <returns>
        /// The desired value.
        /// </returns>
        T Value<T>(IConfigurationKey key);
    }
}
