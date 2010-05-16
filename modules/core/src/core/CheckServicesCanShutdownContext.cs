//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Apollo.Core.Messaging;
using Apollo.Utils.Commands;
using Lokad;

namespace Apollo.Core
{
    /// <summary>
    /// Defines an <see cref="ICommandContext"/> for the <see cref="CheckServicesCanShutdownCommand"/>.
    /// </summary>
    public sealed class CheckCanServicesShutdownContext : ICommandContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckCanServicesShutdownContext"/> class.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="input"/> is <see langword="null" />.
        /// </exception>
        public CheckCanServicesShutdownContext(IEnumerable<DnsName> input)
        {
            {
                Enforce.Argument(() => input);
            }

            Input = input;
        }

        /// <summary>
        /// Gets the input for the command.
        /// </summary>
        /// <value>The input for the command.</value>
        public IEnumerable<DnsName> Input
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the services can be shut down.
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
