//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core.Base.Projects
{
    /// <summary>
    /// Stores non-persistent information about a dataset. This includes information
    /// about the communication channels that can be used to communicate with the dataset
    /// and information about the commands that the dataset provides.
    /// </summary>
    public sealed class DatasetOnlineInformation
    {
        /// <summary>
        /// Gets a value indicating the ID number of the dataset for 
        /// which the non-persistence information is stored.
        /// </summary>
        public DatasetId Id
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        // Channel

        // Commands
    }
}
