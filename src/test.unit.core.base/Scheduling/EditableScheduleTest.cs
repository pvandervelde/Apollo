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

            var start = new EditableStartVertex(1);
            graph.AddVertex(start);

            var end = new EditableEndVertex(2);
            graph.AddVertex(end);
            graph.AddEdge(new ScheduleEdge(start, end));

            var schedule = new EditableSchedule(graph, start, end);

            Assert.AreSame(start, schedule.Start);
            Assert.AreSame(end, schedule.End);
            Assert.AreElementsSameIgnoringOrder(new IScheduleVertex[] { start, end }, schedule.Vertices);
            Assert.AreEqual(1, schedule.NumberOfOutboundConnections(start));
            Assert.AreEqual(1, schedule.NumberOfInboundConnections(end));
        }

        [Test]
        public void TraverseSchedulePartially()
        {
            EditableSchedule schedule = null;
            {
                var graph = new BidirectionalGraph<IScheduleVertex, ScheduleEdge>();

                var start = new EditableStartVertex(1);
                graph.AddVertex(start);

                var end = new EditableEndVertex(2);
                graph.AddVertex(end);

                var vertex1 = new EditableInsertVertex(3);
                graph.AddVertex(vertex1);

                var vertex2 = new EditableInsertVertex(4);
                graph.AddVertex(vertex2);

                graph.AddEdge(new ScheduleEdge(start, vertex1));
                graph.AddEdge(new ScheduleEdge(vertex1, vertex2));
                graph.AddEdge(new ScheduleEdge(vertex2, end));

                schedule = new EditableSchedule(graph, start, end);
            }

            var vertices = new List<int>();
            schedule.TraverseSchedule(
                schedule.Start,
                true,
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
            EditableSchedule schedule = null;
            {
                var graph = new BidirectionalGraph<IScheduleVertex, ScheduleEdge>();

                var start = new EditableStartVertex(1);
                graph.AddVertex(start);

                var end = new EditableEndVertex(2);
                graph.AddVertex(end);

                var vertex1 = new EditableInsertVertex(3);
                graph.AddVertex(vertex1);

                var vertex2 = new EditableInsertVertex(4);
                graph.AddVertex(vertex2);

                graph.AddEdge(new ScheduleEdge(start, vertex1));
                graph.AddEdge(new ScheduleEdge(vertex1, vertex2));
                graph.AddEdge(new ScheduleEdge(vertex2, end));

                schedule = new EditableSchedule(graph, start, end);
            }

            var vertices = new List<int>();
            schedule.TraverseSchedule(
                schedule.Start,
                true,
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
            EditableSchedule schedule = null;
            {
                var graph = new BidirectionalGraph<IScheduleVertex, ScheduleEdge>();

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

                graph.AddEdge(new ScheduleEdge(start, vertex1));
                graph.AddEdge(new ScheduleEdge(vertex1, vertex2));

                graph.AddEdge(new ScheduleEdge(vertex2, end));
                graph.AddEdge(new ScheduleEdge(vertex2, vertex3));

                graph.AddEdge(new ScheduleEdge(vertex3, vertex1));

                schedule = new EditableSchedule(graph, start, end);
            }

            var vertices = new List<int>();
            schedule.TraverseSchedule(
                schedule.Start,
                true,
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
            EditableSchedule schedule = null;
            {
                var graph = new BidirectionalGraph<IScheduleVertex, ScheduleEdge>();

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

                var vertex4 = new EditableInsertVertex(6);
                graph.AddVertex(vertex4);

                var vertex5 = new EditableInsertVertex(7);
                graph.AddVertex(vertex5);

                graph.AddEdge(new ScheduleEdge(start, vertex1));
                graph.AddEdge(new ScheduleEdge(vertex1, vertex2));

                graph.AddEdge(new ScheduleEdge(vertex2, end));
                graph.AddEdge(new ScheduleEdge(vertex2, vertex3));

                graph.AddEdge(new ScheduleEdge(vertex3, vertex1));
                graph.AddEdge(new ScheduleEdge(vertex3, vertex4));

                graph.AddEdge(new ScheduleEdge(vertex4, vertex5));
                graph.AddEdge(new ScheduleEdge(vertex5, vertex3));

                schedule = new EditableSchedule(graph, start, end);
            }

            var vertices = new List<int>();
            schedule.TraverseSchedule(
                schedule.Start,
                true,
                (vertex, edges) =>
                {
                    vertices.Add(vertex.Index);
                    return true;
                });

            Assert.AreElementsEqual(new int[] { 1, 3, 4, 2, 5, 6, 7 }, vertices);
        }
    }
}
