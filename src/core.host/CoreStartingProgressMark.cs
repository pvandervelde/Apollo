//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Utilities.Progress;

namespace Apollo.Core.Host
{
    /// <summary>
    /// Defines an <see cref="IProgressMark"/> which indicates that the application core is starting.
    /// </summary>
    [Serializable]
    public sealed class CoreStartingProgressMark : IProgressMark
    {
    }
}
