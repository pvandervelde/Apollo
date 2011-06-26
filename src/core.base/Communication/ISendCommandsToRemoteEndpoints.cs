﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Defines the interface for objects that handle sending commands to remote endpoints.
    /// </summary>
    public interface ISendCommandsToRemoteEndpoints
    {
        /// <summary>
        /// Returns a collection describing all the known endpoints and the commands they
        /// provide.
        /// </summary>
        /// <returns>
        /// The collection describing all the known endpoints and the commands they describe.
        /// </returns>
        IEnumerable<Tuple<EndpointId, IEnumerable<Type>>> AvailableCommands();

        /// <summary>
        /// Returns a collection describing all the known commands for the given endpoint.
        /// </summary>
        /// <param name="endpoint">The ID number of the endpoint.</param>
        /// <returns>
        ///     The collection describing all the known commands for the given endpoint.
        /// </returns>
        IEnumerable<Type> AvailableCommandsFor(EndpointId endpoint);

        /// <summary>
        /// An event raised when an endpoint signs on and provides a set of commands.
        /// </summary>
        event EventHandler<CommandSetAvailabilityEventArgs> OnEndpointSignedIn;

        /// <summary>
        /// An event raised when an endpoint signs off.
        /// </summary>
        event EventHandler<EndpointEventArgs> OnEndpointSignedOff;

        /// <summary>
        /// Returns a value indicating if there are any known commands for a given endpoint.
        /// </summary>
        /// <param name="endpoint">The ID number of the endpoint.</param>
        /// <returns>
        ///     <see langword="true" /> if there are known commands for the given endpoint; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool HasCommandsFor(EndpointId endpoint);

        /// <summary>
        /// Returns a value indicating if a specific set of commands is available for the given endpoint.
        /// </summary>
        /// <param name="endpoint">The ID number of the endpoint.</param>
        /// <param name="commandInterfaceType">The type of the command that should be available.</param>
        /// <returns>
        ///     <see langword="true" /> if there are the speicfic commands exist for the given endpoint; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool HasCommandFor(EndpointId endpoint, Type commandInterfaceType);

        /// <summary>
        /// Returns the command proxy for the given endpoint.
        /// </summary>
        /// <typeparam name="TCommand">The typeof command set that should be returned.</typeparam>
        /// <param name="endpoint">The ID number of the endpoint for which the commands should be returned.</param>
        /// <returns>The requested command set.</returns>
        TCommand CommandsFor<TCommand>(EndpointId endpoint) where TCommand : class, ICommandSet;

        /// <summary>
        /// Returns the command proxy for the given endpoint.
        /// </summary>
        /// <param name="endpoint">The ID number of the endpoint for which the commands should be returned.</param>
        /// <param name="commandType">The type of the command.</param>
        /// <returns>The requested command set.</returns>
        ICommandSet CommandsFor(EndpointId endpoint, Type commandType);

        /// <summary>
        /// Closes the connection to the given endpoint.
        /// </summary>
        /// <remarks>
        /// Closing the connection to a given endpoint also invalidates
        /// all commandsets for that endpoint.
        /// </remarks>
        /// <param name="endpoint">The ID of the endpoint with which the connection should be closed.</param>
        void CloseConnectionTo(EndpointId endpoint);

        /// <summary>
        /// Closes all connections.
        /// </summary>
        void CloseConnections();
    }
}
