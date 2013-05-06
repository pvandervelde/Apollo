//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.UI.Explorer.Nuclei
{
    /// <summary>
    /// Defines the interface for classes which deal with assembly resolution.
    /// </summary>
    internal interface IAppDomainAssemblyResolver
    {
        /// <summary>
        /// Attaches the assembly resolution method to the <see cref="AppDomain.AssemblyResolve"/>
        /// event.
        /// </summary>
        void Attach();
    }
}
