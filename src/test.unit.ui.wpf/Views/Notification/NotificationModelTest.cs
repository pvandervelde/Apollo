//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Apollo.Utilities;
using Moq;
using NUnit.Framework;

namespace Apollo.UI.Wpf.Views.Notification
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class NotificationModelTest
    {
        [Test]
        public void Notification()
        {
            var context = new Mock<IContextAware>();
            {
                context.Setup(c => c.IsSynchronized)
                    .Returns(true);
            }

            var collector = new Mock<ICollectNotifications>();
            var model = new NotificationModel(context.Object, collector.Object);

            var propertyChangedWasRaised = 0;
            var properties = new List<string>();
            model.PropertyChanged += (s, e) =>
            {
                propertyChangedWasRaised++;
                properties.Add(e.PropertyName);
            };

            var text = "a";
            collector.Raise(c => c.OnNotification += null, new NotificationEventArgs(text));

            Assert.AreEqual(1, propertyChangedWasRaised);
            Assert.AreEqual(text, model.Notification);
            Assert.That(
                properties,
                Is.EquivalentTo(
                    new List<string>
                    {
                        "Notification",
                    }));
        }
    }
}
