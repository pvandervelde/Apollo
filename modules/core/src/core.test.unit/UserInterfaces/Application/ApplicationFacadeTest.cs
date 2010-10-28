//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Utils.Commands;
using MbUnit.Framework;
using Moq;

namespace Apollo.Core.UserInterfaces.Application
{
    [TestFixture]
    [Description("Tests the FusionHelper class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class ApplicationFacadeTest
    {
        [Test]
        [Description("Checks that an object cannot be created without the user interface service.")]
        public void CreateWithNullService()
        {
            Assert.Throws<ArgumentNullException>(() => new ApplicationFacade(null));
        }

        [Test]
        [Description("Checks that the canshutdown command is correctly given.")]
        public void CanShutDown()
        {
            bool returnValue = false;

            var service = new Mock<IUserInterfaceService>();
            {
                service.Setup(s => s.Invoke(It.IsAny<CommandId>(), It.IsAny<ICommandContext>()))
                    .Callback<CommandId, ICommandContext>(
                        (id, context) => 
                            {
                                Assert.AreEqual(CheckApplicationCanShutdownCommand.CommandId, id);
                                var c = (CheckApplicationCanShutdownContext)context;
                                c.Result = returnValue;
                            });
            }

            var facade = new ApplicationFacade(service.Object);
            var result = facade.CanShutdown();

            Assert.AreEqual(returnValue, result);
        }

        [Test]
        [Description("Checks that the shutdown command is correctly given.")]
        public void Shutdown()
        {
            var force = true;

            var service = new Mock<IUserInterfaceService>();
            {
                service.Setup(s => s.Invoke(It.IsAny<CommandId>(), It.IsAny<ICommandContext>()))
                    .Callback<CommandId, ICommandContext>(
                        (id, context) => 
                            {
                                Assert.AreEqual(ShutdownApplicationCommand.CommandId, id);
                                
                                var c = (ShutdownApplicationContext)context;
                                Assert.AreEqual(force, c.IsShutdownForced);
                                c.Result = false;
                            });
            }

            var facade = new ApplicationFacade(service.Object);

            bool wasTriggered = false;
            Action action = () => { wasTriggered = true; };
            facade.Shutdown(force, action);

            Assert.IsTrue(wasTriggered);
        }

        [Test]
        [Description("Checks that notifications can be registered.")]
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

            var facade = new ApplicationFacade(service.Object);
            
            var name = new NotificationName("bla");
            Action<INotificationArguments> callback = o => { };
            facade.RegisterNotification(name, callback);

            Assert.AreSame(name, notification);
            Assert.AreSame(callback, action);
        }
    }
}
