//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Defines an attribute that indicates that an <see cref="IMessageProcessAction"/> class
    /// provides a last chance for processing a message before the message is destroyed 
    /// without processing it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    internal sealed class LastChanceMessageHandlerAttribute : Attribute
    {
    }
}
