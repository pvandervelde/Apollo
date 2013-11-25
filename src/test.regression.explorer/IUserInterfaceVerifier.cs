//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Test.UI.Explorer
{
    /// <summary>
    /// Defines the interface for objects that provide verification of one or more parts of the user interface.
    /// </summary>
    internal interface IUserInterfaceVerifier
    {
        /// <summary>
        /// Verifies a part of the user interface.
        /// </summary>
        /// <param name="testLog">The log object used for the current test.</param>
        void Verify(Log testLog);
    }
}
