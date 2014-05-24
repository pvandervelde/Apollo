//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Core.Base.Activation
{
    /// <summary>
    /// Stores a selected distribution plan and indicates if the
    /// selection process was canceled at some point.
    /// </summary>
    public sealed class SelectedProposal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectedProposal"/> class.
        /// </summary>
        public SelectedProposal()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectedProposal"/> class.
        /// </summary>
        /// <param name="plan">The selected plan.</param>
        public SelectedProposal(DistributionPlan plan)
        {
            WasSelectionCanceled = plan == null;
            Plan = plan;
        }

        /// <summary>
        /// Gets a value indicating whether the selection process
        /// was canceled at any point.
        /// </summary>
        public bool WasSelectionCanceled
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the selected plan.
        /// </summary>
        public DistributionPlan Plan
        {
            get;
            private set;
        }
    }
}
