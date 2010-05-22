﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Apollo.Utils;

namespace Apollo.Core.Utils
{
    /// <summary>
    /// Holds a set of constants that apply to the application.
    /// </summary>
    [Serializable]
    internal sealed class ApplicationConstants : IApplicationConstants, ICompanyConstants
    {
        /// <summary>
        /// Gets the assembly that called into this assembly.
        /// </summary>
        /// <returns>
        /// The calling assembly.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate",
            Justification = "This member is potentially expensive to invoke and should thus be a method, not a property.")]
        private static Assembly GetAssembly()
        {
            return Assembly.GetExecutingAssembly();
        }

        /// <summary>
        /// Gets the attribute from the calling assembly.
        /// </summary>
        /// <typeparam name="T">The type of attribute that should be gotten from the assembly.</typeparam>
        /// <returns>
        /// The requested attribute.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "The type parameter indicates which attribute we're looking for.")]
        private static T GetAttributeFromAssembly<T>() where T : Attribute
        {
            var attributes = GetAssembly().GetCustomAttributes(typeof(T), false);
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
                var assemblyCompany = GetAttributeFromAssembly<AssemblyCompanyAttribute>();
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
                var assemblyName = GetAttributeFromAssembly<AssemblyProductAttribute>();
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
        public Version ApplicationCompatibilityVersion
        {
            get
            {
                var fullVersion = ApplicationVersion;
                return new Version(fullVersion.Major, fullVersion.Minor);
            }
        }
    }
}