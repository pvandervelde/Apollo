//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core
{
    /// <summary>
    /// An attribute which is placed on <see cref="KernelService"/> classes to indicate
    /// which security permissions they require.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ServiceSecurityLevelAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceSecurityLevelAttribute"/> class.
        /// </summary>
        /// <param name="level">The requested security level.</param>
        public ServiceSecurityLevelAttribute(SecurityLevel level)
        {
            SecurityLevel = level;
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