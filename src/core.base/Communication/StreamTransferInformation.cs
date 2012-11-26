//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Core.Base.Properties;
using Lokad;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Defines the methods for storing information about the way a stream should 
    /// be transferred over a stream connection.
    /// </summary>
    [Serializable]
    public abstract class StreamTransferInformation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamTransferInformation"/> class.
        /// </summary>
        /// <param name="type">The type of the channel that is used to transfer the stream.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="type"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="type"/> does not inherit from <see cref="IChannelType"/>.
        /// </exception>
        protected StreamTransferInformation(Type type)
        {
            {
                Enforce.Argument(() => type);
                Enforce.With<ArgumentException>(
                    typeof(IChannelType).IsAssignableFrom(type),
                    Resources.Exceptions_Messages_AChannelTypeMustDeriveFromIChannelType);
            }

            ChannelType = type;
        }

        /// <summary>
        /// Gets the type of channel on which the transfer should take place.
        /// </summary>
        public Type ChannelType
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets a value indicating the position in the file stream from where the
        /// transfer should start.
        /// </summary>
        public long StartPosition
        {
            get;
            set;
        }
    }
}
