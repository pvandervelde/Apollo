//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Extensions.Scheduling;
using MbUnit.Framework;
using QuickGraph;

namespace Apollo.Core.Base.Scheduling
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class EditableScheduleTest
    {
        [Test]
        public void Create()
        {
            var graph = new BidirectionalGraph<IScheduleVertex, ScheduleEdge>();

            var start = new StartVertex(1);
            graph.AddVertex(start);

            var end = new EndVertex(2);
            graph.AddVertex(end);
            graph.AddEdge(new ScheduleEdge(start, end));

            var schedule = new Schedule(graph, start, end);

            Assert.AreSame(start, schedule.Start);
            Assert.AreSame(end, schedule.End);
            Assert.AreElementsSameIgnoringOrder(new IScheduleVertex[] { start, end }, schedule.Vertices);
            Assert.AreEqual(1, schedule.NumberOfOutboundConnections(start));
            Assert.AreEqual(1, schedule.NumberOfInboundConnections(end));
        }

        [Test]
        public void TraverseSchedulePartially()
        {
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

                graph.AddEdge(new ScheduleEdge(start, vertex1));
                graph.AddEdge(new ScheduleEdge(vertex1, vertex2));
                graph.AddEdge(new ScheduleEdge(vertex2, end));

                schedule = new Schedule(graph, start, end);
            }

            var vertices = new List<int>();
            schedule.TraverseAllScheduleVertices(
                schedule.Start,
                (vertex, edges) =>
                {
                    vertices.Add(vertex.Index);
                    return vertex.Index != 3;
                });

            Assert.AreElementsEqual(new int[] { 1, 3 }, vertices);
        }

        [Test]
        public void TraverseScheduleCompletely()
        {
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

                graph.AddEdge(new ScheduleEdge(start, vertex1));
                graph.AddEdge(new ScheduleEdge(vertex1, vertex2));
                graph.AddEdge(new ScheduleEdge(vertex2, end));

                schedule = new Schedule(graph, start, end);
            }

            var vertices = new List<int>();
            schedule.TraverseAllScheduleVertices(
                schedule.Start,
                (vertex, edges) =>
                {
                    vertices.Add(vertex.Index);
                    return true;
                });

            Assert.AreElementsEqual(new int[] { 1, 3, 4, 2 }, vertices);
        }

        [Test]
        public void TraverseScheduleWithLoop()
        {
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

                var vertex3 = new InsertVertex(5);
                graph.AddVertex(vertex3);

                graph.AddEdge(new ScheduleEdge(start, vertex1));
                graph.AddEdge(new ScheduleEdge(vertex1, vertex2));

                graph.AddEdge(new ScheduleEdge(vertex2, end));
                graph.AddEdge(new ScheduleEdge(vertex2, vertex3));

                graph.AddEdge(new ScheduleEdge(vertex3, vertex1));

                schedule = new Schedule(graph, start, end);
            }

            var vertices = new List<int>();
            schedule.TraverseAllScheduleVertices(
                schedule.Start,
                (vertex, edges) =>
                {
                    vertices.Add(vertex.Index);
                    return true;
                });

            Assert.AreElementsEqual(new int[] { 1, 3, 4, 2, 5 }, vertices);
        }

        [Test]
        public void TraverseScheduleWithInnerAndOuterLoop()
        {
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

                var vertex4 = new InsertVertex(6);
                graph.AddVertex(vertex4);

                var vertex5 = new InsertVertex(7);
                graph.AddVertex(vertex5);

                graph.AddEdge(new ScheduleEdge(start, vertex1));
                graph.AddEdge(new ScheduleEdge(vertex1, vertex2));

                graph.AddEdge(new ScheduleEdge(vertex2, end));
                graph.AddEdge(new ScheduleEdge(vertex2, vertex3));

                graph.AddEdge(new ScheduleEdge(vertex3, vertex1));
                graph.AddEdge(new ScheduleEdge(vertex3, vertex4));

                graph.AddEdge(new ScheduleEdge(vertex4, vertex5));
                graph.AddEdge(new ScheduleEdge(vertex5, vertex3));

                schedule = new Schedule(graph, start, end);
            }

            var vertices = new List<int>();
            schedule.TraverseAllScheduleVertices(
                schedule.Start,
                (vertex, edges) =>
                {
                    vertices.Add(vertex.Index);
                    return true;
                });

            Assert.AreElementsEqual(new int[] { 1, 3, 4, 2, 5, 6, 7 }, vertices);
        }

        [Test]
        public void RoundTripSerialise()
        {
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

                var vertex4 = new InsertVertex(6);
                graph.AddVertex(vertex4);

                var vertex5 = new InsertVertex(7);
                graph.AddVertex(vertex5);

                graph.AddEdge(new ScheduleEdge(start, vertex1));
                graph.AddEdge(new ScheduleEdge(vertex1, vertex2));

                graph.AddEdge(new ScheduleEdge(vertex2, end));
                graph.AddEdge(new ScheduleEdge(vertex2, vertex3));

                graph.AddEdge(new ScheduleEdge(vertex3, vertex1));
                graph.AddEdge(new ScheduleEdge(vertex3, vertex4));

                graph.AddEdge(new ScheduleEdge(vertex4, vertex5));
                graph.AddEdge(new ScheduleEdge(vertex5, vertex3));

                schedule = new Schedule(graph, start, end);
            }

            var otherSchedule = Assert.BinarySerializeThenDeserialize(schedule);
            var vertices = new List<int>();
            otherSchedule.TraverseAllScheduleVertices(
                otherSchedule.Start,
                (vertex, edges) =>
                {
                    vertices.Add(vertex.Index);
                    return true;
                });

            Assert.AreElementsEqual(new int[] { 1, 3, 4, 2, 5, 6, 7 }, vertices);
        }
    }
}
