//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core.Extensions.Scheduling
{
    /// <summary>
    /// A vertex for the <see cref="IEditableSchedule"/> which indicates that the schedule graph starts at this position.
    /// </summary>
    /// <remarks>
    /// All editable schedule vertices should be immutable because a schedule is copied
    /// by reusing the vertices.
    /// </remarks>
    [Serializable]
    public sealed class EditableStartVertex : IEditableScheduleVertex
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EditableStartVertex"/> class.
        /// </summary>
        internal EditableStartVertex()
        { 
        }
    }
}
