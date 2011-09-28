//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Defines the interface for objects that track messages indicating
    /// that a new remote command has been registered.
    /// </summary>
    internal interface IReportNewProxies
    {
        /// <summary>
        /// An event raised when a new remote command is registered.
        /// </summary>
        event EventHandler<ProxyInformationEventArgs> OnNewProxyRegistered;
    }
}
