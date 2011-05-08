﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Castle.DynamicProxy;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Defines an <see cref="IInterceptor"/> for <see cref="CommandSetProxy.SelfReference"/>.
    /// </summary>
    /// <remarks>
    /// This interceptor is created so that dynamic proxies can override the 
    /// <see cref="CommandSetProxy.SelfReference"/> and return a reference to the proxy
    /// instead of the underlying object. This should prevent the 'leaking' of an incorrect reference 
    /// to the outside world. For more information see:
    /// http://kozmic.pl/2009/10/30/castle-dynamic-proxy-tutorial-part-xv-patterns-and-antipatterns
    /// </remarks>
    internal sealed class CommandSetProxySelfReferenceInterceptor : IInterceptor
    {
        /// <summary>
        /// Called when a method or property call is intercepted.
        /// </summary>
        /// <param name="invocation">Information about the call that was intercepted.</param>
        public void Intercept(IInvocation invocation)
        {
            invocation.ReturnValue = invocation.Proxy;
        }
    }
}
