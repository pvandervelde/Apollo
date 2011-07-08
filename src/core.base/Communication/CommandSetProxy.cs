﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Forms the base for remote <see cref="ICommandSet"/> proxy objects.
    /// </summary>
    /// <remarks>
    /// This type is not really meant to be used except by the DynamicProxy2 framework, hence
    /// it should be an open type which is not abstract.
    /// </remarks>
    [ExcludeFromCodeCoverage]
    internal class CommandSetProxy : ICommandSet
    {
        /// <summary>
        /// Returns the reference to the 'current' object.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is created so that dynamic proxies can override the method and return a 
        /// reference to the proxy. This should prevent the 'leaking' of the this reference to the
        /// outside world. For more information see:
        /// http://kozmic.pl/2009/10/30/castle-dynamic-proxy-tutorial-part-xv-patterns-and-antipatterns
        /// </para>
        /// <para>
        /// Note that this method is <c>protected internal</c> so that the <see cref="CommandSetInterceptorSelector"/>
        /// can get access to the method through an expression tree.
        /// </para>
        /// </remarks>
        /// <returns>
        /// The current object.
        /// </returns>
        protected internal virtual object SelfReference()
        {
            return this;
        }
    }
}
