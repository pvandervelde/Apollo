//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Reflection;
using Apollo.Internals;

namespace Apollo.Utilities
{
    /// <summary>
    /// Holds a set of constants that apply to the application.
    /// </summary>
    public static class ApplicationConstants
    {
        /// <summary>
        /// Gets the assembly that called into this assembly.
        /// </summary>
        /// <returns>
        /// The calling assembly.
        /// </returns>
        private static Assembly GetAssembly()
        {
            var assembly = Assembly.GetEntryAssembly();
            if (assembly == null)
            { 
                // Either we're being called from unmanaged code
                // or we're in a different appdomain than the actual executable
                assembly = Assembly.GetExecutingAssembly();
            }

            return assembly;
        }

        /// <summary>
        /// Gets the name of the company.
        /// </summary>
        /// <value>The name of the company.</value>
        public static string CompanyName
        {
            get
            {
                return CompanyInformation.CompanyName;
            }
        }

        /// <summary>
        /// Gets the name of the application.
        /// </summary>
        /// <value>The name of the application.</value>
        public static string ApplicationName
        {
            get
            {
                return ProductInformation.ProductName;
            }
        }

        /// <summary>
        /// Gets the application version.
        /// </summary>
        /// <value>The application version.</value>
        public static Version ApplicationVersion
        {
            get
            {
                var applicationVersion = GetAssembly().GetName().Version;
                return applicationVersion;
            }
        }

        /// <summary>
        /// Gets the application version with which this application is compatible.
        /// </summary>
        /// <value>The application compatibility version.</value>
        /// <remarks>
        /// A compatible application version indicates that the current version reads the
        /// configuration files of the compatible application.
        /// </remarks>
        public static Version ApplicationCompatibilityVersion
        {
            get
            {
                var fullVersion = ApplicationVersion;
                return new Version(fullVersion.Major, fullVersion.Minor);
            }
        }
    }
}
