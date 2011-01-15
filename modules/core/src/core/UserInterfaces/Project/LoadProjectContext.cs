//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Runtime.Remoting;
using Apollo.Core.Projects;
using Apollo.Utils;
using Apollo.Utils.Commands;

namespace Apollo.Core.UserInterfaces.Project
{
    /// <summary>
    /// Defines an <see cref="ICommandContext"/> for the <see cref="LoadProjectCommand"/>.
    /// </summary>
    internal sealed class LoadProjectContext : ICommandContext
    {
        /// <summary>
        /// Gets or sets a value indicating the persistence information
        /// which describes from where the project should be loaded.
        /// </summary>
        public IPersistenceInformation LoadFrom
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating the <see cref="ObjRef"/> object
        /// that can be used to create the proxy to the <see cref="IProject"/>.
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
