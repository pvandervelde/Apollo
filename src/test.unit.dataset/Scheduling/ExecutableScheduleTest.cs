//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Extensions.Scheduling;
using MbUnit.Framework;
using QuickGraph;

namespace Apollo.Core.Dataset.Scheduling
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class ExecutableScheduleTest
    {
        [Test]
        public void Create()
        {
            var graph = new AdjacencyGraph<IExecutableScheduleVertex, ExecutableScheduleEdge>();
            
            var start = new ExecutableStartVertex(1);
            graph.AddVertex(start);

            var end = new ExecutableEndVertex(2);
            graph.AddVertex(end);
            graph.AddEdge(new ExecutableScheduleEdge(start, end, null));

            var id = new ScheduleId();
            var schedule = new ExecutableSchedule(id, graph, start, end);

            Assert.AreSame(id, schedule.Id);
            Assert.AreSame(graph, schedule.Graph);
            Assert.AreSame(start, schedule.Start);
            Assert.AreSame(end, schedule.End);
        }
    }
}
