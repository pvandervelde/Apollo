//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Test.Manual.Console
{
    /// <summary>
    /// Defines the interface for a window that allows the user to interact
    /// with the application.
    /// </summary>
    internal interface IInteractiveWindow
    {
        /// <summary>
        /// Shows the window in a modeless way.
        /// </summary>
        void Show();
    }
}
