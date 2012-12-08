//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apollo.Core.Host.Plugins
{
    internal sealed class PluginService : KernelService
    {
        /// <summary>
        /// Provides derivative classes with a possibility to
        /// perform startup tasks.
        /// </summary>
        protected override void StartService()
        {
            // Do nothing here. Derrivatives may override and implement this method.
        }

        /// <summary>
        /// Provides derivative classes with a possibility to
        /// perform shutdown tasks.
        /// </summary>
        protected override void StopService()
        {
            // Do nothing here. Derrivatives may override and implement this method.
        }
    }
}
