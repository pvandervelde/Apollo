//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Utils;

namespace Apollo.Core
{
    /// <summary>
    /// An attribute which is placed on <see cref="KernelService"/> classes to indicate
    /// which security permissions they require.
    /// </summary>
    [ExcludeFromCoverage("Attributes do not need to be tested")]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ServiceSecurityLevelAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceSecurityLevelAttribute"/> class.
        /// </summary>
        /// <param name="securityLevel">The requested security level.</param>
        public ServiceSecurityLevelAttribute(SecurityLevel securityLevel)
        {
            SecurityLevel = securityLevel;
        }

        /// <summary>
        /// Gets the requested security level.
        /// </summary>
        /// <value>The security level.</value>
        public SecurityLevel SecurityLevel
        {
            get;
            private set;
        }
    }
}