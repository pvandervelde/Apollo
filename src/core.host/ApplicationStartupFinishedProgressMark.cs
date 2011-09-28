//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Utilities;

namespace Apollo.Core.Host
{
    /// <summary>
    /// Defines an <see cref="IProgressMark"/> which indicates that the application core 
    /// has finished the startup process.
    /// </summary>
    [Serializable]
    public sealed class ApplicationStartupFinishedProgressMark : IProgressMark
    {
    }
}
