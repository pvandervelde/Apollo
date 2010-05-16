//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Apollo.Utils.Commands;

namespace Apollo.Core
{
    /// <summary>
    /// Defines an <see cref="ICommandContext"/> for the <see cref="CheckApplicationCanShutdownCommand"/>.
    /// </summary>
    internal sealed class CheckApplicationCanShutdownContext : ICommandContext
    {
        /// <summary>
        /// Gets or sets a value indicating whether the application can be shutdown or not.
        /// </summary>
        /// <value>
        /// The command result.
        /// </value>
        public bool Result
        {
            get;
            set;
        }
    }
}
