//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apollo.Core.Base.Communication
{
    internal sealed class CommunicationConstants : ICommunicationConstants
    {
        /// <summary>
        /// Gets the time span used for time-out values in commands
        /// that should only take a very short time (i.e. command execution
        /// time is neglible compared to communication time).
        /// </summary>
        public TimeSpan ShortCommandTimeout
        {
            get
            {
                return new TimeSpan(0, 0, 30);
            }
        }
    }
}
