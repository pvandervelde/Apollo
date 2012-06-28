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
    public sealed class EditableSubScheduleVertexTest
    {
        [Test]
        public void Create()
        {
            var index = 10;
            var subScheduleId = new ScheduleId();
            var vertex = new EditableSubScheduleVertex(index, subScheduleId);

            Assert.AreEqual(index, vertex.Index);
            Assert.AreSame(subScheduleId, vertex.ScheduleToExecute);
        }
    }
}
