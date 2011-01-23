//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Apollo.Core.Messaging
{
    /// <summary>
    /// The interface for objects which use the <see cref="MessagePipeline"/> to send messages
    /// to the services running in the kernel.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces",
        Justification = "The use of an attribute over an interface destroys the symmetry between sending and receiving interfaces.")]
    public interface ISendMessages : IDnsNameObject
    {
    }
}
