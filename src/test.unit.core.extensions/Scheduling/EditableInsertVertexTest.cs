//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using MbUnit.Framework;

namespace Apollo.Core.Extensions.Scheduling
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class EditableInsertVertexTest
    {
        [Test]
        public void Index()
        {
            var index = 10;
            var vertex = new InsertVertex(index);

            Assert.AreEqual(index, vertex.Index);
            Assert.AreEqual(-1, vertex.RemainingInserts);
        }

        [Test]
        public void InsertCount()
        {
            var index = 10;
            var insertCount = 5;
            var vertex = new InsertVertex(index, insertCount);

            Assert.AreEqual(index, vertex.Index);
            Assert.AreEqual(insertCount, vertex.RemainingInserts);
        }
    }
}
