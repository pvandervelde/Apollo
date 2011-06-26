//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Apollo.Core.Base.Communication;
using Lokad;

namespace Apollo.Core.Base
{
    /// <summary>
    /// Stores non-persistent information about a dataset. This includes information
    /// about the communication channels that can be used to communicate with the dataset
    /// and information about the commands that the dataset provides.
    /// </summary>
    public sealed class DatasetOnlineInformation
    {
        /// <summary>
        /// The object that handles sending commands to the remote endpoints.
        /// </summary>
        private readonly ISendCommandsToRemoteEndpoints m_Hub;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetOnlineInformation"/> class.
        /// </summary>
        /// <param name="id">The ID number of the dataset.</param>
        /// <param name="endpoint">The ID number of the endpoint that has the actual dataset loaded.</param>
        /// <param name="hub">The object that handles sending commands to the remote endpoint.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="id"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="endpoint"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="hub"/> is <see langword="null" />.
        /// </exception>
        public DatasetOnlineInformation(
            DatasetId id, 
            EndpointId endpoint,
            ISendCommandsToRemoteEndpoints hub)
        {
            {
                Enforce.Argument(() => id);
                Enforce.Argument(() => endpoint);
                Enforce.Argument(() => hub);
            }

            Id = id;
            Endpoint = endpoint;
            m_Hub = hub;
        }

        /// <summary>
        /// Gets the ID number of the dataset for 
        /// which the non-persistence information is stored.
        /// </summary>
        public DatasetId Id
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the ID number of the endpoint which has the actual
        /// dataset loaded.
        /// </summary>
        public EndpointId Endpoint
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns a collection containing all the commands that the dataset has
        /// defined.
        /// </summary>
        /// <returns>
        ///     The collection with all the commands that the dataset has defined.
        /// </returns>
        public IEnumerable<ICommandSet> AvailableCommands()
        {
            return from commandType in m_Hub.AvailableCommandsFor(Endpoint)
                   where (commandType.GetCustomAttributes(typeof(InternalCommandAttribute), true).Length == 0)
                   select m_Hub.CommandsFor(Endpoint, commandType);
        }

        /// <summary>
        /// Returns the command of the given type.
        /// </summary>
        /// <typeparam name="TCommand">The type of the desired command.</typeparam>
        /// <returns>
        ///     The command of the given type or <see langword="null" /> if that command doesn't exist.
        /// </returns>
        public TCommand Command<TCommand>() where TCommand : class, ICommandSet
        {
            return m_Hub.CommandsFor<TCommand>(Endpoint);
        }

        /// <summary>
        /// Closes the remote dataset application.
        /// </summary>
        public void Close()
        {
            Debug.Assert(m_Hub.HasCommandFor(Endpoint, typeof(IDatasetApplicationCommands)), "Missing essential command set.");
            var commands = m_Hub.CommandsFor<IDatasetApplicationCommands>(Endpoint);
            var result = commands.Close();
            result.Wait();
        }
    }
}
