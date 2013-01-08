//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks.Schedulers;
using Apollo.Core.Base.Scheduling;
using Apollo.Core.Dataset.Scheduling.Processors;
using Apollo.Core.Extensions.Scheduling;
using Apollo.Utilities.History;
using MbUnit.Framework;
using Moq;
using QuickGraph;

namespace Apollo.Core.Dataset.Scheduling
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class ScheduleExecutorTest
    {
        private static ISchedule BuildThreeVertexSchedule(IScheduleVertex middle)
        {
            var graph = new BidirectionalGraph<IScheduleVertex, ScheduleEdge>();
            var start = new StartVertex(1);
            graph.AddVertex(start);

            var end = new EndVertex(2);
            graph.AddVertex(end);

            graph.AddVertex(middle);
            graph.AddEdge(new ScheduleEdge(start, middle, null));
            graph.AddEdge(new ScheduleEdge(middle, end, null));

            return new Schedule(graph, start, end);
        }

        private static Schedule CreateScheduleGraphWithOuterAndInnerLoop(
            ScheduleConditionInformation outerLoopConditionInfo,
            ScheduleConditionInformation innerLoopConditionInfo,
            ScheduleActionInformation outerLoopInfo,
            ScheduleActionInformation innerLoopInfo)
        {
            var graph = new BidirectionalGraph<IScheduleVertex, ScheduleEdge>();
            var start = new StartVertex(1);
            graph.AddVertex(start);

            var end = new EndVertex(2);
            graph.AddVertex(end);

            var vertex1 = new InsertVertex(3);
            graph.AddVertex(vertex1);

            var vertex2 = new InsertVertex(4);
            graph.AddVertex(vertex2);

            var vertex3 = new ExecutingActionVertex(5, outerLoopInfo.Id);
            graph.AddVertex(vertex3);

            var vertex4 = new InsertVertex(6);
            graph.AddVertex(vertex4);

            var vertex5 = new ExecutingActionVertex(7, innerLoopInfo.Id);
            graph.AddVertex(vertex5);

            graph.AddEdge(new ScheduleEdge(start, vertex1, null));
            graph.AddEdge(new ScheduleEdge(vertex1, vertex2, null));

            graph.AddEdge(new ScheduleEdge(vertex2, end, outerLoopConditionInfo.Id));
            graph.AddEdge(new ScheduleEdge(vertex2, vertex3, null));

            graph.AddEdge(new ScheduleEdge(vertex3, vertex1, innerLoopConditionInfo.Id));
            graph.AddEdge(new ScheduleEdge(vertex3, vertex4, null));

            graph.AddEdge(new ScheduleEdge(vertex4, vertex5, null));
            graph.AddEdge(new ScheduleEdge(vertex5, vertex3, null));

            return new Schedule(graph, start, end);
        }

        [Test]
        public void RunWithMissingProcessors()
        {
            var schedule = BuildThreeVertexSchedule(new InsertVertex(3));
            var executor = new ScheduleExecutor(
                new List<IProcesExecutableScheduleVertices> 
                    { 
                        new StartVertexProcessor(),
                        new EndVertexProcessor(),
                    },
                ScheduleConditionStorage.CreateInstanceWithoutTimeline(),
                schedule,
                new ScheduleId(),
                new ScheduleExecutionInfo(new CurrentThreadTaskScheduler()));

            ScheduleExecutionState state = ScheduleExecutionState.None;
            executor.OnFinish += (s, e) => { state = e.State; };

            executor.Start();
            Assert.AreEqual(ScheduleExecutionState.NoProcessorForVertex, state);
        }

        [Test]
        [Ignore("not implemented yet")]
        public void CancelRun()
        { 
        }

        [Test]
        public void RunWithActionVertex()
        {
            var action = new Mock<IScheduleAction>();
            {
                action.Setup(a => a.Execute(It.IsAny<CancellationToken>()))
                    .Verifiable();
            }

            var collection = ScheduleActionStorage.CreateInstanceWithoutTimeline();
            var info = collection.Add(
                action.Object, 
                "a", 
                "b");

            var schedule = BuildThreeVertexSchedule(new ExecutingActionVertex(3, info.Id));
            var executor = new ScheduleExecutor(
                new List<IProcesExecutableScheduleVertices> 
                    { 
                        new StartVertexProcessor(),
                        new EndVertexProcessor(),
                        new ActionVertexProcessor(collection),
                    },
                ScheduleConditionStorage.CreateInstanceWithoutTimeline(),
                schedule,
                new ScheduleId(),
                new ScheduleExecutionInfo(new CurrentThreadTaskScheduler()));

            ScheduleExecutionState state = ScheduleExecutionState.None;
            executor.OnFinish += (s, e) => { state = e.State; };

            executor.Start();
            Assert.AreEqual(ScheduleExecutionState.Completed, state);
            action.Verify(a => a.Execute(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public void RunWithMarkHistoryVertex()
        {
            var marker = new TimeMarker(10);
            var timeline = new Mock<ITimeline>();
            {
                timeline.Setup(t => t.Mark())
                    .Returns(marker);
            }

            TimeMarker storedMarker = null;
            var schedule = BuildThreeVertexSchedule(new MarkHistoryVertex(3));
            var executor = new ScheduleExecutor(
                new List<IProcesExecutableScheduleVertices> 
                    { 
                        new StartVertexProcessor(),
                        new EndVertexProcessor(),
                        new MarkHistoryVertexProcessor(timeline.Object, m => storedMarker = m),
                    },
                ScheduleConditionStorage.CreateInstanceWithoutTimeline(),
                schedule,
                new ScheduleId(),
                new ScheduleExecutionInfo(new CurrentThreadTaskScheduler()));

            ScheduleExecutionState state = ScheduleExecutionState.None;
            executor.OnFinish += (s, e) => { state = e.State; };

            executor.Start();
            Assert.AreEqual(ScheduleExecutionState.Completed, state);
            Assert.AreEqual(marker, storedMarker);
        }

        [Test]
        public void RunWithSubScheduleVertex()
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
                    .Returns(subExecutor.Object);
            }

            var id = new ScheduleId();
            var schedule = BuildThreeVertexSchedule(new SubScheduleVertex(3, id));
            var executor = new ScheduleExecutor(
                new List<IProcesExecutableScheduleVertices> 
                    { 
                        new StartVertexProcessor(),
                        new EndVertexProcessor(),
                        new SubScheduleVertexProcessor(distributor.Object),
                    },
                ScheduleConditionStorage.CreateInstanceWithoutTimeline(),
                schedule,
                new ScheduleId(),
                new ScheduleExecutionInfo(new CurrentThreadTaskScheduler()));

            ScheduleExecutionState state = ScheduleExecutionState.None;
            executor.OnFinish += (s, e) => { state = e.State; };

            executor.Start();
            Assert.AreEqual(ScheduleExecutionState.Completed, state);
        }

        [Test]
        public void RunWithNoOpVertex()
        {
            var schedule = BuildThreeVertexSchedule(new InsertVertex(3));
            var executor = new ScheduleExecutor(
                new List<IProcesExecutableScheduleVertices> 
                    { 
                        new StartVertexProcessor(),
                        new EndVertexProcessor(),
                        new InsertVertexProcessor(),
                    },
                ScheduleConditionStorage.CreateInstanceWithoutTimeline(),
                schedule,
                new ScheduleId(),
                new ScheduleExecutionInfo(new CurrentThreadTaskScheduler()));

            ScheduleExecutionState state = ScheduleExecutionState.None;
            executor.OnFinish += (s, e) => { state = e.State; };

            executor.Start();
            Assert.AreEqual(ScheduleExecutionState.Completed, state);
        }

        [Test]
        public void RunWithBlockingConditionOnFirstEdge()
        {
            var condition = new Mock<IScheduleCondition>();
            {
                condition.Setup(c => c.CanTraverse(It.IsAny<CancellationToken>()))
                    .Returns(false);
            }

            var conditionStorage = ScheduleConditionStorage.CreateInstanceWithoutTimeline();
            var conditionInfo = conditionStorage.Add(condition.Object, "a", "b");

            Schedule schedule = null;
            {
                var graph = new BidirectionalGraph<IScheduleVertex, ScheduleEdge>();
                var start = new StartVertex(1);
                graph.AddVertex(start);

                var end = new EndVertex(2);
                graph.AddVertex(end);

                var middle1 = new InsertVertex(3);
                graph.AddVertex(middle1);

                var middle2 = new InsertVertex(4);
                graph.AddVertex(middle2);

                graph.AddEdge(new ScheduleEdge(start, middle1, conditionInfo.Id));
                graph.AddEdge(new ScheduleEdge(start, middle2, null));

                graph.AddEdge(new ScheduleEdge(middle1, end, null));
                graph.AddEdge(new ScheduleEdge(middle2, end, null));

                schedule = new Schedule(graph, start, end);
            }

            var executor = new ScheduleExecutor(
                new List<IProcesExecutableScheduleVertices> 
                    { 
                        new StartVertexProcessor(),
                        new EndVertexProcessor(),
                        new InsertVertexProcessor(),
                    },
                conditionStorage,
                schedule,
                new ScheduleId(),
                new ScheduleExecutionInfo(new CurrentThreadTaskScheduler()));

            var executionOrder = new List<int>();
            executor.OnVertexProcess += (s, e) => { executionOrder.Add(e.Vertex); };

            executor.Start();
            Assert.AreElementsEqual(new int[] { 1, 4, 2 }, executionOrder);
        }

        [Test]
        public void RunWithBlockingConditionOnSecondEdge()
        {
            var condition = new Mock<IScheduleCondition>();
            {
                condition.Setup(c => c.CanTraverse(It.IsAny<CancellationToken>()))
                    .Returns(false);
            }

            var conditionStorage = ScheduleConditionStorage.CreateInstanceWithoutTimeline();
            var conditionInfo = conditionStorage.Add(condition.Object, "a", "b");

            Schedule schedule = null;
            {
                var graph = new BidirectionalGraph<IScheduleVertex, ScheduleEdge>();
                var start = new StartVertex(1);
                graph.AddVertex(start);

                var end = new EndVertex(2);
                graph.AddVertex(end);

                var middle1 = new InsertVertex(3);
                graph.AddVertex(middle1);

                var middle2 = new InsertVertex(4);
                graph.AddVertex(middle2);

                graph.AddEdge(new ScheduleEdge(start, middle1, null));
                graph.AddEdge(new ScheduleEdge(start, middle2, conditionInfo.Id));

                graph.AddEdge(new ScheduleEdge(middle1, end, null));
                graph.AddEdge(new ScheduleEdge(middle2, end, null));

                schedule = new Schedule(graph, start, end);
            }

            var executor = new ScheduleExecutor(
                new List<IProcesExecutableScheduleVertices> 
                    { 
                        new StartVertexProcessor(),
                        new EndVertexProcessor(),
                        new InsertVertexProcessor(),
                    },
                conditionStorage,
                schedule,
                new ScheduleId(),
                new ScheduleExecutionInfo(new CurrentThreadTaskScheduler()));

            var executionOrder = new List<int>();
            executor.OnVertexProcess += (s, e) => { executionOrder.Add(e.Vertex); };

            executor.Start();
            Assert.AreElementsEqual(new int[] { 1, 3, 2 }, executionOrder);
        }

        [Test]
        public void RunWithLoop()
        {
            bool passThrough = false;
            var condition = new Mock<IScheduleCondition>();
            {
                condition.Setup(c => c.CanTraverse(It.IsAny<CancellationToken>()))
                    .Returns(() => passThrough);
            }

            var conditionStorage = ScheduleConditionStorage.CreateInstanceWithoutTimeline();
            var conditionInfo = conditionStorage.Add(condition.Object, "a", "b");

            var action = new Mock<IScheduleAction>();
            {
                action.Setup(a => a.Execute(It.IsAny<CancellationToken>()))
                    .Callback(() => passThrough = true);
            }

            var collection = ScheduleActionStorage.CreateInstanceWithoutTimeline();
            var info = collection.Add(
                action.Object, 
                "a", 
                "b");

            // Making a schedule that looks like:
            // start -> node1 --> node2 -> end
            //            ^           |
            //            |-- node3 <-|
            Schedule schedule = null;
            {
                var graph = new BidirectionalGraph<IScheduleVertex, ScheduleEdge>();
                var start = new StartVertex(1);
                graph.AddVertex(start);

                var end = new EndVertex(2);
                graph.AddVertex(end);

                var vertex1 = new InsertVertex(3);
                graph.AddVertex(vertex1);

                var vertex2 = new InsertVertex(4);
                graph.AddVertex(vertex2);

                var vertex3 = new ExecutingActionVertex(5, info.Id);
                graph.AddVertex(vertex3);

                graph.AddEdge(new ScheduleEdge(start, vertex1, null));
                graph.AddEdge(new ScheduleEdge(vertex1, vertex2, null));

                graph.AddEdge(new ScheduleEdge(vertex2, end, conditionInfo.Id));
                graph.AddEdge(new ScheduleEdge(vertex2, vertex3, null));

                graph.AddEdge(new ScheduleEdge(vertex3, vertex1, null));

                schedule = new Schedule(graph, start, end);
            }

            var executor = new ScheduleExecutor(
                new List<IProcesExecutableScheduleVertices> 
                    { 
                        new StartVertexProcessor(),
                        new EndVertexProcessor(),
                        new InsertVertexProcessor(),
                        new ActionVertexProcessor(collection),
                    },
                conditionStorage,
                schedule,
                new ScheduleId(),
                new ScheduleExecutionInfo(new CurrentThreadTaskScheduler()));

            var executionOrder = new List<int>();
            executor.OnVertexProcess += (s, e) => { executionOrder.Add(e.Vertex); };

            executor.Start();
            Assert.AreElementsEqual(new int[] { 1, 3, 4, 5, 3, 4, 2 }, executionOrder);
        }

        [Test]
        public void RunWithInnerAndOuterLoop()
        {
            bool outerLoopPassThrough = false;
            var outerLoopCondition = new Mock<IScheduleCondition>();
            {
                outerLoopCondition.Setup(c => c.CanTraverse(It.IsAny<CancellationToken>()))
                    .Returns(() => outerLoopPassThrough);
            }

            bool innerLoopPassThrough = false;
            var innerLoopCondition = new Mock<IScheduleCondition>();
            {
                innerLoopCondition.Setup(c => c.CanTraverse(It.IsAny<CancellationToken>()))
                    .Returns(() => innerLoopPassThrough);
            }

            var conditionStorage = ScheduleConditionStorage.CreateInstanceWithoutTimeline();
            var outerLoopConditionInfo = conditionStorage.Add(
                outerLoopCondition.Object, 
                "a", 
                "b");
            var innerLoopConditionInfo = conditionStorage.Add(
                innerLoopCondition.Object, 
                "a", 
                "b");

            var outerLoopAction = new Mock<IScheduleAction>();
            {
                outerLoopAction.Setup(a => a.Execute(It.IsAny<CancellationToken>()))
                    .Callback(() => outerLoopPassThrough = true);
            }

            var innerLoopAction = new Mock<IScheduleAction>();
            {
                innerLoopAction.Setup(a => a.Execute(It.IsAny<CancellationToken>()))
                    .Callback(() => innerLoopPassThrough = true);
            }

            var collection = ScheduleActionStorage.CreateInstanceWithoutTimeline();
            var outerLoopInfo = collection.Add(
                outerLoopAction.Object, 
                "a", 
                "b");
            var innerLoopInfo = collection.Add(
                innerLoopAction.Object, 
                "a", 
                "b");

            // Making a schedule that looks like:
            // start -> node1 --> node2 -> end
            //            ^           |
            //            |-- node3 <-|
            //                ^  |
            //         node5--|  |->  node4
            //           ^              |
            //           |--------------|
            Schedule schedule = null;
            {
                schedule = CreateScheduleGraphWithOuterAndInnerLoop(outerLoopConditionInfo, innerLoopConditionInfo, outerLoopInfo, innerLoopInfo);
            }

            var executor = new ScheduleExecutor(
                new List<IProcesExecutableScheduleVertices> 
                    { 
                        new StartVertexProcessor(),
                        new EndVertexProcessor(),
                        new InsertVertexProcessor(),
                        new ActionVertexProcessor(collection),
                    },
                conditionStorage,
                schedule,
                new ScheduleId(),
                new ScheduleExecutionInfo(new CurrentThreadTaskScheduler()));

            var executionOrder = new List<int>();
            executor.OnVertexProcess += (s, e) => { executionOrder.Add(e.Vertex); };

            executor.Start();
            Assert.AreElementsEqual(new int[] { 1, 3, 4, 5, 6, 7, 5, 3, 4, 2 }, executionOrder);
        }

        [Test]
        [Ignore("Not implemented yet")]
        public void RunWithPause()
        { 
        }

        [Test]
        public void RunWithNonPassableEdgeSet()
        {
            var condition = new Mock<IScheduleCondition>();
            {
                condition.Setup(c => c.CanTraverse(It.IsAny<CancellationToken>()))
                    .Returns(false);
            }

            var conditionStorage = ScheduleConditionStorage.CreateInstanceWithoutTimeline();
            var conditionInfo = conditionStorage.Add(condition.Object, "a", "b");

            Schedule schedule = null;
            {
                var graph = new BidirectionalGraph<IScheduleVertex, ScheduleEdge>();
                var start = new StartVertex(1);
                graph.AddVertex(start);

                var end = new EndVertex(2);
                graph.AddVertex(end);

                var middle1 = new InsertVertex(3);
                graph.AddVertex(middle1);

                var middle2 = new InsertVertex(4);
                graph.AddVertex(middle2);

                graph.AddEdge(new ScheduleEdge(start, middle1, conditionInfo.Id));
                graph.AddEdge(new ScheduleEdge(start, middle2, conditionInfo.Id));

                graph.AddEdge(new ScheduleEdge(middle1, end, null));
                graph.AddEdge(new ScheduleEdge(middle2, end, null));

                schedule = new Schedule(graph, start, end);
            }

            var executor = new ScheduleExecutor(
                new List<IProcesExecutableScheduleVertices> 
                    { 
                        new StartVertexProcessor(),
                        new EndVertexProcessor(),
                        new InsertVertexProcessor(),
                    },
                conditionStorage,
                schedule,
                new ScheduleId(),
                new ScheduleExecutionInfo(new CurrentThreadTaskScheduler()));

            ScheduleExecutionState state = ScheduleExecutionState.None;
            executor.OnFinish += (s, e) => { state = e.State; };

            executor.Start();
            Assert.AreEqual(ScheduleExecutionState.NoTraversableEdgeFound, state);
        }
    }
}
