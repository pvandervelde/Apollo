//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;

namespace Apollo.Utilities
{
    /// <summary>
    /// Defines a set of utility methods for working with files.
    /// </summary>
    public static class FileUtilities
    {
        /// <summary>
        /// Determines the relative path for the given path and basePath in a naive way.
        /// </summary>
        /// <remarks>
        /// The 'algorithm' used to determine the relative path is very simple and can only
        /// handle cases where the base path is part of the given path, e.g. if the path is
        /// c:\temp\myfile.txt then the base path can either be c:\ or c:\temp but not
        /// c:\someotherdir.
        /// </remarks>
        /// <param name="path">The path of the file.</param>
        /// <param name="basePath">The base path from where the relative path is calculated.</param>
        /// <returns>The relative path of file located at <paramref name="path"/>.</returns>
        public static string GetRelativePath(string path, string basePath)
        {
            int index = path.IndexOf(basePath, StringComparison.OrdinalIgnoreCase);
            return path.Substring(index + basePath.Length).TrimStart(Path.DirectorySeparatorChar);
        }
    }
}
