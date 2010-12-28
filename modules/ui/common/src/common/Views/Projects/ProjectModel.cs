//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Core.UserInterfaces.Project;
using Lokad;

namespace Apollo.UI.Common.Views.Projects
{
    /// <summary>
    /// Defines the viewmodel for the project.
    /// </summary>
    public sealed class ProjectModel : Model
    {
        /// <summary>
        /// The project that holds the data.
        /// </summary>
        private readonly ILinkToProjects m_Project;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectModel"/> class.
        /// </summary>
        /// <param name="facade">The project that holds all the data.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="facade"/> is <see langword="null" />.
        /// </exception>
        public ProjectModel(ILinkToProjects facade)
        {
            {
                Enforce.Argument(() => facade);
            }

            m_Project = facade;
        }
    }
}
