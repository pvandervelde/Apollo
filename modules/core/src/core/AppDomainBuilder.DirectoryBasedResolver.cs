//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lokad;
using Apollo.Utils.Fusion;

namespace Apollo.Core
{ 
    internal sealed partial class AppDomainBuilder
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
            private IList<string> m_Directories;

            /// <summary>
            /// Stores the paths to the relevant directories.
            /// </summary>
            /// <param name="directoryPaths">
            ///     The paths to the relevant directories
            /// </param>
            public void StoreDirectoryPaths(IEnumerable<DirectoryInfo> directoryPaths)
            {
                {
                    Enforce.Argument(() => directoryPaths);
                }

                var paths = from path in directoryPaths
                            select path.FullName;
                m_Directories = new List<string>(paths);
            }

            /// <summary>
            /// Attaches the assembly resolution method to the <see cref="AppDomain.AssemblyResolve"/>
            /// event.
            /// </summary>
            public void Attach()
            {
                {
                    Enforce.NotNull(() => m_Directories);
                }

                var domain = AppDomain.CurrentDomain;

                var helper = new FusionHelper();
                helper.FileEnumerator = () => m_Directories.SelectMany((dir) => Directory.GetFiles(dir, FileExtensions.AssemblyExtension, SearchOption.AllDirectories));

                domain.AssemblyResolve += new ResolveEventHandler(helper.LocateAssemblyOnAssemblyLoadFailure);
            }
        }
    }
}