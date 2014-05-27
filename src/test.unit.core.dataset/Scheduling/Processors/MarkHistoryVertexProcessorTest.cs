//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base.Scheduling;
using Apollo.Core.Extensions.Scheduling;
using Apollo.Utilities.History;
using Moq;
using NUnit.Framework;

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
            Assert.AreEqual(typeof(MarkHistoryVertex), processor.VertexTypeToProcess);
        }

        [Test]
        public void ProcessWithIncorrectVertexType()
        {
            var timeline = new Mock<ITimeline>();
            var processor = new MarkHistoryVertexProcessor(timeline.Object, m => { });
            using (var info = new ScheduleExecutionInfo())
            {
                var state = processor.Process(new StartVertex(1), info);
                Assert.AreEqual(ScheduleExecutionState.IncorrectProcessorForVertex, state);
            }
        }

        [Test]
        public void ProcessWithCancellation()
        {
            var timeline = new Mock<ITimeline>();
            using (var info = new ScheduleExecutionInfo())
            {
                info.CancelScheduleExecution();

                var processor = new MarkHistoryVertexProcessor(timeline.Object, m => { });
                var state = processor.Process(new MarkHistoryVertex(1), info);
                Assert.AreEqual(ScheduleExecutionState.Canceled, state);
            }
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
            using (var info = new ScheduleExecutionInfo())
            {
                var state = processor.Process(new MarkHistoryVertex(1), info);
                Assert.AreEqual(ScheduleExecutionState.Executing, state);
                Assert.AreEqual(marker, storedMarker);
            }
        }
    }
}
