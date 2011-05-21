//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using MbUnit.Framework;
using Moq;

namespace Apollo.Utilities.Commands
{
    [TestFixture]
    [Description("Tests the CommandFactory class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class CommandFactoryTest
    {
        [Test]
        [Description("Checks that a command cannot be added with a duplicate name.")]
        public void AddWithDuplicateName()
        {
            var factory = new CommandFactory();

            var name = new CommandId("name");
            Func<ICommand> activator = () => new Mock<ICommand>().Object;
            
            factory.Add(name, activator);
            Assert.Throws<DuplicateCommandException>(() => factory.Add(name, activator));
        }

        [Test]
        [Description("Checks that a command can be added.")]
        public void Add()
        {
            var factory = new CommandFactory();

            var name = new CommandId("name");
            Func<ICommand> activator = () => new Mock<ICommand>().Object;

            factory.Add(name, activator);
            Assert.IsTrue(factory.Contains(name));
        }

        [Test]
        [Description("Checks that a command cannot be removed with an unknown name.")]
        public void RemoveWithUnknownName()
        {
            var factory = new CommandFactory();
            Assert.Throws<UnknownCommandException>(() => factory.Remove(new CommandId("name")));
        }

        [Test]
        [Description("Checks that a command can be removed.")]
        public void Remove()
        {
            var factory = new CommandFactory();

            var name = new CommandId("name");
            Func<ICommand> activator = () => new Mock<ICommand>().Object;

            factory.Add(name, activator);
            factory.Remove(name);
            Assert.IsFalse(factory.Contains(name));
        }

        [Test]
        [Description("Checks that a command cannot be invoked with an unknown name.")]
        public void InvokeWithoutContextWithUnknownName()
        {
            var factory = new CommandFactory();
            Assert.Throws<UnknownCommandException>(() => factory.Invoke(new CommandId("name")));
        }

        [Test]
        [Description("Checks that a command can be invoked.")]
        public void InvokeWithoutContext()
        {
            var factory = new CommandFactory();
            var name = new CommandId("name");

            ICommandContext storedContext = null;
            var commandMock = new Mock<ICommand>();
            {
                commandMock.Setup(command => command.Invoke(It.IsAny<ICommandContext>()))
                    .Callback<ICommandContext>(context => { storedContext = context; });
            }

            factory.Add(name, () => commandMock.Object);
            factory.Invoke(name);
            Assert.IsNotNull(storedContext);
        }

        [Test]
        [Description("Checks that a command cannot be invoked.")]
        public void Invoke()
        {
            var factory = new CommandFactory();
            var name = new CommandId("name");

            ICommandContext storedContext = null;
            var commandMock = new Mock<ICommand>();
            {
                commandMock.Setup(command => command.Invoke(It.IsAny<ICommandContext>()))
                    .Callback<ICommandContext>(context => { storedContext = context; });
            }

            var contextMock = new Mock<ICommandContext>();

            factory.Add(name, () => commandMock.Object);
            factory.Invoke(name, contextMock.Object);
            Assert.AreSame(contextMock.Object, storedContext);
        }
    }
}
