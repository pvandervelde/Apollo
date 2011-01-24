//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Core.Base.Projects
{
    /// <summary>
    /// Defines who requested the creation of a specific dataset.
    /// </summary>
    public enum DatasetCreator
    {
        /// <summary>
        /// The creation of the dataset was not requested by anybody, it just 
        /// appeared. Normally this would be an error condition.
        /// </summary>
        None,

        /// <summary>
        /// The creation of the dataset was requested by the system, i.e. another
        /// dataset or the project.
        /// </summary>
        System,

        /// <summary>
        /// The creation of the dataset was requested by the user.
        /// </summary>
        User,
    }
}
