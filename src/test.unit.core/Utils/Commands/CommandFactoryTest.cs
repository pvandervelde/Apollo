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
        [Description("Checks that a command cannot be added without a name.")]
        public void AddWithNullName()
        {
            var factory = new CommandFactory();
            Assert.Throws<ArgumentNullException>(() => factory.Add(null, () => new Mock<ICommand>().Object));
        }

        [Test]
        [Description("Checks that a command cannot be added without an activator.")]
        public void AddWithNullActivator()
        {
            var factory = new CommandFactory();
            Assert.Throws<ArgumentNullException>(() => factory.Add(new CommandId("name"), null));
        }

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
        [Description("Checks that a command cannot be removed without a name.")]
        public void RemoveWithNullName()
        {
            var factory = new CommandFactory();
            Assert.Throws<ArgumentNullException>(() => factory.Remove(null));
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
        [Description("Checks that a command cannot be invoked without a name.")]
        public void InvokeWithoutContextWithNullName()
        {
            var factory = new CommandFactory();
            Assert.Throws<ArgumentNullException>(() => factory.Invoke(null));
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
        [Description("Checks that a command cannot be invoked without a name.")]
        public void InvokeWithNullName()
        {
            var factory = new CommandFactory();
            Assert.Throws<ArgumentNullException>(() => factory.Invoke(null, new Mock<ICommandContext>().Object));
        }

        [Test]
        [Description("Checks that a command cannot be invoked without a context.")]
        public void InvokeWithNullContext()
        {
            var factory = new CommandFactory();
            Assert.Throws<ArgumentNullException>(() => factory.Invoke(new CommandId("name"), null));
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
