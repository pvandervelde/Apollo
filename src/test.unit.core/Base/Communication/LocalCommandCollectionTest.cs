//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Apollo.Core.Base.Communication;
using MbUnit.Framework;
using Moq;

namespace Apollo.Base.Communication
{
    [TestFixture]
    [Description("Tests the ManualDiscoverySource class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class LocalCommandCollectionTest
    {
        public interface IMockCommandSetWithTaskReturn : ICommandSet
        {
            Task MyMethod(int input);
        }

        [Test]
        [Description("Checks that a command set can be registered.")]
        public void Register()
        {
            var collection = new LocalCommandCollection();
            
            var commands = new Mock<IMockCommandSetWithTaskReturn>();
            collection.Register(typeof(IMockCommandSetWithTaskReturn), commands.Object);

            Assert.IsTrue(collection.Exists(pair => pair.Key.Equals(typeof(IMockCommandSetWithTaskReturn))));
        }

        [Test]
        [Description("Checks that a command set can only be registered once.")]
        public void RegisterWithExistingType()
        { 
            var collection = new LocalCommandCollection();
            
            var commands = new Mock<IMockCommandSetWithTaskReturn>();
            collection.Register(typeof(IMockCommandSetWithTaskReturn), commands.Object);
            Assert.Throws<CommandAlreadyRegisteredException>(() => collection.Register(typeof(IMockCommandSetWithTaskReturn), commands.Object));
        }

        [Test]
        [Description("Checks that asking for an unknown command set returns a null reference.")]
        public void CommandsForWithUnknownType()
        {
            var collection = new LocalCommandCollection();
            Assert.IsNull(collection.CommandsFor(typeof(IMockCommandSetWithTaskReturn)));
        }

        [Test]
        [Description("Checks that asking for a command set the command set object.")]
        public void CommandsFor()
        {
            var collection = new LocalCommandCollection();

            var commands = new Mock<IMockCommandSetWithTaskReturn>();
            collection.Register(typeof(IMockCommandSetWithTaskReturn), commands.Object);

            var commandSet = collection.CommandsFor(typeof(IMockCommandSetWithTaskReturn));
            Assert.AreSame(commands.Object, commandSet);
        }
    }
}
