//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Apollo.Utils;
using Apollo.Utils.Fusion;
using Lokad;

namespace Apollo.Core
{
    /// <content>
    /// Contains the definition of the <see cref="FileBasedResolver"/> class.
    /// </content>
    internal static partial class AppDomainBuilder
    {
        /// <summary>
        /// Attaches a method to the <see cref="AppDomain.AssemblyResolve"/> event and
        /// provides assembly resolution based on a set of predefined files.
        /// </summary>
        [ExcludeFromCoverage("This class is used to handle AppDomain construction for Apollo. Integration testing is more suitable.")]
        private sealed class FileBasedResolver : MarshalByRefObject, IAppDomainAssemblyResolver
        {
            /// <summary>
            /// Stores the file extensions and other constants related to file paths.
            /// </summary>
            private IFileConstants m_FileConstants;

            /// <summary>
            /// Stores the files as a collection of file paths.
            /// </summary>
            /// <design>
            /// Explicitly store the file paths in strings because FileInfo objects are eventually
            /// nuked because FileInfo is a MarshalByRefObject and can thus go out of scope.
            /// </design>
            private IEnumerable<string> m_Files;

            /// <summary>
            /// Stores the paths to the relevant assemblies.
            /// </summary>
            /// <param name="fileConstants">The file constants.</param>
            /// <param name="filePaths">The paths to the relevant assemblies.</param>
            /// <exception cref="ArgumentNullException">
            ///     Thrown if <paramref name="fileConstants"/> is <see langword="null" />.
            /// </exception>
            /// <exception cref="ArgumentNullException">
            ///     Thrown when <paramref name="filePaths"/> is <see langword="null" />.
            /// </exception>
            public void StoreFilePaths(IFileConstants fileConstants, IEnumerable<string> filePaths)
            {
                {
                    Enforce.Argument(() => fileConstants);
                    Enforce.Argument(() => filePaths); 
                }

                m_FileConstants = fileConstants;
                m_Files = filePaths;
            }

            /// <summary>
            /// Attaches the assembly resolution method to the <see cref="AppDomain.AssemblyResolve"/>
            /// event of the current <see cref="AppDomain"/>.
            /// </summary>
            /// <exception cref="InvalidOperationException">
            /// Thrown when <see cref="FileBasedResolver.StoreFilePaths"/> has not been called prior to
            /// attaching the directory resolver to an <see cref="AppDomain"/>.
            /// </exception>
            public void Attach()
            {
                {
                    Enforce.NotNull(() => m_Files);
                }

                var domain = AppDomain.CurrentDomain;
                {
                    var helper = new FusionHelper(
                        () => m_Files,
                        m_FileConstants);

                    domain.AssemblyResolve += helper.LocateAssemblyOnAssemblyLoadFailure;
                }
            }
        }
    }
}