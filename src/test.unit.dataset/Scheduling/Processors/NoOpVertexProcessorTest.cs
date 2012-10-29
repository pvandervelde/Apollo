//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base.Scheduling;
using MbUnit.Framework;

namespace Apollo.Core.Dataset.Scheduling.Processors
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class NoOpVertexProcessorTest
    {
        [Test]
        public void VertexTypeToProcess()
        {
            var processor = new NoOpVertexProcessor();
            Assert.AreEqual(typeof(ExecutableNoOpVertex), processor.VertexTypeToProcess);
        }

        [Test]
        public void ProcessWithIncorrectVertexType()
        {
            var processor = new NoOpVertexProcessor();
            var state = processor.Process(new ExecutableStartVertex(1), new ScheduleExecutionInfo());
            Assert.AreEqual(ScheduleExecutionState.IncorrectProcessorForVertex, state);
        }

        [Test]
        public void ProcessWithCancellation()
        {
            var info = new ScheduleExecutionInfo();
            info.CancelScheduleExecution();

            var processor = new NoOpVertexProcessor();
            var state = processor.Process(new ExecutableNoOpVertex(1), info);
            Assert.AreEqual(ScheduleExecutionState.Canceled, state);
        }

        [Test]
        public void Process()
        {
            var processor = new NoOpVertexProcessor();
            var state = processor.Process(new ExecutableNoOpVertex(1), new ScheduleExecutionInfo());
            Assert.AreEqual(ScheduleExecutionState.Executing, state);
        }
    }
}
