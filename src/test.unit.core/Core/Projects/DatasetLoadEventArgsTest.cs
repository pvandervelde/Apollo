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
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class DatasetLoadEventArgsTest
    {
        /// <summary>
        /// Store a link to a machine here so that we only need to build it once.
        /// </summary>
        private readonly Machine m_Machine = new Machine();

        [Test]
        public void CreateWithEmptyMachinesCollection()
        {
            var id = new DatasetId();
            var machines = new List<Machine> { };
            Assert.Throws<ArgumentException>(() => new DatasetLoadEventArgs(id, machines));
        }

        [Test]
        public void Create()
        {
            var id = new DatasetId();
            var machines = new List<Machine> { m_Machine };

            var args = new DatasetLoadEventArgs(id, machines);
            Assert.AreSame(id, args.Id);
            Assert.AreElementsSameIgnoringOrder(machines, args.LoadedOn);
        }
    }
}
