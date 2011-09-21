//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base.Communication;

namespace Apollo.Base.Communication
{
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1601:PartialElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation. Especially not in partial classes.")]
    public sealed partial class NotificationProxyBuilderTest
    {
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
        public sealed class MockNotificationSetNotAnInterface : INotificationSet
        {
        }

        public interface IMockNotificationSetWithGenericParameters<T> : INotificationSet
        {
        }

        public interface IMockNotificationSetWithProperties : INotificationSet
        {
            int MyProperty { get; set; }
        }

        public interface IMockNotificationSetWithMethods : INotificationSet
        {
            void SomeMethod(int someParameter);
        }

        public interface IMockNotificationSetWithoutEvents : INotificationSet
        {
        }

        public delegate void MyEventHandler(object sender, EventArgs args);

        public interface IMockNotificationSetWithNonEventHandlerEvent : INotificationSet
        {
            event MyEventHandler OnMyEvent;
        }

        public class MyNonSerializableEventArgs : EventArgs
        {
        }

        public interface IMockNotificationSetWithNonSerializableEventArgs : INotificationSet
        {
            event EventHandler<MyNonSerializableEventArgs> OnMyEvent;
        }
        
        public interface IMockNotificationSetWithEventHandler : INotificationSet
        {
            event EventHandler OnMyEvent;
        }

        [Serializable]
        public class MySerializableEventArgs : EventArgs
        {
        }

        public interface IMockNotificationSetWithTypedEventHandler : INotificationSet
        {
            event EventHandler<MySerializableEventArgs> OnMyEvent;
        }
    }
}
