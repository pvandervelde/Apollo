//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using System.Security;
using System.Diagnostics.CodeAnalysis;


namespace Apollo.Utils.Fusion
{
    /// <summary>
    /// Contains methods for assisting with the assembly loading process.
    /// </summary>
    /// <remarks>
    /// Note because these methods assist in the assembly loading process it
    /// is not possible to place this class in a separate assembly from the
    /// elements which need to provide assembly loading assistence.
    /// </remarks>
    /// <design>
    /// <para>
    /// The goal of the <c>FusionHelper</c> class is to provide a fallback for the
    /// assembly loading process. The <c>LocateAssemblyOnAssemblyLoadFailure</c> method
    /// is attached to the <c>AppDomain.AssemblyResolve</c> event. 
    /// </para>
    /// <para>
    /// The <c>FusionHelper</c> searches through a set of directories for assembly files.
    /// The assembly files that are found are checked to see if they match with the requested
    /// assembly file.
    /// </para>
    /// <para>
    /// The choice was made to search through a set of directories, and not for instance
    /// a list of known assemblies, because:
    /// <list type="Bullet">
    /// <item>
    /// It is not possible to know which dependencies exist in the assemblies we have to load
    /// </item>
    /// <item>
    /// We do not know which assemblies can safely be loaded in the AppDomain prior to setting up
    /// the loader. On the other hand it is easier to make the load / not-load decision based on
    /// the directory that the assemblies have to come from.
    /// </item>
    /// </list>
    /// </para>
    /// </design>
    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", 
        Justification="Source will be linked from other projects and thus be used.")]
    internal sealed class FusionHelper
    {
        // @TODO: Do we need to make this all thread safe? Probably, but how...

        /// <summary>
        /// The collection that holds all the directories that should be searched
        /// for the 'missing' assemblies.
        /// </summary>
        private string m_Directory;

        /// <summary>
        /// The delegate which is used to return a file enumerator based on a specific directory.
        /// </summary>
        private Func<IEnumerable<string>> m_FileEnumerator;

        /// <summary>
        /// The delegate which is used to load an assembly from a specific file path.
        /// </summary>
        private Func<string, Assembly> m_AssemblyLoader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FusionHelper"/> class.
        /// </summary>
        public FusionHelper() : this((string)null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FusionHelper"/> class.
        /// </summary>
        /// <param name="baseDirectory">The base directory.</param>
        public FusionHelper(DirectoryInfo baseDirectory) : this(baseDirectory.FullName)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FusionHelper"/> class.
        /// </summary>
        /// <param name="baseDirectory">The base directory.</param>
        public FusionHelper(string baseDirectory)
        {
            m_Directory = baseDirectory;
        }

        /// <summary>
        /// Gets or sets the file enumerator which is used to enumerate the files in a specific directory. 
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
            Justification = "Source will be linked from other projects and thus be used.")]
        internal Func<IEnumerable<string>> FileEnumerator
        {
            private get 
            {
                if (m_FileEnumerator == null)
                {
                    Debug.Assert(!string.IsNullOrEmpty(m_Directory), "The directory must be specified!");
                    m_FileEnumerator = () => Directory.GetFiles(m_Directory, FileExtensions.AssemblyExtension, SearchOption.AllDirectories);
                }

                return m_FileEnumerator;
            }
            set 
            {
                m_FileEnumerator = value;
            }
        }

        /// <summary>
        /// Gets or sets the assembly loader which is used to load assemblies from a specific path.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
            Justification = "Source will be linked from other projects and thus be used.")]
        internal Func<string, Assembly> AssemblyLoader 
        {
            private get 
            {
                if (m_AssemblyLoader == null)
                {
                    m_AssemblyLoader = (path) => Assembly.LoadFrom(path);
                }
                return m_AssemblyLoader;
            }
            set 
            {
                m_AssemblyLoader = value;
            }
        }

        /// <summary>
        /// An event handler which is invoked when the search for an assembly fails.
        /// </summary>
        /// <param name="sender">The object which raised the event.</param>
        /// <param name="args">
        ///     The <see cref="System.ResolveEventArgs"/> instance containing the event data.
        /// </param>
        /// <returns>
        ///     An assembly reference if the required assembly can be found; otherwise <see langword="null"/>.
        /// </returns>
        public Assembly LocateAssemblyOnAssemblyLoadFailure(object sender, ResolveEventArgs args)
        {
            // This handler is called only when the common language runtime tries to bind to 
            // an assembly and fails to locate the assembly.
            return LocateAssembly(args.Name);
        }

        private Assembly LocateAssembly(string assemblyFullName)
        {
            Debug.Assert(assemblyFullName != null, "Expected a non-null assembly name string.");
            Debug.Assert(assemblyFullName != string.Empty, "Expected a non-empty assembly name string.");

            // It is not possible to use the AssemblyName class because that attempts to load the 
            // assembly. Obviously we're are currently trying to find the assembly.
            // So parse the actual assembly name from the name string
            
            // First check if we have been passed a fully qualified name or only a module name
            string fileName = assemblyFullName;
            string version = string.Empty;
            string culture = string.Empty;
            string publicKey = string.Empty;
            if (IsAssemblyNameFullyQualified(assemblyFullName))
            {
                // Split the assembly name out into different parts. The name
                // normally consists of:
                // - File name
                // - Version
                // - Culture
                // - PublicKeyToken
                // e.g.: mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
                string[] nameSections = assemblyFullName.Split(',');
                Debug.Assert(nameSections.Length == 4, "There should be 4 sections in the assembly name.");

                // The first section is the module name
                fileName = nameSections[0].Trim();
                // The second section is the version number
                version = ExtractValueFromKeyValuePair(nameSections[1]);
                // The third element is the culture
                culture = ExtractValueFromKeyValuePair(nameSections[2]);
                // The final element is the public key
                publicKey = ExtractValueFromKeyValuePair(nameSections[3]);
            }

            // If the file name already has the '.dll' extension then we don't need to add that, otherwise we do
            fileName = MakeModuleNameQualifiedFileName(fileName);

            // Search through all the directories and see if we can match the assemblyFileName with any of
            // the files in the stored directories
            var files = FileEnumerator();

            // Search for the first file that matches the assembly we're looking for
            var match = (from filePath in files
                         where (IsFileTheDesiredAssembly(filePath, fileName, version, culture, publicKey))
                         select filePath)
                         .FirstOrDefault();

            if (match != null)
            {
                return AssemblyLoader(match); 
            }

            // Did not find the assembly.
            return null;
        }

        private  bool IsAssemblyNameFullyQualified(string assemblyFullName)
        {
            Debug.Assert(!string.IsNullOrEmpty(assemblyFullName), "The assembly full name should not be empty.");

            // We will assume that the name is fully qualified if there is a comma in the file name.
            // If there is a comma in the file name it is relatively safe to assume that the file name is NOT
            // a qualified assembly file path so it will probably be a fully qualified assembly name.
            return assemblyFullName.Contains(",");
        }

        private static string ExtractValueFromKeyValuePair(string input)
        {
            Debug.Assert(!string.IsNullOrEmpty(input), "The input should not be empty.");

            return input
                .Substring(input.IndexOf(AssemblyNameElements.KeyValueSeparator) + AssemblyNameElements.KeyValueSeparator.Length)
                .Trim();
        }

        private static string MakeModuleNameQualifiedFileName(string fileName)
        {
            Debug.Assert(!string.IsNullOrEmpty(fileName), "The assembly file name should not be empty.");

            return (fileName.IndexOf(FileExtensions.AssemblyExtension, StringComparison.OrdinalIgnoreCase) < 0) ?
                string.Format("{0}{1}", fileName, FileExtensions.AssemblyExtension) :
                fileName;
        }

        /// <summary>
        /// Determines if the file at the specified <paramref name="filePath"/> is the assembly that the loader is
        /// looking for.
        /// </summary>
        /// <param name="filePath">The absolute file path to the file which might be the desired assembly.</param>
        /// <param name="fileName">The file name and extension for the desired assembly.</param>
        /// <param name="version">The version for the desired assembly.</param>
        /// <param name="culture">The culture for the desired assembly.</param>
        /// <param name="publicKey">The public key tocken for the desired assembly.</param>
        /// <returns>
        ///     <see langword="true"/> if the filePath points to the desired assembly; otherwise <see langword="false"/>.
        /// </returns>
        private bool IsFileTheDesiredAssembly(string filePath, string fileName, string version, string culture, string publicKey)
        {
            Debug.Assert(!string.IsNullOrEmpty(filePath), "The assembly file path should not be empty.");
            if (!Path.GetFileName(filePath).Equals(fileName, StringComparison.CurrentCultureIgnoreCase))
            {
                return false;
            }

            // Check the file version and culture too            

            // The path exists so there is a file with the specific file name. This is probably
            // an assembly.
            if ((!string.IsNullOrEmpty(version)) || (!string.IsNullOrEmpty(culture)) || (!string.IsNullOrEmpty(publicKey)))
            {
                AssemblyName assemblyName = null;
                try
                {
                    // Load the assembly name but without loading the assembly file into the AppDomain.
                    assemblyName = AssemblyName.GetAssemblyName(filePath);
                }
                catch (ArgumentException)
                {
                    // filePath is invalid, e.g. an assembly with an invalid culture.
                    return false;
                }
                catch (FileNotFoundException)
                {
                    // filePath doesn't point to a valid file or doesn't exist
                    return false;
                }
                catch (SecurityException)
                {
                    // The caller doesn't have discovery permission for the given path
                    return false;
                }
                catch (BadImageFormatException)
                { 
                    // The file is not a valid assembly file
                    return false;
                }
                catch (FileLoadException)
                {
                    // the file was already loaded but with a different set of evidence
                    return false;
                }

                if (!string.IsNullOrEmpty(version))
                {
                    Version expectedVersion = new Version(version);
                    if (!expectedVersion.Equals(assemblyName.Version))
                    {
                        return false;
                    }
                }

                if (!string.IsNullOrEmpty(culture))
                {
                    // The 'Neutral' culture is actually the invariant culture. This is the culture an
                    // assembly gets if no culture was specified so...
                    if (culture.Equals(AssemblyNameElements.InvariantCulture, StringComparison.InvariantCultureIgnoreCase))
                    {
                        culture = string.Empty;
                    }

                    CultureInfo expectedCulture = new CultureInfo(culture);
                    if (!expectedCulture.Equals(assemblyName.CultureInfo))
                    {
                        return false;
                    }
                }

                if ((!string.IsNullOrEmpty(publicKey)) && (!publicKey.Equals(AssemblyNameElements.NullString, StringComparison.CurrentCultureIgnoreCase)))
                {
                    var actualPublicKeyToken = assemblyName.GetPublicKeyToken();
                    var str = new System.Text.ASCIIEncoding().GetString(actualPublicKeyToken);
                    return str.Equals(publicKey, StringComparison.CurrentCultureIgnoreCase);
                }
            }

            return true;
        }
    }
}