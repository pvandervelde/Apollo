﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base.Scheduling;
using Apollo.Core.Extensions.Scheduling;
using NUnit.Framework;

namespace Apollo.Core.Dataset.Scheduling.Processors
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class StartVertexProcessorTest
    {
        [Test]
        public void VertexTypeToProcess()
        {
            var processor = new StartVertexProcessor();
            Assert.AreEqual(typeof(StartVertex), processor.VertexTypeToProcess);
        }

        [Test]
        public void ProcessWithIncorrectVertexType()
        {
            var processor = new StartVertexProcessor();
            using (var info = new ScheduleExecutionInfo())
            {
                var state = processor.Process(new EndVertex(1), info);
                Assert.AreEqual(ScheduleExecutionState.IncorrectProcessorForVertex, state);
            }
        }

        [Test]
        public void ProcessWithCancellation()
        {
            using (var info = new ScheduleExecutionInfo())
            {
                info.CancelScheduleExecution();

                var processor = new StartVertexProcessor();
                var state = processor.Process(new StartVertex(1), info);
                Assert.AreEqual(ScheduleExecutionState.Canceled, state);
            }
        }

        [Test]
        public void Process()
        {
            var processor = new StartVertexProcessor();
            using (var info = new ScheduleExecutionInfo())
            {
                var state = processor.Process(new StartVertex(1), info);
                Assert.AreEqual(ScheduleExecutionState.Executing, state);
            }
        }
    }
}
