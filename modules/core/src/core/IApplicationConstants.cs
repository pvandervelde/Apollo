//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apollo.Core
{
    /// <summary>
    /// Defines the interface for the application wide constant value storage.
    /// </summary>
    public interface IApplicationConstants
    {
        /// <summary>
        /// Gets the delay that is used between consequitive updates in user interface progress bars
        /// and other controls.
        /// </summary>
        /// <value>The update delay.</value>
        TimeSpan UserInterfaceUpdateDelay
        {
            get;
        }
    }
}
