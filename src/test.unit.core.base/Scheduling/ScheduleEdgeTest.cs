//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Extensions.Scheduling;
using MbUnit.Framework;

namespace Apollo.Core.Base.Scheduling
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class ScheduleEdgeTest
    {
        [Test]
        public void Create()
        {
            IScheduleVertex source = new StartVertex(10);
            IScheduleVertex target = new EndVertex(11);

            var condition = new ScheduleElementId();
            var edge = new ScheduleEdge(source, target, condition);

            Assert.AreSame(source, edge.Source);
            Assert.AreSame(target, edge.Target);
            Assert.AreSame(condition, edge.TraversingCondition);
        }
    }
}
