//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Policy;
using Lokad;

namespace Apollo.Core
{
    /// <summary>
    /// Holds data that describes the <see cref="PermissionSet"/> and the full trust
    /// assemblies for an <see cref="AppDomain"/>.
    /// </summary>
    internal sealed class AppDomainSandboxData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppDomainSandboxData"/> class.
        /// </summary>
        /// <param name="level">The security level.</param>
        /// <param name="fullTrustAssemblies">The collection of full trust assemblies.</param>
        public AppDomainSandboxData(SecurityLevel level, IEnumerable<StrongName> fullTrustAssemblies)
        {
            {
                Enforce.Argument(() => fullTrustAssemblies);
            }

            Level = level;
            FullTrustAssemblies = fullTrustAssemblies;
        }

        /// <summary>
        /// Gets the security level for the <see cref="AppDomain"/>.
        /// </summary>
        /// <value>The security level.</value>
        public SecurityLevel Level
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the collection of full trust assemblies.
        /// </summary>
        /// <value>The collection of full trust assemblies.</value>
        public IEnumerable<StrongName> FullTrustAssemblies
        {
            get;
            private set;
        }
    }
}
