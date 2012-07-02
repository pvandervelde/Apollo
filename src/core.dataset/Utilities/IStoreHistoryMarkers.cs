//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apollo.Utilities.History;

namespace Apollo.Core.Dataset.Utilities
{
    internal interface IStoreHistoryMarkers
    {
        void Add(TimeMarker marker);
    }
}
