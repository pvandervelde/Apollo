//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base.Scheduling;
using Apollo.Core.Extensions.Scheduling;
using MbUnit.Framework;
using Moq;

namespace Apollo.Core.Dataset.Scheduling.Processors
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class SubScheduleVertexProcessorTest
    {
        [Test]
        public void VertexTypeToProcess()
        {
            var distributor = new Mock<IDistributeScheduleExecutions>();
            var processor = new SubScheduleVertexProcessor(distributor.Object);
            Assert.AreEqual(typeof(SubScheduleVertex), processor.VertexTypeToProcess);
        }

        [Test]
        public void ProcessWithIncorrectVertexType()
        {
            var distributor = new Mock<IDistributeScheduleExecutions>();
            var processor = new SubScheduleVertexProcessor(distributor.Object);
            var state = processor.Process(new StartVertex(1), new ScheduleExecutionInfo());
            Assert.AreEqual(ScheduleExecutionState.IncorrectProcessorForVertex, state);
        }

        [Test]
        public void ProcessWithCancellation()
        {
            var distributor = new Mock<IDistributeScheduleExecutions>();
            var info = new ScheduleExecutionInfo();
            info.CancelScheduleExecution();

            var processor = new SubScheduleVertexProcessor(distributor.Object);
            var state = processor.Process(new SubScheduleVertex(1, new ScheduleId()), info);
            Assert.AreEqual(ScheduleExecutionState.Canceled, state);
        }

        [Test]
        [Ignore("Don't know how to test pausing a process properly")]
        public void ProcessWithPause()
        {
        }

        [Test]
        public void Process()
        {
            var subExecutor = new Mock<IExecuteSchedules>();
            {
                subExecutor.Setup(s => s.IsLocal)
                    .Returns(false);
            }

            var distributor = new Mock<IDistributeScheduleExecutions>();
            {
                distributor.Setup(
                    d => d.Execute(
                        It.IsAny<ScheduleId>(),
                        It.IsAny<IEnumerable<IScheduleVariable>>(),
                        It.IsAny<ScheduleExecutionInfo>(),
                        It.IsAny<bool>()))
                    .Returns(subExecutor.Object)
                    .Verifiable();
            }

            var id = new ScheduleId();

            var processor = new SubScheduleVertexProcessor(distributor.Object);
            var state = processor.Process(new SubScheduleVertex(1, id), new ScheduleExecutionInfo());
            Assert.AreEqual(ScheduleExecutionState.Executing, state);
            distributor.Verify(
                d => d.Execute(
                    It.Is<ScheduleId>(incoming => incoming.Equals(id)),
                    It.IsAny<IEnumerable<IScheduleVariable>>(),
                    It.IsAny<ScheduleExecutionInfo>(),
                    It.IsAny<bool>()),
                Times.Once());
        }
    }
}
