//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Threading.Tasks;
using Apollo.Core.Projects;
using Apollo.Utilities.Commands;

namespace Apollo.Core.UserInterfaces.Projects
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
        public Task<IProject> Result
        {
            get;
            set;
        }
    }
}
