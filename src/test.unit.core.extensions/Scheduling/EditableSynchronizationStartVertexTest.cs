//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MbUnit.Framework;
using Moq;

namespace Apollo.Core.Extensions.Scheduling
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class EditableSynchronizationStartVertexTest
    {
        [Test]
        public void Create()
        {
            var variable = new Mock<IScheduleVariable>();

            var index = 10;
            var variables = new List<IScheduleVariable> { variable.Object };
            var vertex = new EditableSynchronizationStartVertex(index, variables);

            Assert.AreEqual(index, vertex.Index);
            Assert.AreSame(variables, vertex.VariablesToSynchronizeOn);
        }
    }
}
