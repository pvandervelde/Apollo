//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Utilities
{
    /// <summary>
    /// Defines a set of constants that apply to the entire application.
    /// </summary>
    public interface IApplicationConstants : ICompanyConstants
    {
        /// <summary>
        /// Gets the name of the application.
        /// </summary>
        /// <value>The name of the application.</value>
        string ApplicationName 
        {
            get; 
        }

        /// <summary>
        /// Gets the application version.
        /// </summary>
        /// <value>The application version.</value>
        Version ApplicationVersion
        {
            get;
        }

        /// <summary>
        /// Gets the application version with which this application is compatible.
        /// </summary>
        /// <remarks>
        /// A compatible application version indicates that the current version reads the
        /// configuration files of the compatible application.
        /// </remarks>
        /// <value>The application compatibility version.</value>
        Version ApplicationCompatibilityVersion
        {
            get;
        }
    }
}