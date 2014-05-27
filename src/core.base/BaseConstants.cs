//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Nuclei.Diagnostics.Profiling;

namespace Apollo.Core.Base
{
    /// <summary>
    /// Defines the constants for the Apollo.Core.Base assembly.
    /// </summary>
    public static class BaseConstants
    {
        /// <summary>
        /// The prefix used for each log message.
        /// </summary>
        internal const string LogPrefix = "Apollo.Core.Base";

        /// <summary>
        /// The timing group that will be used for the timing of all UI actions.
        /// </summary>
        private static readonly TimingGroup s_TimingGroup
            = new TimingGroup();

        /// <summary>
        /// Gets the timing group that is used to identify all the timings done in the UI actions.
        /// </summary>
        public static TimingGroup TimingGroup
        {
            get
            {
                return s_TimingGroup;
            }
        }
    }
}
