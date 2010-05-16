//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Utils.Commands;

namespace Apollo.Core
{
    /// <summary>
    /// Defines an <see cref="ICommandContext"/> for the <see cref="ShutdownApplicationCommand"/>.
    /// </summary>
    public sealed class ShutdownApplicationContext : ICommandContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShutdownApplicationContext"/> class.
        /// </summary>
        /// <param name="isForced">If set to <see langword="true"/> then a forced shutdown is requested.</param>
        public ShutdownApplicationContext(bool isForced)
        {
            IsShutdownForced = isForced;
        }

        /// <summary>
        /// Gets a value indicating whether this shutdown is forced.
        /// </summary>
        /// <value>
        ///     <see langword="true"/> if this shutdown is forced; otherwise, <see langword="false"/>.
        /// </value>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool IsShutdownForced
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the application is shutting down or not.
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
