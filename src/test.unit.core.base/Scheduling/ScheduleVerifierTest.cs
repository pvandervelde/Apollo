//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Extensions.Scheduling;
using Moq;
using NUnit.Framework;
using QuickGraph;

namespace Apollo.Core.Base.Scheduling
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class ScheduleVerifierTest
    {
        [Test]
        public void StartVertexWithInboundEdges()
        {
            var knownSchedules = new Mock<IStoreSchedules>();
            var verifier = new ScheduleVerifier(knownSchedules.Object);

            Schedule schedule;
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

                graph.AddEdge(new ScheduleEdge(start, vertex1));
                graph.AddEdge(new ScheduleEdge(vertex1, vertex2));
                graph.AddEdge(new ScheduleEdge(vertex2, start));
                graph.AddEdge(new ScheduleEdge(vertex2, end));

                schedule = new Schedule(graph, start, end);
            }

            var id = new ScheduleId();
            var failures = new List<Tuple<ScheduleIntegrityFailureType, IScheduleVertex>>();
            var result = verifier.IsValid(
                id,
                schedule,
                (f, v) => failures.Add(new Tuple<ScheduleIntegrityFailureType, IScheduleVertex>(f, v)));

            Assert.IsFalse(result);
            Assert.AreEqual(1, failures.Count);
            Assert.AreEqual(ScheduleIntegrityFailureType.ScheduleIsMissingStart, failures[0].Item1);
            Assert.AreSame(schedule.Start, failures[0].Item2);
        }

        [Test]
        public void EndVertexWithOutboundEdges()
        {
            var knownSchedules = new Mock<IStoreSchedules>();
            var verifier = new ScheduleVerifier(knownSchedules.Object);

            Schedule schedule;
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

                graph.AddEdge(new ScheduleEdge(start, vertex1));
                graph.AddEdge(new ScheduleEdge(vertex1, vertex2));
                graph.AddEdge(new ScheduleEdge(vertex2, end));
                graph.AddEdge(new ScheduleEdge(end, vertex1));

                schedule = new Schedule(graph, start, end);
            }

            var id = new ScheduleId();
            var failures = new List<Tuple<ScheduleIntegrityFailureType, IScheduleVertex>>();
            var result = verifier.IsValid(
                id,
                schedule,
                (f, v) => failures.Add(new Tuple<ScheduleIntegrityFailureType, IScheduleVertex>(f, v)));

            Assert.IsFalse(result);
            Assert.AreEqual(1, failures.Count);
            Assert.AreEqual(ScheduleIntegrityFailureType.ScheduleIsMissingEnd, failures[0].Item1);
            Assert.AreSame(schedule.End, failures[0].Item2);
        }

        [Test]
        public void VertexWithNoEdges()
        {
            var knownSchedules = new Mock<IStoreSchedules>();
            var verifier = new ScheduleVerifier(knownSchedules.Object);

            var graph = new BidirectionalGraph<IScheduleVertex, ScheduleEdge>();

            var start = new StartVertex(1);
            graph.AddVertex(start);

            var end = new EndVertex(2);
            graph.AddVertex(end);

            var vertex1 = new InsertVertex(3);
            graph.AddVertex(vertex1);

            var vertex2 = new InsertVertex(4);
            graph.AddVertex(vertex2);

            graph.AddEdge(new ScheduleEdge(start, vertex1));
            graph.AddEdge(new ScheduleEdge(vertex1, end));

            var schedule = new Schedule(graph, start, end);

            var id = new ScheduleId();
            var failures = new List<Tuple<ScheduleIntegrityFailureType, IScheduleVertex>>();
            var result = verifier.IsValid(
                id,
                schedule,
                (f, v) => failures.Add(new Tuple<ScheduleIntegrityFailureType, IScheduleVertex>(f, v)));

            Assert.IsFalse(result);
            Assert.AreEqual(2, failures.Count);
            Assert.AreEqual(ScheduleIntegrityFailureType.ScheduleVertexIsNotReachableFromStart, failures[0].Item1);
            Assert.AreSame(vertex2, failures[0].Item2);

            Assert.AreEqual(ScheduleIntegrityFailureType.ScheduleEndIsNotReachableFromVertex, failures[1].Item1);
            Assert.AreSame(vertex2, failures[1].Item2);
        }

        [Test]
        public void NonStartVertexWithOutboundEdges()
        {
            var knownSchedules = new Mock<IStoreSchedules>();
            var verifier = new ScheduleVerifier(knownSchedules.Object);

            var graph = new BidirectionalGraph<IScheduleVertex, ScheduleEdge>();

            var start = new StartVertex(1);
            graph.AddVertex(start);

            var end = new EndVertex(2);
            graph.AddVertex(end);

            var vertex1 = new InsertVertex(3);
            graph.AddVertex(vertex1);

            var vertex2 = new InsertVertex(4);
            graph.AddVertex(vertex2);

            graph.AddEdge(new ScheduleEdge(start, vertex1));
            graph.AddEdge(new ScheduleEdge(vertex1, end));
            graph.AddEdge(new ScheduleEdge(vertex2, end));

            var schedule = new Schedule(graph, start, end);

            var id = new ScheduleId();
            var failures = new List<Tuple<ScheduleIntegrityFailureType, IScheduleVertex>>();
            var result = verifier.IsValid(
                id,
                schedule,
                (f, v) => failures.Add(new Tuple<ScheduleIntegrityFailureType, IScheduleVertex>(f, v)));

            Assert.IsFalse(result);
            Assert.AreEqual(1, failures.Count);
            Assert.AreEqual(ScheduleIntegrityFailureType.ScheduleVertexIsNotReachableFromStart, failures[0].Item1);
            Assert.AreSame(vertex2, failures[0].Item2);
        }

        [Test]
        public void CycleWithNoEdgesToEnd()
        {
            var knownSchedules = new Mock<IStoreSchedules>();
            var verifier = new ScheduleVerifier(knownSchedules.Object);

            var graph = new BidirectionalGraph<IScheduleVertex, ScheduleEdge>();

            var start = new StartVertex(1);
            graph.AddVertex(start);

            var end = new EndVertex(2);
            graph.AddVertex(end);

            var vertex1 = new InsertVertex(3);
            graph.AddVertex(vertex1);

            var vertex2 = new InsertVertex(4);
            graph.AddVertex(vertex2);

            var vertex3 = new InsertVertex(5);
            graph.AddVertex(vertex3);

            graph.AddEdge(new ScheduleEdge(start, end));
            graph.AddEdge(new ScheduleEdge(start, vertex1));
            graph.AddEdge(new ScheduleEdge(vertex1, vertex2));
            graph.AddEdge(new ScheduleEdge(vertex2, vertex3));
            graph.AddEdge(new ScheduleEdge(vertex3, vertex1));

            var schedule = new Schedule(graph, start, end);

            var id = new ScheduleId();
            var failures = new List<Tuple<ScheduleIntegrityFailureType, IScheduleVertex>>();
            var result = verifier.IsValid(
                id,
                schedule,
                (f, v) => failures.Add(new Tuple<ScheduleIntegrityFailureType, IScheduleVertex>(f, v)));

            Assert.IsFalse(result);
            Assert.AreEqual(3, failures.Count);
            Assert.AreEqual(ScheduleIntegrityFailureType.ScheduleEndIsNotReachableFromVertex, failures[0].Item1);
            Assert.AreSame(vertex1, failures[0].Item2);

            Assert.AreEqual(ScheduleIntegrityFailureType.ScheduleEndIsNotReachableFromVertex, failures[1].Item1);
            Assert.AreSame(vertex2, failures[1].Item2);

            Assert.AreEqual(ScheduleIntegrityFailureType.ScheduleEndIsNotReachableFromVertex, failures[2].Item1);
            Assert.AreSame(vertex3, failures[2].Item2);
        }

        [Test]
        public void NonEndVertexWithOnlyInboundEdges()
        {
            var knownSchedules = new Mock<IStoreSchedules>();
            var verifier = new ScheduleVerifier(knownSchedules.Object);

            var graph = new BidirectionalGraph<IScheduleVertex, ScheduleEdge>();

            var start = new StartVertex(1);
            graph.AddVertex(start);

            var end = new EndVertex(2);
            graph.AddVertex(end);

            var vertex1 = new InsertVertex(3);
            graph.AddVertex(vertex1);

            var vertex2 = new InsertVertex(4);
            graph.AddVertex(vertex2);

            graph.AddEdge(new ScheduleEdge(start, vertex1));
            graph.AddEdge(new ScheduleEdge(vertex1, end));
            graph.AddEdge(new ScheduleEdge(vertex1, vertex2));

            var schedule = new Schedule(graph, start, end);

            var id = new ScheduleId();
            var failures = new List<Tuple<ScheduleIntegrityFailureType, IScheduleVertex>>();
            var result = verifier.IsValid(
                id,
                schedule,
                (f, v) => failures.Add(new Tuple<ScheduleIntegrityFailureType, IScheduleVertex>(f, v)));

            Assert.IsFalse(result);
            Assert.AreEqual(1, failures.Count);
            Assert.AreEqual(ScheduleIntegrityFailureType.ScheduleEndIsNotReachableFromVertex, failures[0].Item1);
            Assert.AreSame(vertex2, failures[0].Item2);
        }

        [Test]
        public void CycleWithNoEdgesFromStart()
        {
            var knownSchedules = new Mock<IStoreSchedules>();
            var verifier = new ScheduleVerifier(knownSchedules.Object);

            var graph = new BidirectionalGraph<IScheduleVertex, ScheduleEdge>();

            var start = new StartVertex(1);
            graph.AddVertex(start);

            var end = new EndVertex(2);
            graph.AddVertex(end);

            var vertex1 = new InsertVertex(3);
            graph.AddVertex(vertex1);

            var vertex2 = new InsertVertex(4);
            graph.AddVertex(vertex2);

            var vertex3 = new InsertVertex(5);
            graph.AddVertex(vertex3);

            graph.AddEdge(new ScheduleEdge(start, end));
            graph.AddEdge(new ScheduleEdge(vertex1, end));
            graph.AddEdge(new ScheduleEdge(vertex1, vertex2));
            graph.AddEdge(new ScheduleEdge(vertex2, vertex3));
            graph.AddEdge(new ScheduleEdge(vertex3, vertex1));

            var schedule = new Schedule(graph, start, end);

            var id = new ScheduleId();
            var failures = new List<Tuple<ScheduleIntegrityFailureType, IScheduleVertex>>();
            var result = verifier.IsValid(
                id,
                schedule,
                (f, v) => failures.Add(new Tuple<ScheduleIntegrityFailureType, IScheduleVertex>(f, v)));

            Assert.IsFalse(result);
            Assert.AreEqual(3, failures.Count);
            Assert.AreEqual(ScheduleIntegrityFailureType.ScheduleVertexIsNotReachableFromStart, failures[0].Item1);
            Assert.AreSame(vertex1, failures[0].Item2);

            Assert.AreEqual(ScheduleIntegrityFailureType.ScheduleVertexIsNotReachableFromStart, failures[1].Item1);
            Assert.AreSame(vertex2, failures[1].Item2);

            Assert.AreEqual(ScheduleIntegrityFailureType.ScheduleVertexIsNotReachableFromStart, failures[2].Item1);
            Assert.AreSame(vertex3, failures[2].Item2);
        }

        [Test]
        public void VertexWithMultipleEdgesInOneDirection()
        {
            var knownSchedules = new Mock<IStoreSchedules>();
            var verifier = new ScheduleVerifier(knownSchedules.Object);

            var graph = new BidirectionalGraph<IScheduleVertex, ScheduleEdge>();

            var start = new StartVertex(1);
            graph.AddVertex(start);

            var end = new EndVertex(2);
            graph.AddVertex(end);

            var vertex1 = new InsertVertex(3);
            graph.AddVertex(vertex1);

            graph.AddEdge(new ScheduleEdge(start, vertex1));
            graph.AddEdge(new ScheduleEdge(vertex1, end));
            graph.AddEdge(new ScheduleEdge(vertex1, end));

            var schedule = new Schedule(graph, start, end);

            var id = new ScheduleId();
            var failures = new List<Tuple<ScheduleIntegrityFailureType, IScheduleVertex>>();
            var result = verifier.IsValid(
                id,
                schedule,
                (f, v) => failures.Add(new Tuple<ScheduleIntegrityFailureType, IScheduleVertex>(f, v)));

            Assert.IsFalse(result);
            Assert.AreEqual(1, failures.Count);
            Assert.AreEqual(ScheduleIntegrityFailureType.VertexLinksToOtherVertexInMultipleWays, failures[0].Item1);
            Assert.AreSame(vertex1, failures[0].Item2);
        }

        [Test]
        [Ignore("Not implemented yet")]
        public void SynchronizationBlockWithNoStart()
        { 
        }

        [Test]
        [Ignore("Not implemented yet")]
        public void SynchronizationBlockWithNoEnd()
        { 
        }

        [Test]
        [Ignore("Not implemented yet")]
        public void SynchronizationBlockWithNonUpdatingVariables()
        { 
        }

        [Test]
        public void SubScheduleWithLinkBackToParent()
        {
            var id = new ScheduleId();

            var subScheduleId = new ScheduleId();
            ISchedule subSchedule;
            {
                var subGraph = new BidirectionalGraph<IScheduleVertex, ScheduleEdge>();
                var start = new StartVertex(1);
                var end = new EndVertex(2);
                var vertex1 = new SubScheduleVertex(3, id);
                subGraph.AddVertex(start);
                subGraph.AddVertex(end);
                subGraph.AddVertex(vertex1);
                subGraph.AddEdge(new ScheduleEdge(start, vertex1));
                subGraph.AddEdge(new ScheduleEdge(vertex1, end));
                subSchedule = new Schedule(subGraph, start, end);
            }

            IScheduleVertex errorVertex;
            Schedule schedule;
            {
                var graph = new BidirectionalGraph<IScheduleVertex, ScheduleEdge>();
                var start = new StartVertex(1);
                var end = new EndVertex(2);
                var vertex1 = new SubScheduleVertex(3, subScheduleId);
                graph.AddVertex(start);
                graph.AddVertex(end);
                graph.AddVertex(vertex1);
                graph.AddEdge(new ScheduleEdge(start, vertex1));
                graph.AddEdge(new ScheduleEdge(vertex1, end));
                schedule = new Schedule(graph, start, end);

                errorVertex = vertex1;
            }
            
            var knownSchedules = new Mock<IStoreSchedules>();
            {
                knownSchedules.Setup(s => s.Contains(It.IsAny<ScheduleId>()))
                    .Returns<ScheduleId>(subScheduleId.Equals);
                knownSchedules.Setup(s => s.Schedule(It.IsAny<ScheduleId>()))
                    .Returns<ScheduleId>(scheduleId => subSchedule);
            }

            var failures = new List<Tuple<ScheduleIntegrityFailureType, IScheduleVertex>>();

            var verifier = new ScheduleVerifier(knownSchedules.Object);
            var result = verifier.IsValid(
                id,
                schedule,
                (f, v) => failures.Add(new Tuple<ScheduleIntegrityFailureType, IScheduleVertex>(f, v)));

            Assert.IsFalse(result);
            Assert.AreEqual(1, failures.Count);
            Assert.AreEqual(ScheduleIntegrityFailureType.SubScheduleLinksBackToParentSchedule, failures[0].Item1);
            Assert.AreSame(errorVertex, failures[0].Item2);
        }

        [Test]
        public void NestedSubScheduleWithLinkBackToRootSchedule()
        {
            var id = new ScheduleId();

            var subSubScheduleId = new ScheduleId();
            ISchedule subSubSchedule;
            {
                var subGraph = new BidirectionalGraph<IScheduleVertex, ScheduleEdge>();
                var start = new StartVertex(1);
                var end = new EndVertex(2);
                var vertex1 = new SubScheduleVertex(3, id);
                subGraph.AddVertex(start);
                subGraph.AddVertex(end);
                subGraph.AddVertex(vertex1);
                subGraph.AddEdge(new ScheduleEdge(start, vertex1));
                subGraph.AddEdge(new ScheduleEdge(vertex1, end));
                subSubSchedule = new Schedule(subGraph, start, end);
            }

            var subScheduleId = new ScheduleId();
            ISchedule subSchedule;
            {
                var subGraph = new BidirectionalGraph<IScheduleVertex, ScheduleEdge>();
                var start = new StartVertex(1);
                var end = new EndVertex(2);
                var vertex1 = new SubScheduleVertex(3, subSubScheduleId);
                subGraph.AddVertex(start);
                subGraph.AddVertex(end);
                subGraph.AddVertex(vertex1);
                subGraph.AddEdge(new ScheduleEdge(start, vertex1));
                subGraph.AddEdge(new ScheduleEdge(vertex1, end));
                subSchedule = new Schedule(subGraph, start, end);
            }

            IScheduleVertex errorVertex;
            Schedule schedule;
            {
                var graph = new BidirectionalGraph<IScheduleVertex, ScheduleEdge>();
                var start = new StartVertex(1);
                var end = new EndVertex(2);
                var vertex1 = new SubScheduleVertex(3, subScheduleId);
                graph.AddVertex(start);
                graph.AddVertex(end);
                graph.AddVertex(vertex1);
                graph.AddEdge(new ScheduleEdge(start, vertex1));
                graph.AddEdge(new ScheduleEdge(vertex1, end));
                schedule = new Schedule(graph, start, end);

                errorVertex = vertex1;
            }

            var knownSchedules = new Mock<IStoreSchedules>();
            {
                knownSchedules.Setup(s => s.Contains(It.IsAny<ScheduleId>()))
                    .Returns<ScheduleId>(scheduleId => subScheduleId.Equals(scheduleId) || subSubScheduleId.Equals(scheduleId));
                knownSchedules.Setup(s => s.Schedule(It.IsAny<ScheduleId>()))
                    .Returns<ScheduleId>(
                        scheduleId => 
                        {
                            return subSubScheduleId.Equals(scheduleId) ? subSubSchedule : subSchedule;
                        });
            }

            var failures = new List<Tuple<ScheduleIntegrityFailureType, IScheduleVertex>>();

            var verifier = new ScheduleVerifier(knownSchedules.Object);
            var result = verifier.IsValid(
                id,
                schedule,
                (f, v) => failures.Add(new Tuple<ScheduleIntegrityFailureType, IScheduleVertex>(f, v)));

            Assert.IsFalse(result);
            Assert.AreEqual(1, failures.Count);
            Assert.AreEqual(ScheduleIntegrityFailureType.SubScheduleLinksBackToParentSchedule, failures[0].Item1);
            Assert.AreSame(errorVertex, failures[0].Item2);
        }
    }
}
