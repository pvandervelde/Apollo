//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Utilities.Licensing
{
    /// <summary>
    /// Defines the interface for objects that run validations of the license
    /// over a sequence of time.
    /// </summary>
    internal interface ITimeBasedValidator
    {
        /// <summary>
        /// Starts the validation.
        /// </summary>
        void StartValidation();
    }
}
