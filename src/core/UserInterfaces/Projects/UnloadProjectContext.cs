//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Threading.Tasks;
using Apollo.Utilities.Commands;

namespace Apollo.Core.UserInterfaces.Projects
{
    /// <summary>
    /// Defines an <see cref="ICommandContext"/> for the <see cref="UnloadProjectCommand"/>.
    /// </summary>
    internal sealed class UnloadProjectContext : ICommandContext
    {
        /// <summary>
        /// Gets or sets the <see cref="Task"/> that is currently handling the
        /// shutdown of the project.
        /// </summary>
        public Task Result
        {
            get;
            set;
        }
    }
}
