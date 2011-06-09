﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Windows.Input;
using Apollo.UI.Common.Properties;

namespace Apollo.UI.Common.Views.Projects
{
    /// <summary>
    /// Defines the viewmodel for the project.
    /// </summary>
    public sealed class ProjectModel : Model
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectModel"/> class.
        /// </summary>
        /// <param name="closeCommand">The command that closes the current project.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="closeCommand"/> is <see langword="null" />.
        /// </exception>
        public ProjectModel(ICommand closeCommand)
        {
            {
                Lokad.Enforce.Argument(() => closeCommand);
            }

            CloseCommand = closeCommand;
        }

        /// <summary>
        /// Gets the name of the model for uses on a display.
        /// </summary>
        public string DisplayName
        {
            get 
            {
                return Resources.ProjectView_ViewName;
            }
        }

        /// <summary>
        /// Gets the command that can be used to close the current model
        /// and all related views.
        /// </summary>
        public ICommand CloseCommand
        {
            get;
            private set;
        }
    }
}
