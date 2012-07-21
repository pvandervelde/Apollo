//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using MbUnit.Framework;

namespace Apollo.Core.Dataset.Scheduling
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class ExecutableSynchronizationEndVertexTest
    {
        [Test]
        public void Index()
        {
            var index = 10;
            var vertex = new ExecutableSynchronizationEndVertex(index);

            Assert.AreEqual(index, vertex.Index);
        }
    }
}
