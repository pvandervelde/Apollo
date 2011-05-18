//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Defines an attribute that indicates what the relative performance of a <see cref="IChannelType"/> is 
    /// versus other channel types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    [ExcludeFromCodeCoverage()]
    internal sealed class ChannelRelativePerformanceAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelRelativePerformanceAttribute"/> class.
        /// </summary>
        /// <param name="relativePerformanceOrder">
        /// The relative order of the current <see cref="IChannelType"/> versus other channel types. Lower
        /// numbers indicate better performance. Lowest allowable value is 1.
        /// </param>
        public ChannelRelativePerformanceAttribute(int relativePerformanceOrder)
        {
            {
                Debug.Assert(relativePerformanceOrder > 0, "The performance order must be larger than zero.");
            }

            RelativeOrder = relativePerformanceOrder;
        }

        /// <summary>
        /// Gets the relative order of the current <see cref="IChannelType"/> versus
        /// other channel types. Lower number indicate better performance.
        /// </summary>
        public int RelativeOrder
        {
            get;
            private set;
        }
    }
}
