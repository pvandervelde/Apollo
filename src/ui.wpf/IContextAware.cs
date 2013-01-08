//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.UI.Wpf
{
    /// <summary>
    /// Defines the interface for objects that are context aware and need to perform
    /// certain actions on a specific thread.
    /// </summary>
    public interface IContextAware
    {
        /// <summary>
        /// Gets a value indicating whether the current methods are executing in
        /// a synchronized context or not.
        /// </summary>
        bool IsSynchronized
        {
            get;
        }

        /// <summary>
        /// Invokes the given action in the correct context.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        void Invoke(Action action);
    }
}
