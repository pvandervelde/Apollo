//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Core.Dataset.Plugins
{
    /// <summary>
    /// Stores information about the changes that were applied to a part instance.
    /// </summary>
    internal sealed class InstanceUpdate
    {
        /// <summary>
        /// Gets or sets the ID of the part instance that was changed.
        /// </summary>
        public PartInstanceId Instance
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the change that was applied to the instance.
        /// </summary>
        public InstanceChange Change
        {
            get;
            set;
        }
    }
}
