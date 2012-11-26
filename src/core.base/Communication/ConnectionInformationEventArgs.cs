//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Lokad;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// An <see cref="EventArgs"/> class that stores information about a connection that can
    /// be made to a remote application.
    /// </summary>
    public sealed class ConnectionInformationEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionInformationEventArgs"/> class.
        /// </summary>
        /// <param name="connectionInfo">
        /// The connection information.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="connectionInfo"/> is <see langword="null" />.
        /// </exception>
        public ConnectionInformationEventArgs(ChannelConnectionInformation connectionInfo)
        {
            {
                Enforce.Argument(() => connectionInfo);
            }

            ConnectionInformation = connectionInfo;
        }

        /// <summary>
        /// Gets a value providing the information necessary to connect to a given endpoint.
        /// </summary>
        public ChannelConnectionInformation ConnectionInformation
        {
            get;
            private set;
        }
    }
}
