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
using Moq;
using NUnit.Framework;
using QuickGraph;

namespace Apollo.Core.Dataset.Scheduling
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class ScheduleDistributorTest
    {
        private static Schedule BuildSchedule(
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
            Schedule schedule = null;
            {
                var graph = new BidirectionalGraph<IScheduleVertex, ScheduleEdge>();

                var start = new StartVertex(1);
                graph.AddVertex(start);

                var end = new EndVertex(2);
                graph.AddVertex(end);

                var vertex1 = new ExecutingActionVertex(3, action1);
                graph.AddVertex(vertex1);

                var vertex2 = new ExecutingActionVertex(4, action2);
                graph.AddVertex(vertex2);

                var vertex3 = new SynchronizationStartVertex(5, new IScheduleVariable[] { variable.Object });
                graph.AddVertex(vertex3);

                var vertex4 = new ExecutingActionVertex(6, action2);
                graph.AddVertex(vertex4);

                var vertex5 = new SynchronizationEndVertex(7);
                graph.AddVertex(vertex5);

                var vertex6 = new SubScheduleVertex(8, scheduleId);
                graph.AddVertex(vertex6);

                var vertex7 = new InsertVertex(9);
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

                schedule = new Schedule(graph, start, end);
            }

            return schedule;
        }

        private static void VerifySchedule(ISchedule first, ISchedule second)
        {
            var linearisedEditableGraph = new List<Tuple<IScheduleVertex, List<Tuple<ScheduleElementId, IScheduleVertex>>>>();
            first.TraverseAllScheduleVertices(
                first.Start, 
                (vertex, edges) =>
                {
                    linearisedEditableGraph.Add(
                        new Tuple<IScheduleVertex, List<Tuple<ScheduleElementId, IScheduleVertex>>>(
                            vertex,
                            edges.ToList()));

                    return true;
                });

            var linearisedExecutableGraph = new List<Tuple<IScheduleVertex, List<Tuple<ScheduleElementId, IScheduleVertex>>>>();
            second.TraverseAllScheduleVertices(
                second.Start,
                (vertex, edges) =>
                {
                    linearisedExecutableGraph.Add(
                        new Tuple<IScheduleVertex, List<Tuple<ScheduleElementId, IScheduleVertex>>>(
                            vertex,
                            edges.ToList()));

                    return true;
                });

            CompareLinearisedGraphs(linearisedEditableGraph, linearisedExecutableGraph);
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
                Assert.AreEqual(expectedPair.Item1.GetType(), givenPair.Item1.GetType());

                Assert.AreEqual(expectedPair.Item2.Count, givenPair.Item2.Count);
                for (int j = 0; j < expectedPair.Item2.Count; j++)
                {
                    Assert.AreEqual(expectedPair.Item2[j].Item1, givenPair.Item2[j].Item1);
                    Assert.AreEqual(expectedPair.Item2[j].Item2.Index, givenPair.Item2[j].Item2.Index);
                }
            }
        }

        [Test]
        public void ExecuteWithUnknownSchedule()
        {
            var distributor = new ScheduleDistributor(
                ScheduleStorage.CreateInstanceWithoutTimeline(),
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

            var knownSchedules = ScheduleStorage.CreateInstanceWithoutTimeline();
            var scheduleInfo = knownSchedules.Add(
                schedule, 
                "a", 
                "b");

            ISchedule storedSchedule = null;
            var executor = new Mock<IExecuteSchedules>();
            Func<ISchedule, ScheduleId, ScheduleExecutionInfo, IExecuteSchedules> builder =
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

            var knownSchedules = ScheduleStorage.CreateInstanceWithoutTimeline();
            var scheduleInfo = knownSchedules.Add(
                schedule, 
                "a", 
                "b");

            var executor = new Mock<IExecuteSchedules>();

            int index = 0;
            Func<ISchedule, ScheduleId, ScheduleExecutionInfo, IExecuteSchedules> builder =
                (s, id, e) =>
                {
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
