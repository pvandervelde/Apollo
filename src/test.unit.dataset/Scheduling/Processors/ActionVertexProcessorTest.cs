//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Apollo.Core.Base.Scheduling;
using Apollo.Core.Extensions.Scheduling;
using MbUnit.Framework;
using Moq;

namespace Apollo.Core.Dataset.Scheduling.Processors
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class ActionVertexProcessorTest
    {
        [Test]
        public void VertexTypeToProcess()
        {
            var collection = ScheduleActionStorage.BuildStorageWithoutTimeline();
            var processor = new ActionVertexProcessor(collection);
            Assert.AreEqual(typeof(ExecutableActionVertex), processor.VertexTypeToProcess);
        }

        [Test]
        public void ProcessWithIncorrectVertexType()
        {
            var collection = ScheduleActionStorage.BuildStorageWithoutTimeline();
            var processor = new ActionVertexProcessor(collection);
            var state = processor.Process(new ExecutableStartVertex(1), new ScheduleExecutionInfo());
            Assert.AreEqual(ScheduleExecutionState.IncorrectProcessorForVertex, state);
        }

        [Test]
        public void ProcessWithCancellation()
        {
            var action = new Mock<IScheduleAction>();
            {
                action.Setup(a => a.Execute(It.IsAny<CancellationToken>()))
                    .Verifiable();
            }

            var collection = ScheduleActionStorage.BuildStorageWithoutTimeline();
            var info = collection.Add(
                new ScheduleElementId(), 
                action.Object, 
                "a", 
                "b", 
                "c", 
                new List<IScheduleVariable>(), 
                new List<IScheduleDependency>());

            var executionInfo = new ScheduleExecutionInfo();
            executionInfo.CancelScheduleExecution();

            var processor = new ActionVertexProcessor(collection);
            var state = processor.Process(new ExecutableActionVertex(1, info.Id), executionInfo);
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
            var action = new Mock<IScheduleAction>();
            {
                action.Setup(a => a.Execute(It.IsAny<CancellationToken>()))
                    .Verifiable();
            }

            var collection = ScheduleActionStorage.BuildStorageWithoutTimeline();
            var info = collection.Add(
                new ScheduleElementId(), 
                action.Object, 
                "a", 
                "b", 
                "c", 
                new List<IScheduleVariable>(), 
                new List<IScheduleDependency>());

            var processor = new ActionVertexProcessor(collection);
            var state = processor.Process(new ExecutableActionVertex(1, info.Id), new ScheduleExecutionInfo());
            Assert.AreEqual(ScheduleExecutionState.Executing, state);
            action.Verify(a => a.Execute(It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
