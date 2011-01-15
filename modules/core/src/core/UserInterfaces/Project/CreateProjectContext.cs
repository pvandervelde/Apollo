﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Apollo.Core.Projects;
using Apollo.Utils.Commands;

namespace Apollo.Core.UserInterfaces.Project
{
    /// <summary>
    /// Defines an <see cref="ICommandContext"/> for the <see cref="CreateProjectCommand"/>.
    /// </summary>
    internal sealed class CreateProjectContext : ICommandContext
    {
        /// <summary>
        /// Gets or sets a value indicating the <see cref="IProject"/>.
        /// </summary>
        /// <value>
        /// The command result.
        /// </value>
        public IProject Result
        {
            get;
            set;
        }
    }
}
