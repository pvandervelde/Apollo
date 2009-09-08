using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apollo.Core.Messages;

namespace Apollo.Core
{
    /// <summary>
    /// Defines the interface for objects which have a unique <see cref=" DnsName"/> with 
    /// which they can be identified.
    /// </summary>
    public interface IDnsNameObject
    {
        /// <summary>
        /// Gets the identifier of the object.
        /// </summary>
        /// <value>The identifier.</value>
        DnsName Name { get; }
    }
}
