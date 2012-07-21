//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;

namespace Apollo.Core.Extensions.Scheduling
{
    /// <summary>
    /// Defines the interface for objects that verify the integrity of a schedule.
    /// </summary>
    public interface IVerifyScheduleIntegrity
    {
        /// <summary>
        /// Determines if the given schedule is a valid schedule.
        /// </summary>
        /// <param name="id">The ID of the schedule.</param>
        /// <param name="schedule">The schedule that should be verified.</param>
        /// <param name="onValidationFailure">The action which is invoked for each validation failure.</param>
        /// <returns>
        /// <see langword="true"/> if the schedule is valid; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool IsValid(ScheduleId id, IEditableSchedule schedule, Action<ScheduleIntegrityFailureType, IEditableScheduleVertex> onValidationFailure);
    }
}
