//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core.Extensions.Scheduling
{
    /// <summary>
    /// A vertex for the <see cref="IEditableSchedule"/> which marks the position where a set of variables
    /// should be synchronized.
    /// </summary>
    [Serializable]
    public sealed class EditableSynchronizationEndVertex : IEditableScheduleVertex
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EditableSynchronizationEndVertex"/> class.
        /// </summary>
        internal EditableSynchronizationEndVertex()
        { 
        }
    }
}
