//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Utilities
{
    /// <summary>
    /// A progress mark that indicates that it doesn't know which mark
    /// the progress is at.
    /// </summary>
    [Serializable]
    internal sealed class IndeterminateProgressMark : IProgressMark
    {
    }
}
