﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Utilities;

namespace Apollo.Core
{
    /// <summary>
    /// Defines an <see cref="IProgressMark"/> which indicates that the application is starting.
    /// </summary>
    [Serializable]
    public sealed class ApplicationStartingProgressMark : IProgressMark
    {
    }
}
