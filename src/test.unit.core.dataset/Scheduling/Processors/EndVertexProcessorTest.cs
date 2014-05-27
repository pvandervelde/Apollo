//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base.Scheduling;
using Apollo.Core.Extensions.Scheduling;
using NUnit.Framework;

namespace Apollo.Core.Dataset.Scheduling.Processors
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class EndVertexProcessorTest
    {
        [Test]
        public void VertexTypeToProcess()
        {
            var processor = new EndVertexProcessor();
            Assert.AreEqual(typeof(EndVertex), processor.VertexTypeToProcess);
        }

        [Test]
        public void ProcessWithIncorrectVertexType()
        {
            var processor = new EndVertexProcessor();
            using (var info = new ScheduleExecutionInfo())
            {
                var state = processor.Process(new StartVertex(1), info);
                Assert.AreEqual(ScheduleExecutionState.IncorrectProcessorForVertex, state);
            }
        }

        [Test]
        public void Process()
        {
            var processor = new EndVertexProcessor();
            using (var info = new ScheduleExecutionInfo())
            {
                var state = processor.Process(new EndVertex(1), info);
                Assert.AreEqual(ScheduleExecutionState.Completed, state);
            }
        }
    }
}
