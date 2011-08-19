//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace Apollo.Utilities.ExceptionHandling
{
    /// <summary>
    /// Defines methods and variables for the generation of application logs and report dump files.
    /// </summary>
    internal static class ReportingUtilities
    {
        /// <summary>
        /// Defines the extension of a dump file.
        /// </summary>
        public const string DumpFileExtension = "nsdump";

        /// <summary>
        /// Returns the full path to the directory that is used to write the application logs.
        /// </summary>
        /// <returns>
        ///     The full path to the application specific log directory.
        /// </returns>
        public static string ProductSpecificApplicationDataDirectory()
        {
            // Get the local file path. There is a function for this (ApplicationConstants)
            // but we don't want to use it because we want to prevent any of our own code from
            // loading. If an exception happens that is not handled then we might be having
            // loader issues. These are probably due to us trying to load some of our code or
            // one of it's dependencies. Given that this is causing a problem it seems wise to not
            // try to use our (external) code to find an assembly file path ...
            var localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            if (localAppDataPath == null)
            {
                throw new DirectoryNotFoundException();
            }

            var companyAttribute = GetAttributeFromAssembly<AssemblyCompanyAttribute>();
            Debug.Assert((companyAttribute != null) && !string.IsNullOrEmpty(companyAttribute.Company), "There should be a company name.");

            var productAttribute = GetAttributeFromAssembly<AssemblyProductAttribute>();
            Debug.Assert((productAttribute != null) && !string.IsNullOrEmpty(productAttribute.Product), "There should be a product name.");

            var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;

            var companyDirectory = Path.Combine(localAppDataPath, companyAttribute.Company);
            var productDirectory = Path.Combine(companyDirectory, productAttribute.Product);
            var versionDirectory = Path.Combine(productDirectory, new Version(assemblyVersion.Major, assemblyVersion.Minor).ToString(2));

            return versionDirectory;
        }

        /// <summary>
        /// Gets the attribute from the calling assembly.
        /// </summary>
        /// <typeparam name="T">The type of attribute that should be gotten from the assembly.</typeparam>
        /// <returns>
        /// The requested attribute.
        /// </returns>
        private static T GetAttributeFromAssembly<T>() where T : Attribute
        {
            var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(T), false);
            Debug.Assert(attributes.Length == 1, "There should only be one attribute.");

            var requestedAttribute = attributes[0] as T;
            Debug.Assert(requestedAttribute != null, "Found an incorrect attribute type.");

            return requestedAttribute;
        }

        /// <summary>
        /// Returns the name for a report dump file.
        /// </summary>
        /// <returns>
        ///     The name of a new dump file.
        /// </returns>
        public static string GenerateDumpFileName()
        {
            // Write the GUID in registry format: {00000000-0000-0000-0000-000000000000}
            return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", Guid.NewGuid().ToString("B"), DumpFileExtension);
        }
    }
}
