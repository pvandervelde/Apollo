//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Apollo.Core.Extensions.Scheduling
{
    /// <summary>
    /// Defines the interface for objects that define a variable that is used in a schedule.
    /// </summary>
    /// <remarks>
    /// Note that variables must be serializable because they may be transported to an external data set application.
    /// </remarks>
    [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces",
        Justification = "This interface is waiting implementation.")]
    public interface IScheduleVariable
    {
    }
}
