//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Utilities.History
{
    /// <summary>
    /// Defines the interface for objects which need to provide information which survives a roll-back
    /// or a roll-forward.
    /// </summary>
    public interface IReceiveMessagesFromTheFuture
    {
        /// <summary>
        /// Receives a <see cref="TimelineTraveller"/> which holds information created by the
        /// current object in a future state.
        /// </summary>
        /// <param name="traveller">The traveller.</param>
        void ReceiveTraveller(TimelineTraveller traveller);
    }
}
