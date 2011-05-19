//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Utilities
{
    /// <summary>
    /// Defines an empty <see cref="IPersistenceInformation"/> object which is used to
    /// create new empty datasets.
    /// </summary>
    [Serializable]
    public sealed class NullPersistenceInformation : IPersistenceInformation
    {
    }
}
