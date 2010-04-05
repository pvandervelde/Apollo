//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Reflection;
using Apollo.Utils;

namespace Apollo.Core.Utils
{
    /// <summary>
    /// Holds a set of constants that apply to the application.
    /// </summary>
    internal sealed class ApplicationConstants : IApplicationConstants, ICompanyConstants
    {
        /// <summary>
        /// Gets the attribute from the current assembly.
        /// </summary>
        /// <typeparam name="T">The type of attribute that should be gotten from the assembly.</typeparam>
        /// <returns>
        /// The requested attribute.
        /// </returns>
        private static T GetAttributeFromCurrentAssembly<T>() where T : Attribute
        {
            var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(T), false);
            Debug.Assert(attributes.Length == 1, "There should only be one attribute.");

            var requestedAttribute = attributes[0] as T;
            Debug.Assert(requestedAttribute != null, "Found an incorrect attribute type.");

            return requestedAttribute;
        }

        /// <summary>
        /// Gets the name of the company.
        /// </summary>
        /// <value>The name of the company.</value>
        public string CompanyName
        {
            get
            {
                var assemblyCompany = GetAttributeFromCurrentAssembly<AssemblyCompanyAttribute>();
                return assemblyCompany.Company;
            }
        }

        /// <summary>
        /// Gets the name of the application.
        /// </summary>
        /// <value>The name of the application.</value>
        public string ApplicationName
        {
            get
            {
                var assemblyName = GetAttributeFromCurrentAssembly<AssemblyProductAttribute>();
                return assemblyName.Product;
            }
        }

        /// <summary>
        /// Gets the application version.
        /// </summary>
        /// <value>The application version.</value>
        public Version ApplicationVersion
        {
            get
            {
                var applicationVersion = GetAttributeFromCurrentAssembly<AssemblyVersionAttribute>();
                return new Version(applicationVersion.Version);
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
        public Version ApplicationCompatibilityVersion
        {
            get
            {
                var fullVersion =  ApplicationVersion;
                return new Version(fullVersion.Major, fullVersion.Minor);
            }
        }
    }
}
