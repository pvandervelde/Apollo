//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core
{
    /// <summary>
    /// Defines an attribute that is placed on <c>KernelService</c> classes to indicate
    /// which private bin path should be provided for the service.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    internal sealed class PrivateBinPathRequirementsAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrivateBinPathRequirementsAttribute"/> class.
        /// </summary>
        /// <param name="option">
        /// The option that describes which private bin path
        /// should be provided for the service on which this attribute is placed.
        /// </param>
        public PrivateBinPathRequirementsAttribute(PrivateBinPathOption option)
        {
            Option = option;
        }

        /// <summary>
        /// Gets the option that describes which private bin path
        /// should be provided for the service on which this attribute is placed.
        /// </summary>
        /// <value>The option.</value>
        public PrivateBinPathOption Option 
        { 
            get;
            private set;
        }
    }
}
