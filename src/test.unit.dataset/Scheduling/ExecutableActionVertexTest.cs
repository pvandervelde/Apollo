﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Extensions.Scheduling;
using MbUnit.Framework;

namespace Apollo.Core.Dataset.Scheduling
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class ExecutableActionVertexTest
    {
        [Test]
        public void Create()
        {
            var index = 10;
            var actionId = new ScheduleElementId();
            var vertex = new ExecutableActionVertex(index, actionId);

            Assert.AreEqual(index, vertex.Index);
            Assert.AreSame(actionId, vertex.ActionToExecute);
        }
    }
}
