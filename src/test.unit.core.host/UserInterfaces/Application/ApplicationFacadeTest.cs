//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Utilities.Commands;
using Moq;
using Nuclei.Diagnostics;
using NUnit.Framework;

namespace Apollo.Core.Host.UserInterfaces.Application
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class ApplicationFacadeTest
    {
        [Test]
        public void Shutdown()
        {
            bool wasTriggered = false;

            var service = new Mock<IUserInterfaceService>();
            {
                service.Setup(s => s.Contains(It.IsAny<CommandId>()))
                    .Returns(true);
                service.Setup(s => s.Invoke(It.IsAny<CommandId>(), It.IsAny<ICommandContext>()))
                    .Callback<CommandId, ICommandContext>(
                        (id, context) => 
                            {
                                Assert.AreEqual(ShutdownApplicationCommand.CommandId, id);

                                wasTriggered = true;
                            });
            }

            var notificationNames = new Mock<INotificationNameConstants>();
            {
                notificationNames.Setup(n => n.SystemShuttingDown)
                    .Returns(new NotificationName("a"));
            }

            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var facade = new ApplicationFacade(service.Object, notificationNames.Object, systemDiagnostics);
            facade.Shutdown();
            Assert.IsTrue(wasTriggered);
        }

        [Test]
        public void ApplicationStatus()
        {
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var service = new Mock<IUserInterfaceService>();
            var notificationNames = new Mock<INotificationNameConstants>();
            {
                notificationNames.Setup(n => n.SystemShuttingDown)
                    .Returns(new NotificationName("a"));
            }

            var facade = new ApplicationFacade(service.Object, notificationNames.Object, systemDiagnostics);

            var status = facade.ApplicationStatus;
            Assert.IsNotNull(status);
            Assert.AreEqual(typeof(ApplicationFacade).Assembly.GetName().Version, status.CoreVersion);
        }

        [Test]
        public void RegisterNotification()
        {
            NotificationName notification = null;
            Action<INotificationArguments> action = null;

            var service = new Mock<IUserInterfaceService>();
            {
                service.Setup(s => s.RegisterNotification(It.IsAny<NotificationName>(), It.IsAny<Action<INotificationArguments>>()))
                    .Callback<NotificationName, Action<INotificationArguments>>(
                        (n, o) => 
                            {
                                notification = n;
                                action = o;
                            });
            }

            var notificationNames = new Mock<INotificationNameConstants>();
            {
                notificationNames.Setup(n => n.SystemShuttingDown)
                    .Returns(new NotificationName("a"));
            }

            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var facade = new ApplicationFacade(service.Object, notificationNames.Object, systemDiagnostics);
            
            var name = new NotificationName("bla");
            Action<INotificationArguments> callback = o => { };
            facade.RegisterNotification(name, callback);

            Assert.AreSame(name, notification);
            Assert.AreSame(callback, action);
        }
    }
}
