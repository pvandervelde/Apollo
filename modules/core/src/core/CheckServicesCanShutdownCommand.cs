//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Apollo.Core.Messaging;
using Apollo.Utils;
using Apollo.Utils.Commands;
using Lokad;
using ICommand = Apollo.Utils.Commands.ICommand;

namespace Apollo.Core
{
    /// <summary>
    /// Defines a command that checks if a specific set of services can be shut down.
    /// </summary>
    public sealed class CheckServicesCanShutdownCommand : ICommand
    {
        #region Static members

        /// <summary>
        /// Defines the Id for the <c>CheckServicesCanShutdownCommand</c>.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "A CommandId reference is immutable")]
        public static readonly CommandId CommandId = new CommandId(@"CheckServicesCanShutdown");

        #endregion

        /// <summary>
        /// The delegate used to send a message for which a response is expected.
        /// </summary>
        private readonly SendMessageWithResponse m_MessageSender;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckServicesCanShutdownCommand"/> class.
        /// </summary>
        /// <param name="messageSender">The function used to send a message.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="messageSender"/> is <see langword="null"/>.
        /// </exception>
        internal CheckServicesCanShutdownCommand(SendMessageWithResponse messageSender)
        {
            {
                Enforce.Argument(() => messageSender);
            }

            m_MessageSender = messageSender;
        }

        #region Implementation of ICommand

        /// <summary>
        /// Gets the ID.
        /// </summary>
        /// <value>The ID for the command.</value>
        public CommandId Id
        {
            get
            {
                return CommandId;
            }
        }

        /// <summary>
        /// Invokes the current command with the specified context as input.
        /// </summary>
        /// <param name="context">The context for the command.</param>
        public void Invoke(ICommandContext context)
        {
            var commandContext = context as CheckCanServicesShutdownContext;
            Debug.Assert(commandContext != null, "Incorrect command context provided.");

            // Send the messages. We want to do this actively (not lazily)
            // so we use a good old foreach loop.
            var responses = new List<IFuture<MessageBody>>();
            foreach (var name in commandContext.Input)
            {
                var future = m_MessageSender(name, new ServiceShutdownCapabilityRequestMessage(), MessageId.None);
                responses.Add(future);
            }

            // Technically we could do an even better job here. Currently we just iterate over the collection
            // and wait when we need to. We could iterate over the collection and check for each future
            // if the results are in already. If so we use them, if not then we move on and come back
            // later.
            // On the other hand the maximum wait is always equal to the time taken by the slowest
            // service. So it shouldn't matter too much in which order we're going to get the results.
            commandContext.Result = responses.Aggregate(
                true,
                (current, future) =>
                {
                    var body = future.Result() as ServiceShutdownCapabilityResponseMessage;
                    return body != null ? current && body.CanShutdown : current;
                });
        }

        #endregion
    }
}
