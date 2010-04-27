//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Lokad;
using Lokad.Rules;

namespace Apollo.Core
{
    /// <summary>
    /// Holds the base path and assembly resolve paths for an <see cref="AppDomain"/>.
    /// </summary>
    internal sealed class AppDomainResolutionPaths
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppDomainResolutionPaths"/> class based on the
        /// specified set of files.
        /// </summary>
        /// <param name="basePath">The base path for the <c>AppDomain</c> path resolution.</param>
        /// <param name="files">The files that can be resolved.</param>
        /// <returns>A new instance of the <see cref="AppDomainResolutionPaths"/> class.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="basePath"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="basePath"/> is an empty string.
        /// </exception>
        public static AppDomainResolutionPaths WithFiles(string basePath, IEnumerable<string> files)
        {
            return WithFilesAndDirectories(basePath, files, null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppDomainResolutionPaths"/> class based on the
        /// specified set of files and directories.
        /// </summary>
        /// <param name="basePath">The base path for the <c>AppDomain</c> path resolution.</param>
        /// <param name="files">The files that can be resolved.</param>
        /// <param name="directories">The directories in which files can be resolved.</param>
        /// <returns>A new instance of the <see cref="AppDomainResolutionPaths"/> class.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="basePath"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="basePath"/> is an empty string.
        /// </exception>
        public static AppDomainResolutionPaths WithFilesAndDirectories(string basePath, IEnumerable<string> files, IEnumerable<string> directories)
        {
            return new AppDomainResolutionPaths(basePath, files, directories);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppDomainResolutionPaths"/> class.
        /// </summary>
        /// <param name="basePath">The base path for the <c>AppDomain</c> path resolution.</param>
        /// <param name="files">The files that can be resolved.</param>
        /// <param name="directories">The directories in which files can be resolved.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="basePath"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="basePath"/> is an empty string.
        /// </exception>
        private AppDomainResolutionPaths(string basePath, IEnumerable<string> files, IEnumerable<string> directories)
        {
            {
                Enforce.Argument(() => basePath);
                Enforce.Argument(() => basePath, StringIs.NotEmpty);
            }

            BasePath = basePath;
            Files = files;
            Directories = directories;
        }

        /// <summary>
        /// Gets the <see cref="AppDomain"/> base path.
        /// </summary>
        /// <value>The base path for the <see cref="AppDomain"/>.</value>
        public string BasePath
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the files which can be loaded.
        /// </summary>
        /// <value>The collection of files.</value>
        public IEnumerable<string> Files
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the directories from which assemblies can be loaded.
        /// </summary>
        /// <value>The collection directories.</value>
        public IEnumerable<string> Directories
        {
            get;
            private set;
        }
    }
}