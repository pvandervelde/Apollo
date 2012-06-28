//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Extensions.Scheduling;
using Apollo.Utilities.History;
using MbUnit.Framework;
using Moq;

namespace Apollo.Core.Dataset.Scheduling.Processors
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class MarkHistoryVertexProcessorTest
    {
        [Test]
        public void VertexTypeToProcess()
        {
            var timeline = new Mock<ITimeline>();
            var processor = new MarkHistoryVertexProcessor(timeline.Object, m => { });
            Assert.AreEqual(typeof(ExecutableMarkHistoryVertex), processor.VertexTypeToProcess);
        }

        [Test]
        public void ProcessWithIncorrectVertexType()
        {
            var timeline = new Mock<ITimeline>();
            var processor = new MarkHistoryVertexProcessor(timeline.Object, m => { });
            var state = processor.Process(new ExecutableStartVertex(1), new ScheduleExecutionInfo());
            Assert.AreEqual(ScheduleExecutionState.IncorrectProcessorForVertex, state);
        }

        [Test]
        public void ProcessWithCancellation()
        {
            var timeline = new Mock<ITimeline>();
            var info = new ScheduleExecutionInfo();
            info.CancelScheduleExecution();

            var processor = new MarkHistoryVertexProcessor(timeline.Object, m => { });
            var state = processor.Process(new ExecutableMarkHistoryVertex(1), info);
            Assert.AreEqual(ScheduleExecutionState.Cancelled, state);
        }

        [Test]
        [Ignore("Don't know how to test pausing a process properly")]
        public void ProcessWithPause()
        {
        }

        [Test]
        public void Process()
        {
            var marker = new TimeMarker(10);
            var timeline = new Mock<ITimeline>();
            {
                timeline.Setup(t => t.Mark())
                    .Returns(marker);
            }

            TimeMarker storedMarker = null;
            var processor = new MarkHistoryVertexProcessor(timeline.Object, m => storedMarker = m);
            var state = processor.Process(new ExecutableMarkHistoryVertex(1), new ScheduleExecutionInfo());
            Assert.AreEqual(ScheduleExecutionState.Executing, state);
            Assert.AreEqual(marker, storedMarker);
        }
    }
}
