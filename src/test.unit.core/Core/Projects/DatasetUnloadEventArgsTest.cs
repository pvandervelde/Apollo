//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base;
using MbUnit.Framework;

namespace Apollo.Core.Projects
{
    [TestFixture]
    [Description("Tests the DatasetUnloadEventArgs class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class DatasetUnloadEventArgsTest
    {
        [Test]
        [Description("Checks that an object can be created correctly.")]
        public void Create()
        {
            var id = new DatasetId();

            var args = new DatasetUnloadEventArgs(id);
            Assert.AreSame(id, args.Id);
        }
    }
}
