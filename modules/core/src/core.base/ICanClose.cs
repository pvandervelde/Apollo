//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Core.Base
{
    /// <summary>
    /// Defines the interface for objects that can be closed.
    /// </summary>
    public interface ICanClose
    {
        /// <summary>
        /// Closes the current object.
        /// </summary>
        void Close();
    }
}
