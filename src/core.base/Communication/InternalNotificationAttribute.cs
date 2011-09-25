//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Indicates that a <see cref="INotificationSet"/> is only for use by the notification system.
    /// </summary>
    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public sealed class InternalNotificationAttribute : Attribute
    {
    }
}
