//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;

namespace Apollo.Core.Extensions.Scheduling
{
    /// <summary>
    /// Defines the interface for objects that describe an action that is stored 
    /// in a schedule.
    /// </summary>
    public interface IScheduleAction
    {
        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="token">The token that is used to cancel the action.</param>
        void Execute(CancellationToken token);
    }
}
