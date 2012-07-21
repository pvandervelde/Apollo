//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Extensions.Scheduling;
using MbUnit.Framework;
using Moq;

namespace Apollo.Core.Dataset.Scheduling
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class ExecutableSynchronizationStartVertexTest
    {
        [Test]
        public void Create()
        {
            var variable = new Mock<IScheduleVariable>();

            var index = 10;
            var variables = new List<IScheduleVariable> { variable.Object };
            var vertex = new ExecutableSynchronizationStartVertex(index, variables);

            Assert.AreEqual(index, vertex.Index);
            Assert.AreSame(variables, vertex.VariablesToSynchronizeOn);
        }
    }
}
