//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using MbUnit.Framework;

namespace Apollo.Core.Extensions.Scheduling
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class EditableScheduleEdgeTest
    {
        [Test]
        public void Create()
        {
            IEditableScheduleVertex source = new EditableStartVertex(10);
            IEditableScheduleVertex target = new EditableEndVertex(11);

            var condition = new ScheduleElementId();
            var edge = new EditableScheduleEdge(source, target, condition);

            Assert.AreSame(source, edge.Source);
            Assert.AreSame(target, edge.Target);
            Assert.AreSame(condition, edge.TraversingCondition);
        }
    }
}
