//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace Apollo.Utilities.Configuration
{
    /// <summary>
    /// Defines helper methods for dealing with configurations.
    /// </summary>
    public static class ConfigurationHelpers
    {
        /// <summary>
        /// The default key for the value that indicates if the profiler should be loaded or not.
        /// </summary>
        private const string LoadProfilerAppSetting = "LoadProfiler";

        /// <summary>
        /// Returns a value indicating if the application should activate the internal profiler.
        /// </summary>
        /// <returns>
        /// <see langword="true" /> if the application should activate the internal profiler; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public static bool ShouldBeProfiling()
        {
            try
            {
                var value = ConfigurationManager.AppSettings[LoadProfilerAppSetting];

                bool result = true;
                bool methodResult = bool.TryParse(value, out result);
                return result && methodResult;
            }
            catch (ConfigurationErrorsException)
            {
                // could not retrieve the AppSetting from the config file
                // meh ...
                return false;
            }
        }
    }
}
