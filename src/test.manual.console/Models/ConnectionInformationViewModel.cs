//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Apollo.Core.Base.Communication;

namespace Test.Manual.Console.Models
{
    /// <summary>
    /// Stores information about an endpoint.
    /// </summary>
    internal sealed class ConnectionInformationViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionInformationViewModel"/> class.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="endpointAddress">The address of the endpoint.</param>
        /// <param name="connection">The type of binding that is used to connect to the endpoint.</param>
        public ConnectionInformationViewModel(EndpointId endpoint, string endpointAddress, string connection)
        {
            Id = endpoint;
            Address = endpointAddress;
            ConnectionType = connection;
        }

        /// <summary>
        /// Gets the ID number of the current endpoint.
        /// </summary>
        public EndpointId Id
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the URL of the current endpoint.
        /// </summary>
        public string Address
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the binding type for the current endpoint.
        /// </summary>
        public string ConnectionType
        {
            get;
            private set;
        }
    }
}
