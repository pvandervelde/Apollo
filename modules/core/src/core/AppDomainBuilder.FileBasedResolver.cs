//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;
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
        private sealed class FileBasedResolver : MarshalByRefObject, IAppDomainAssemblyResolver
        {
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
            /// <param name="filePaths">
            ///     The paths to the relevant assemblies.
            /// </param>
            /// <exception cref="ArgumentNullException">
            /// Thrown when <paramref name="filePaths"/> is <see langword="null" />.
            /// </exception>
            public void StoreFilePaths(IEnumerable<string> filePaths)
            {
                {
                    Enforce.Argument(() => filePaths); 
                }

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
            [SecurityCritical]
            [SecurityTreatAsSafe]
            public void Attach()
            {
                {
                    Enforce.NotNull(() => m_Files);
                }

                var domain = AppDomain.CurrentDomain;
                {
                    var helper = new FusionHelper(() => m_Files);

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