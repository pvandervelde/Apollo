//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Runtime.Remoting;
using Apollo.Core.Projects;
using Apollo.Utils.Commands;

namespace Apollo.Core.UserInterfaces.Project
{
    /// <summary>
    /// Defines an <see cref="ICommandContext"/> for the <see cref="CreateProjectCommand"/>.
    /// </summary>
    public sealed class CreateProjectContext : ICommandContext
    {
        /// <summary>
        /// Gets or sets a value indicating the <see cref="ObjRef"/> object
        /// that can be used to create the proxy to the <see cref="IProject"/>.
        /// </summary>
        /// <value>
        /// The command result.
        /// </value>
        public ObjRef Result
        {
            get;
            set;
        }
    }
}
