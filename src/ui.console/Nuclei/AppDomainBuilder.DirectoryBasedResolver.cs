//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Nuclei;

namespace Apollo.UI.Console.Nuclei
{
    /// <content>
    /// Contains the definition of the <see cref="DirectoryBasedResolver"/> class.
    /// </content>
    internal static partial class AppDomainBuilder
    {
        /// <summary>
        /// Attaches a method to the <see cref="AppDomain.AssemblyResolve"/> event and provides
        /// assembly resolution based on the files available in a set of predefined directories.
        /// </summary>
        private sealed class DirectoryBasedResolver : MarshalByRefObject, IAppDomainAssemblyResolver
        {
            /// <summary>
            /// Stores the file extensions and other constants related to file paths.
            /// </summary>
            private IFileConstants m_FileConstants;

            /// <summary>
            /// Stores the directories as a collection of directory paths.
            /// </summary>
            /// <design>
            /// Explicitly store the directory paths in strings because DirectoryInfo objects are eventually
            /// nuked because DirectoryInfo is a MarshalByRefObject and can thus go out of scope.
            /// </design>
            private IEnumerable<string> m_Directories;

            /// <summary>
            /// Stores the paths to the relevant directories.
            /// </summary>
            /// <param name="fileConstants">The file constants.</param>
            /// <param name="directoryPaths">The paths to the relevant directories.</param>
            /// <exception cref="ArgumentNullException">
            ///     Thrown if <paramref name="fileConstants"/> is <see langword="null" />.
            /// </exception>
            /// <exception cref="ArgumentNullException">
            /// Thrown when <paramref name="directoryPaths"/> is <see langword="null"/>.
            /// </exception>
            public void StoreDirectoryPaths(IFileConstants fileConstants, IEnumerable<string> directoryPaths)
            {
                {
                    Lokad.Enforce.Argument(() => fileConstants);
                    Lokad.Enforce.Argument(() => directoryPaths);
                }

                m_FileConstants = fileConstants;
                m_Directories = directoryPaths;
            }

            /// <summary>
            /// Attaches the assembly resolution method to the <see cref="AppDomain.AssemblyResolve"/>
            /// event of the current <see cref="AppDomain"/>.
            /// </summary>
            /// <exception cref="InvalidOperationException">
            /// Thrown when <see cref="DirectoryBasedResolver.StoreDirectoryPaths"/> has not been called prior to
            /// attaching the directory resolver to an <see cref="AppDomain"/>.
            /// </exception>
            public void Attach()
            {
                {
                    Lokad.Enforce.NotNull(() => m_Directories);
                }

                var domain = AppDomain.CurrentDomain;
                {
                    // For each path in the list get all the assembly files in that path.
                    var helper = new FusionHelper(
                        () => m_Directories.SelectMany(
                            dir => Directory.GetFiles(
                                dir, 
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    "*.{0}",
                                    m_FileConstants.AssemblyExtension), 
                                SearchOption.AllDirectories)),
                        m_FileConstants);
                    domain.AssemblyResolve += helper.LocateAssemblyOnAssemblyLoadFailure;
                }
            }
        }
    }
}
