//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MbUnit.Framework;
using QuickGraph;

namespace Apollo.Core.Extensions.Scheduling
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class ScheduleVertifierTest
    {
        [Test]
        public void StartVertexWithInboundEdges()
        {
            var knownSchedules = new Dictionary<ScheduleId, IEditableSchedule>();
            var verifier = new ScheduleVerifier(knownSchedules);

            EditableSchedule schedule = null;
            {
                var id = new ScheduleId();
                var graph = new BidirectionalGraph<IEditableScheduleVertex, EditableScheduleEdge>();

                var start = new EditableStartVertex(1);
                graph.AddVertex(start);

                var end = new EditableEndVertex(2);
                graph.AddVertex(end);

                var vertex1 = new EditableInsertVertex(3);
                graph.AddVertex(vertex1);

                var vertex2 = new EditableInsertVertex(4);
                graph.AddVertex(vertex2);

                graph.AddEdge(new EditableScheduleEdge(start, vertex1));
                graph.AddEdge(new EditableScheduleEdge(vertex1, vertex2));
                graph.AddEdge(new EditableScheduleEdge(vertex2, start));
                graph.AddEdge(new EditableScheduleEdge(vertex2, end));

                schedule = new EditableSchedule(id, graph, start, end);
            }

            var failures = new List<Tuple<ScheduleIntegrityFailureType, IEditableScheduleVertex>>();
            var result = verifier.IsValid(
                schedule,
                (f, v) => failures.Add(new Tuple<ScheduleIntegrityFailureType, IEditableScheduleVertex>(f, v)));

            Assert.IsFalse(result);
            Assert.AreEqual(1, failures.Count);
            Assert.AreEqual(ScheduleIntegrityFailureType.ScheduleIsMissingStart, failures[0].Item1);
            Assert.AreSame(schedule.Start, failures[0].Item2);
        }

        [Test]
        public void EndVertexWithOutboundEdges()
        {
            var knownSchedules = new Dictionary<ScheduleId, IEditableSchedule>();
            var verifier = new ScheduleVerifier(knownSchedules);

            EditableSchedule schedule = null;
            {
                var id = new ScheduleId();
                var graph = new BidirectionalGraph<IEditableScheduleVertex, EditableScheduleEdge>();

                var start = new EditableStartVertex(1);
                graph.AddVertex(start);

                var end = new EditableEndVertex(2);
                graph.AddVertex(end);

                var vertex1 = new EditableInsertVertex(3);
                graph.AddVertex(vertex1);

                var vertex2 = new EditableInsertVertex(4);
                graph.AddVertex(vertex2);

                graph.AddEdge(new EditableScheduleEdge(start, vertex1));
                graph.AddEdge(new EditableScheduleEdge(vertex1, vertex2));
                graph.AddEdge(new EditableScheduleEdge(vertex2, end));
                graph.AddEdge(new EditableScheduleEdge(end, vertex1));

                schedule = new EditableSchedule(id, graph, start, end);
            }

            var failures = new List<Tuple<ScheduleIntegrityFailureType, IEditableScheduleVertex>>();
            var result = verifier.IsValid(
                schedule,
                (f, v) => failures.Add(new Tuple<ScheduleIntegrityFailureType, IEditableScheduleVertex>(f, v)));

            Assert.IsFalse(result);
            Assert.AreEqual(1, failures.Count);
            Assert.AreEqual(ScheduleIntegrityFailureType.ScheduleIsMissingEnd, failures[0].Item1);
            Assert.AreSame(schedule.End, failures[0].Item2);
        }

        [Test]
        public void VertexWithNoEdges()
        {
            var knownSchedules = new Dictionary<ScheduleId, IEditableSchedule>();
            var verifier = new ScheduleVerifier(knownSchedules);

            EditableSchedule schedule = null;
            var id = new ScheduleId();
            var graph = new BidirectionalGraph<IEditableScheduleVertex, EditableScheduleEdge>();

            var start = new EditableStartVertex(1);
            graph.AddVertex(start);

            var end = new EditableEndVertex(2);
            graph.AddVertex(end);

            var vertex1 = new EditableInsertVertex(3);
            graph.AddVertex(vertex1);

            var vertex2 = new EditableInsertVertex(4);
            graph.AddVertex(vertex2);

            graph.AddEdge(new EditableScheduleEdge(start, vertex1));
            graph.AddEdge(new EditableScheduleEdge(vertex1, end));

            schedule = new EditableSchedule(id, graph, start, end);

            var failures = new List<Tuple<ScheduleIntegrityFailureType, IEditableScheduleVertex>>();
            var result = verifier.IsValid(
                schedule,
                (f, v) => failures.Add(new Tuple<ScheduleIntegrityFailureType, IEditableScheduleVertex>(f, v)));

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
            var knownSchedules = new Dictionary<ScheduleId, IEditableSchedule>();
            var verifier = new ScheduleVerifier(knownSchedules);

            EditableSchedule schedule = null;
            var id = new ScheduleId();
            var graph = new BidirectionalGraph<IEditableScheduleVertex, EditableScheduleEdge>();

            var start = new EditableStartVertex(1);
            graph.AddVertex(start);

            var end = new EditableEndVertex(2);
            graph.AddVertex(end);

            var vertex1 = new EditableInsertVertex(3);
            graph.AddVertex(vertex1);

            var vertex2 = new EditableInsertVertex(4);
            graph.AddVertex(vertex2);

            graph.AddEdge(new EditableScheduleEdge(start, vertex1));
            graph.AddEdge(new EditableScheduleEdge(vertex1, end));
            graph.AddEdge(new EditableScheduleEdge(vertex2, end));

            schedule = new EditableSchedule(id, graph, start, end);

            var failures = new List<Tuple<ScheduleIntegrityFailureType, IEditableScheduleVertex>>();
            var result = verifier.IsValid(
                schedule,
                (f, v) => failures.Add(new Tuple<ScheduleIntegrityFailureType, IEditableScheduleVertex>(f, v)));

            Assert.IsFalse(result);
            Assert.AreEqual(1, failures.Count);
            Assert.AreEqual(ScheduleIntegrityFailureType.ScheduleVertexIsNotReachableFromStart, failures[0].Item1);
            Assert.AreSame(vertex2, failures[0].Item2);
        }

        [Test]
        public void CycleWithNoEdgesToEnd()
        {
            var knownSchedules = new Dictionary<ScheduleId, IEditableSchedule>();
            var verifier = new ScheduleVerifier(knownSchedules);

            EditableSchedule schedule = null;
            var id = new ScheduleId();
            var graph = new BidirectionalGraph<IEditableScheduleVertex, EditableScheduleEdge>();

            var start = new EditableStartVertex(1);
            graph.AddVertex(start);

            var end = new EditableEndVertex(2);
            graph.AddVertex(end);

            var vertex1 = new EditableInsertVertex(3);
            graph.AddVertex(vertex1);

            var vertex2 = new EditableInsertVertex(4);
            graph.AddVertex(vertex2);

            var vertex3 = new EditableInsertVertex(5);
            graph.AddVertex(vertex3);

            graph.AddEdge(new EditableScheduleEdge(start, end));
            graph.AddEdge(new EditableScheduleEdge(start, vertex1));
            graph.AddEdge(new EditableScheduleEdge(vertex1, vertex2));
            graph.AddEdge(new EditableScheduleEdge(vertex2, vertex3));
            graph.AddEdge(new EditableScheduleEdge(vertex3, vertex1));

            schedule = new EditableSchedule(id, graph, start, end);

            var failures = new List<Tuple<ScheduleIntegrityFailureType, IEditableScheduleVertex>>();
            var result = verifier.IsValid(
                schedule,
                (f, v) => failures.Add(new Tuple<ScheduleIntegrityFailureType, IEditableScheduleVertex>(f, v)));

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
            var knownSchedules = new Dictionary<ScheduleId, IEditableSchedule>();
            var verifier = new ScheduleVerifier(knownSchedules);

            EditableSchedule schedule = null;
            var id = new ScheduleId();
            var graph = new BidirectionalGraph<IEditableScheduleVertex, EditableScheduleEdge>();

            var start = new EditableStartVertex(1);
            graph.AddVertex(start);

            var end = new EditableEndVertex(2);
            graph.AddVertex(end);

            var vertex1 = new EditableInsertVertex(3);
            graph.AddVertex(vertex1);

            var vertex2 = new EditableInsertVertex(4);
            graph.AddVertex(vertex2);

            graph.AddEdge(new EditableScheduleEdge(start, vertex1));
            graph.AddEdge(new EditableScheduleEdge(vertex1, end));
            graph.AddEdge(new EditableScheduleEdge(vertex1, vertex2));

            schedule = new EditableSchedule(id, graph, start, end);

            var failures = new List<Tuple<ScheduleIntegrityFailureType, IEditableScheduleVertex>>();
            var result = verifier.IsValid(
                schedule,
                (f, v) => failures.Add(new Tuple<ScheduleIntegrityFailureType, IEditableScheduleVertex>(f, v)));

            Assert.IsFalse(result);
            Assert.AreEqual(1, failures.Count);
            Assert.AreEqual(ScheduleIntegrityFailureType.ScheduleEndIsNotReachableFromVertex, failures[0].Item1);
            Assert.AreSame(vertex2, failures[0].Item2);
        }

        [Test]
        public void CycleWithNoEdgesFromStart()
        {
            var knownSchedules = new Dictionary<ScheduleId, IEditableSchedule>();
            var verifier = new ScheduleVerifier(knownSchedules);

            EditableSchedule schedule = null;
            var id = new ScheduleId();
            var graph = new BidirectionalGraph<IEditableScheduleVertex, EditableScheduleEdge>();

            var start = new EditableStartVertex(1);
            graph.AddVertex(start);

            var end = new EditableEndVertex(2);
            graph.AddVertex(end);

            var vertex1 = new EditableInsertVertex(3);
            graph.AddVertex(vertex1);

            var vertex2 = new EditableInsertVertex(4);
            graph.AddVertex(vertex2);

            var vertex3 = new EditableInsertVertex(5);
            graph.AddVertex(vertex3);

            graph.AddEdge(new EditableScheduleEdge(start, end));
            graph.AddEdge(new EditableScheduleEdge(vertex1, end));
            graph.AddEdge(new EditableScheduleEdge(vertex1, vertex2));
            graph.AddEdge(new EditableScheduleEdge(vertex2, vertex3));
            graph.AddEdge(new EditableScheduleEdge(vertex3, vertex1));

            schedule = new EditableSchedule(id, graph, start, end);

            var failures = new List<Tuple<ScheduleIntegrityFailureType, IEditableScheduleVertex>>();
            var result = verifier.IsValid(
                schedule,
                (f, v) => failures.Add(new Tuple<ScheduleIntegrityFailureType, IEditableScheduleVertex>(f, v)));

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
            var knownSchedules = new Dictionary<ScheduleId, IEditableSchedule>();
            var verifier = new ScheduleVerifier(knownSchedules);

            EditableSchedule schedule = null;
            var id = new ScheduleId();
            var graph = new BidirectionalGraph<IEditableScheduleVertex, EditableScheduleEdge>();

            var start = new EditableStartVertex(1);
            graph.AddVertex(start);

            var end = new EditableEndVertex(2);
            graph.AddVertex(end);

            var vertex1 = new EditableInsertVertex(3);
            graph.AddVertex(vertex1);

            graph.AddEdge(new EditableScheduleEdge(start, vertex1));
            graph.AddEdge(new EditableScheduleEdge(vertex1, end));
            graph.AddEdge(new EditableScheduleEdge(vertex1, end));

            schedule = new EditableSchedule(id, graph, start, end);

            var failures = new List<Tuple<ScheduleIntegrityFailureType, IEditableScheduleVertex>>();
            var result = verifier.IsValid(
                schedule,
                (f, v) => failures.Add(new Tuple<ScheduleIntegrityFailureType, IEditableScheduleVertex>(f, v)));

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
            var knownSchedules = new Dictionary<ScheduleId, IEditableSchedule>();

            EditableSchedule subSchedule = null;
            {
                var subScheduleId = new ScheduleId();
                var subGraph = new BidirectionalGraph<IEditableScheduleVertex, EditableScheduleEdge>();
                var start = new EditableStartVertex(1);
                var end = new EditableEndVertex(2);
                var vertex1 = new EditableSubScheduleVertex(3, id);
                subGraph.AddVertex(start);
                subGraph.AddVertex(end);
                subGraph.AddVertex(vertex1);
                subGraph.AddEdge(new EditableScheduleEdge(start, vertex1));
                subGraph.AddEdge(new EditableScheduleEdge(vertex1, end));
                subSchedule = new EditableSchedule(subScheduleId, subGraph, start, end);
                
                knownSchedules.Add(subScheduleId, subSchedule);
            }

            IEditableScheduleVertex errorVertex = null;
            EditableSchedule schedule = null;
            {
                var graph = new BidirectionalGraph<IEditableScheduleVertex, EditableScheduleEdge>();
                var start = new EditableStartVertex(1);
                var end = new EditableEndVertex(2);
                var vertex1 = new EditableSubScheduleVertex(3, subSchedule.Id);
                graph.AddVertex(start);
                graph.AddVertex(end);
                graph.AddVertex(vertex1);
                graph.AddEdge(new EditableScheduleEdge(start, vertex1));
                graph.AddEdge(new EditableScheduleEdge(vertex1, end));
                schedule = new EditableSchedule(id, graph, start, end);

                errorVertex = vertex1;
            }

            var failures = new List<Tuple<ScheduleIntegrityFailureType, IEditableScheduleVertex>>();

            var verifier = new ScheduleVerifier(knownSchedules);
            var result = verifier.IsValid(
                schedule,
                (f, v) => failures.Add(new Tuple<ScheduleIntegrityFailureType, IEditableScheduleVertex>(f, v)));

            Assert.IsFalse(result);
            Assert.AreEqual(1, failures.Count);
            Assert.AreEqual(ScheduleIntegrityFailureType.SubScheduleLinksBackToParentSchedule, failures[0].Item1);
            Assert.AreSame(errorVertex, failures[0].Item2);
        }

        [Test]
        public void NestedSubScheduleWithLinkBackToRootSchedule()
        {
            var id = new ScheduleId();
            var knownSchedules = new Dictionary<ScheduleId, IEditableSchedule>();

            EditableSchedule subSubSchedule = null;
            {
                var subScheduleId = new ScheduleId();
                var subGraph = new BidirectionalGraph<IEditableScheduleVertex, EditableScheduleEdge>();
                var start = new EditableStartVertex(1);
                var end = new EditableEndVertex(2);
                var vertex1 = new EditableSubScheduleVertex(3, id);
                subGraph.AddVertex(start);
                subGraph.AddVertex(end);
                subGraph.AddVertex(vertex1);
                subGraph.AddEdge(new EditableScheduleEdge(start, vertex1));
                subGraph.AddEdge(new EditableScheduleEdge(vertex1, end));
                subSubSchedule = new EditableSchedule(subScheduleId, subGraph, start, end);

                knownSchedules.Add(subScheduleId, subSubSchedule);
            }

            EditableSchedule subSchedule = null;
            {
                var subScheduleId = new ScheduleId();
                var subGraph = new BidirectionalGraph<IEditableScheduleVertex, EditableScheduleEdge>();
                var start = new EditableStartVertex(1);
                var end = new EditableEndVertex(2);
                var vertex1 = new EditableSubScheduleVertex(3, subSubSchedule.Id);
                subGraph.AddVertex(start);
                subGraph.AddVertex(end);
                subGraph.AddVertex(vertex1);
                subGraph.AddEdge(new EditableScheduleEdge(start, vertex1));
                subGraph.AddEdge(new EditableScheduleEdge(vertex1, end));
                subSchedule = new EditableSchedule(subScheduleId, subGraph, start, end);

                knownSchedules.Add(subScheduleId, subSchedule);
            }

            IEditableScheduleVertex errorVertex = null;
            EditableSchedule schedule = null;
            {
                var graph = new BidirectionalGraph<IEditableScheduleVertex, EditableScheduleEdge>();
                var start = new EditableStartVertex(1);
                var end = new EditableEndVertex(2);
                var vertex1 = new EditableSubScheduleVertex(3, subSchedule.Id);
                graph.AddVertex(start);
                graph.AddVertex(end);
                graph.AddVertex(vertex1);
                graph.AddEdge(new EditableScheduleEdge(start, vertex1));
                graph.AddEdge(new EditableScheduleEdge(vertex1, end));
                schedule = new EditableSchedule(id, graph, start, end);

                errorVertex = vertex1;
            }

            var failures = new List<Tuple<ScheduleIntegrityFailureType, IEditableScheduleVertex>>();

            var verifier = new ScheduleVerifier(knownSchedules);
            var result = verifier.IsValid(
                schedule,
                (f, v) => failures.Add(new Tuple<ScheduleIntegrityFailureType, IEditableScheduleVertex>(f, v)));

            Assert.IsFalse(result);
            Assert.AreEqual(1, failures.Count);
            Assert.AreEqual(ScheduleIntegrityFailureType.SubScheduleLinksBackToParentSchedule, failures[0].Item1);
            Assert.AreSame(errorVertex, failures[0].Item2);
        }
    }
}
