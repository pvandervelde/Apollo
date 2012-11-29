//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Apollo.Core.Base.Scheduling;
using Apollo.Core.Extensions.Scheduling;
using MbUnit.Framework;
using Moq;
using QuickGraph;

namespace Apollo.Core.Dataset.Scheduling
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class ScheduleDistributorTest
    {
        private static EditableSchedule BuildSchedule(
            ScheduleElementId action1, 
            ScheduleElementId action2, 
            ScheduleId scheduleId,
            ScheduleElementId exitCondition,
            ScheduleElementId passThroughCondition)
        {
            var variable = new Mock<IScheduleVariable>();

            // Making a schedule that looks like:
            // start --> node1 -----------------------> node2 -> end
            //            ^                              |
            //            |-- node5 <-- node4 <-- node3<-|
            //                           ^  |
            //                    node7--|  |->  node6
            //                      ^              |
            //                      |--------------|
            EditableSchedule schedule = null;
            {
                var graph = new BidirectionalGraph<IScheduleVertex, ScheduleEdge>();

                var start = new EditableStartVertex(1);
                graph.AddVertex(start);

                var end = new EditableEndVertex(2);
                graph.AddVertex(end);

                var vertex1 = new EditableExecutingActionVertex(3, action1);
                graph.AddVertex(vertex1);

                var vertex2 = new EditableExecutingActionVertex(4, action2);
                graph.AddVertex(vertex2);

                var vertex3 = new EditableSynchronizationStartVertex(5, new IScheduleVariable[] { variable.Object });
                graph.AddVertex(vertex3);

                var vertex4 = new EditableExecutingActionVertex(6, action2);
                graph.AddVertex(vertex4);

                var vertex5 = new EditableSynchronizationEndVertex(7);
                graph.AddVertex(vertex5);

                var vertex6 = new EditableSubScheduleVertex(8, scheduleId);
                graph.AddVertex(vertex6);

                var vertex7 = new EditableInsertVertex(9);
                graph.AddVertex(vertex7);

                graph.AddEdge(new ScheduleEdge(start, vertex1));
                graph.AddEdge(new ScheduleEdge(vertex1, vertex2));

                graph.AddEdge(new ScheduleEdge(vertex2, end, exitCondition));
                graph.AddEdge(new ScheduleEdge(vertex2, vertex3));

                graph.AddEdge(new ScheduleEdge(vertex3, vertex4));
                
                graph.AddEdge(new ScheduleEdge(vertex4, vertex5, passThroughCondition));
                graph.AddEdge(new ScheduleEdge(vertex4, vertex6));

                graph.AddEdge(new ScheduleEdge(vertex5, vertex1));
                graph.AddEdge(new ScheduleEdge(vertex6, vertex7));
                graph.AddEdge(new ScheduleEdge(vertex7, vertex4));

                schedule = new EditableSchedule(graph, start, end);
            }

            return schedule;
        }

        private static void VerifySchedule(EditableSchedule editableSchedule, ExecutableSchedule executableSchedule)
        {
            var linearisedEditableGraph = new List<Tuple<IScheduleVertex, List<Tuple<ScheduleElementId, IScheduleVertex>>>>();
            editableSchedule.TraverseSchedule(
                editableSchedule.Start,
                true,
                (vertex, edges) =>
                {
                    linearisedEditableGraph.Add(
                        new Tuple<IScheduleVertex, List<Tuple<ScheduleElementId, IScheduleVertex>>>(
                            vertex, 
                            edges.ToList()));

                    return true;
                });

            var linearisedExecutableGraph = new List<Tuple<IScheduleVertex, List<Tuple<ScheduleElementId, IScheduleVertex>>>>();
            var executableGraph = executableSchedule.Graph;
            TraverseExecutableSchedule(
                executableSchedule.Graph,
                executableSchedule.Start,
                (vertex, edges) =>
                {
                    linearisedExecutableGraph.Add(
                        new Tuple<IScheduleVertex, List<Tuple<ScheduleElementId, IScheduleVertex>>>(
                            vertex,
                            edges.ToList()));
                });

            CompareLinearisedGraphs(linearisedEditableGraph, linearisedExecutableGraph);
        }

        private static void TraverseExecutableSchedule(
            IVertexListGraph<IScheduleVertex, ScheduleEdge> executableGraph,
            IScheduleVertex start,
            Action<IScheduleVertex, IEnumerable<Tuple<ScheduleElementId, IScheduleVertex>>> vertexAction)
        {
            var nodeCounter = new List<IScheduleVertex>();

            var uncheckedVertices = new Queue<IScheduleVertex>();
            uncheckedVertices.Enqueue(start);
            while (uncheckedVertices.Count > 0)
            {
                var source = uncheckedVertices.Dequeue();
                if (nodeCounter.Contains(source))
                {
                    continue;
                }

                nodeCounter.Add(source);

                var outEdges = executableGraph.OutEdges(source);
                var traverseMap = from edge in outEdges
                                  select new Tuple<ScheduleElementId, IScheduleVertex>(
                                      edge.TraversingCondition,
                                      edge.Target);

                vertexAction(source, traverseMap);
                foreach (var outEdge in outEdges)
                {
                    uncheckedVertices.Enqueue(outEdge.Target);
                }
            }
        }

        private static void CompareLinearisedGraphs(
            List<Tuple<IScheduleVertex, List<Tuple<ScheduleElementId, IScheduleVertex>>>> expected,
            List<Tuple<IScheduleVertex, List<Tuple<ScheduleElementId, IScheduleVertex>>>> given)
        {
            Assert.AreEqual(expected.Count, given.Count);
            for (int i = 0; i < expected.Count; i++)
            {
                var expectedPair = expected[i];
                var givenPair = given[i];

                Assert.AreEqual(expectedPair.Item1.Index, givenPair.Item1.Index);
                Assert.AreEqual(TranslateEditableVertexToExecutableVertex(expectedPair.Item1.GetType()), givenPair.Item1.GetType());

                Assert.AreEqual(expectedPair.Item2.Count, givenPair.Item2.Count);
                for (int j = 0; j < expectedPair.Item2.Count; j++)
                {
                    Assert.AreEqual(expectedPair.Item2[j].Item1, givenPair.Item2[j].Item1);
                    Assert.AreEqual(expectedPair.Item2[j].Item2.Index, givenPair.Item2[j].Item2.Index);
                }
            }
        }

        private static Type TranslateEditableVertexToExecutableVertex(Type type)
        {
            if (type == typeof(EditableStartVertex))
            {
                return typeof(ExecutableStartVertex);
            }

            if (type == typeof(EditableEndVertex))
            {
                return typeof(ExecutableEndVertex);
            }

            if (type == typeof(EditableExecutingActionVertex))
            {
                return typeof(ExecutableActionVertex);
            }

            if (type == typeof(EditableInsertVertex))
            {
                return typeof(ExecutableNoOpVertex);
            }

            if (type == typeof(EditableMarkHistoryVertex))
            {
                return typeof(ExecutableMarkHistoryVertex);
            }

            if (type == typeof(EditableSubScheduleVertex))
            {
                return typeof(ExecutableSubScheduleVertex);
            }

            if (type == typeof(EditableSynchronizationStartVertex))
            {
                return typeof(ExecutableSynchronizationStartVertex);
            }

            if (type == typeof(EditableSynchronizationEndVertex))
            {
                return typeof(ExecutableSynchronizationEndVertex);
            }

            return null;
        }

        [Test]
        public void ExecuteWithUnknownSchedule()
        {
            var distributor = new ScheduleDistributor(
                ScheduleStorage.BuildStorageWithoutTimeline(),
                (s, id, e) => null);
            Assert.Throws<UnknownScheduleException>(() => distributor.Execute(new ScheduleId()));
        }

        [Test]
        public void ExecuteInProcess()
        {
            var action1 = new ScheduleElementId();
            var action2 = new ScheduleElementId();
            var exitCondition = new ScheduleElementId();
            var passThroughCondition = new ScheduleElementId();
            var subSchedule = new ScheduleId();

            var schedule = BuildSchedule(
                action1,
                action2,
                subSchedule,
                exitCondition,
                passThroughCondition);

            var knownSchedules = ScheduleStorage.BuildStorageWithoutTimeline();
            var scheduleInfo = knownSchedules.Add(
                schedule, 
                "a", 
                "b");

            ExecutableSchedule storedSchedule = null;
            var executor = new Mock<IExecuteSchedules>();
            Func<ExecutableSchedule, ScheduleId, ScheduleExecutionInfo, IExecuteSchedules> builder =
                (s, i, e) => 
                {
                    storedSchedule = s;
                    return executor.Object;
                };

            var distributor = new ScheduleDistributor(knownSchedules, builder);
            var returnedExecutor = distributor.Execute(scheduleInfo.Id);
            Assert.AreSame(executor.Object, returnedExecutor);
            VerifySchedule(schedule, storedSchedule);
        }

        [Test]
        [Ignore("Not implemented yet")]
        public void ExecuteOutOfProcess()
        { 
        }

        [Test]
        public void ExecuteWithAlreadyRunningSchedule()
        {
            var action1 = new ScheduleElementId();
            var action2 = new ScheduleElementId();
            var exitCondition = new ScheduleElementId();
            var passThroughCondition = new ScheduleElementId();
            var subSchedule = new ScheduleId();

            var schedule = BuildSchedule(
                action1,
                action2,
                subSchedule,
                exitCondition,
                passThroughCondition);

            var knownSchedules = ScheduleStorage.BuildStorageWithoutTimeline();
            var scheduleInfo = knownSchedules.Add(
                schedule, 
                "a", 
                "b");

            ExecutableSchedule storedSchedule = null;
            var executor = new Mock<IExecuteSchedules>();

            int index = 0;
            Func<ExecutableSchedule, ScheduleId, ScheduleExecutionInfo, IExecuteSchedules> builder =
                (s, id, e) =>
                {
                    storedSchedule = s;
                    index++;
                    return executor.Object;
                };

            var distributor = new ScheduleDistributor(knownSchedules, builder);
            var returnedExecutor = distributor.Execute(scheduleInfo.Id);
            Assert.AreSame(executor.Object, returnedExecutor);
            Assert.AreEqual(1, index);

            var otherReturnedExecutor = distributor.Execute(scheduleInfo.Id);
            Assert.AreSame(executor.Object, otherReturnedExecutor);
            Assert.AreEqual(1, index);
        }
    }
}
