//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Utilities.History
{
    /// <summary>
    /// Indicates if a dependency can block roll-back, roll-forward, both or neither.
    /// </summary>
    public enum ChangeBlocker
    {
        /// <summary>
        /// The dependency will block neither roll-back or roll-forward.
        /// </summary>
        None,

        /// <summary>
        /// The dependency can block roll-back but not roll-forward.
        /// </summary>
        RollBack,

        /// <summary>
        /// The dependency can block roll-forward but not roll-back.
        /// </summary>
        RollForward,

        /// <summary>
        /// The dependency can block both roll-back and roll-foward.
        /// </summary>
        RollBackAndRollForward,
    }
}
