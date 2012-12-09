//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Apollo.Core.Base.Scheduling
{
    /// <summary>
    /// Defines the interface for objects that implements a condition for a schedule.
    /// </summary>
    public interface IScheduleCondition
    {
        /// <summary>
        /// Returns a value indicating if the schedule edge can be traversed.
        /// </summary>
        /// <param name="token">The cancellation token that is used to cancel the evaluation of the condition.</param>
        /// <returns>
        /// <see langword="true" /> if the edge can be traversed; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool CanTraverse(CancellationToken token);
    }
}
