//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Reflection;

namespace Apollo.Core.Host.UserInterfaces.Application
{
    /// <summary>
    /// Defines an object that stores information about the state of Apollo.
    /// </summary>
    internal sealed class SystemInformationStorage : ISystemInformationStorage
    {
        /// <summary>
        /// The version number of the core assembly.
        /// </summary>
        /// <design>
        /// This was made static because it is highly unlikely that this value will
        /// change during the execution of the application.
        /// </design>
        private static readonly Version s_CoreVersion = DetermineCoreVersion();

        /// <summary>
        /// Determines the version number of the core assembly.
        /// </summary>
        /// <returns>
        ///     The requested version number.
        /// </returns>
        private static Version DetermineCoreVersion()
        { 
            var coreAssembly = Assembly.GetExecutingAssembly();
            return coreAssembly.GetName().Version;
        }

        /// <summary>
        /// Gets the version of the core assembly.
        /// </summary>
        public Version CoreVersion
        {
            get 
            {
                return s_CoreVersion;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the startup time of the core.
        /// </summary>
        public DateTimeOffset StartupTime
        {
            get;
            set;
        }
    }
}
