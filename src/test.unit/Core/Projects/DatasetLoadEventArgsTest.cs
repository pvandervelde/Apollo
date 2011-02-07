//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base;
using Apollo.Core.Base.Loaders;
using MbUnit.Framework;

namespace Apollo.Core.Projects
{
    [TestFixture]
    [Description("Tests the DatasetLoadEventArgs class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class DatasetLoadEventArgsTest
    {
        [Test]
        [Description("Checks that an object cannot be created with a null ID object.")]
        public void CreateWithNullId()
        {
            var machines = new List<Machine> { new Machine() };
            Assert.Throws<ArgumentNullException>(() => new DatasetLoadEventArgs(null, machines));
        }

        [Test]
        [Description("Checks that an object cannot be created with a null machines collection.")]
        public void CreateWithNullMachinesCollection()
        {
            var id = new DatasetId();
            Assert.Throws<ArgumentNullException>(() => new DatasetLoadEventArgs(id, null));
        }

        [Test]
        [Description("Checks that an object cannot be created with an empty machines collection.")]
        public void CreateWithEmptyMachinesCollection()
        {
            var id = new DatasetId();
            var machines = new List<Machine> { };
            Assert.Throws<ArgumentException>(() => new DatasetLoadEventArgs(id, machines));
        }

        [Test]
        [Description("Checks that an object can be created correctly.")]
        public void Create()
        {
            var id = new DatasetId();
            var machines = new List<Machine> { new Machine() };

            var args = new DatasetLoadEventArgs(id, machines);
            Assert.AreSame(id, args.Id);
            Assert.AreElementsSameIgnoringOrder(machines, args.LoadedOn);
        }
    }
}
