//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using Apollo.Utils.Fusion;
using Lokad;

namespace Apollo.Core
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
            /// <param name="directoryPaths">
            ///     The paths to the relevant directories.
            /// </param>
            /// <exception cref="ArgumentNullException">
            /// Thrown when <paramref name="directoryPaths"/> is <see langword="null" />.
            /// </exception>
            public void StoreDirectoryPaths(IEnumerable<string> directoryPaths)
            {
                {
                    Enforce.Argument(() => directoryPaths);
                }

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
            [SecurityCritical]
            [SecurityTreatAsSafe]
            public void Attach()
            {
                {
                    Enforce.NotNull(() => m_Directories);
                }

                var domain = AppDomain.CurrentDomain;
                {
                    // For each path in the list get all the assembly files in that path.
                    var helper = new FusionHelper(() => m_Directories.SelectMany(dir => Directory.GetFiles(dir, FileExtensions.AssemblyExtension, SearchOption.AllDirectories)));

                    // Asset permission to control the AppDomain. This can be done safely
                    // because we will attach to the AssemblyResolve event but we'll only 
                    // resolve assemblies from a known set of paths or files.
                    var set = new PermissionSet(PermissionState.None);
                    set.AddPermission(new SecurityPermission(SecurityPermissionFlag.ControlAppDomain));

                    // Request the permission to connect to the AppDomain.
                    // No need for a try..finally because the assert is removed as soon as we reach
                    // the CodeAccessPermission.RevertAssert() or until the stack unwinds, which
                    // ever comes first
                    set.Assert();
                    domain.AssemblyResolve += helper.LocateAssemblyOnAssemblyLoadFailure;
                    CodeAccessPermission.RevertAssert();
                }
            }
        }
    }
}