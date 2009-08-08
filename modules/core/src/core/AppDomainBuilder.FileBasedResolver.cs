//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Apollo.Utils.Fusion;
using Lokad;

namespace Apollo.Core
{ 
    internal sealed partial class AppDomainBuilder
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
            private IList<string> m_Files;

            /// <summary>
            /// Stores the paths to the relevant assemblies.
            /// </summary>
            /// <param name="filePaths">
            ///     The paths to the relevant assemblies
            /// </param>
            public void StoreFilePaths(IEnumerable<FileInfo> filePaths)
            {
                {
                    Enforce.Argument(() => filePaths); 
                }

                var paths = from path in filePaths
                            select path.FullName;
                m_Files = new List<string>(paths);
            }

            /// <summary>
            /// Attaches the assembly resolution method to the <see cref="AppDomain.AssemblyResolve"/>
            /// event.
            /// </summary>
            public void Attach()
            {
                {
                    Enforce.NotNull(() => m_Files); 
                }

                var domain = AppDomain.CurrentDomain;

                var helper = new FusionHelper();
                helper.FileEnumerator = () => m_Files;

                domain.AssemblyResolve += new ResolveEventHandler(helper.LocateAssemblyOnAssemblyLoadFailure);
            }
        }
    }
}