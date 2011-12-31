//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apollo.Utilities.History
{
    /// <summary>
    /// Defines the interface for objects that track the history of a collection of other 
    /// objects.
    /// </summary>
    public interface ITimeline : IConnectObjectsToHistory, IFollowHistory, ITrackHistoryChanges
    {
    }
}
