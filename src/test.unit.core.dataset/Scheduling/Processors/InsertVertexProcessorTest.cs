//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base.Scheduling;
using Apollo.Core.Extensions.Scheduling;
using MbUnit.Framework;

namespace Apollo.Core.Dataset.Scheduling.Processors
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class InsertVertexProcessorTest
    {
        [Test]
        public void VertexTypeToProcess()
        {
            var processor = new InsertVertexProcessor();
            Assert.AreEqual(typeof(InsertVertex), processor.VertexTypeToProcess);
        }

        [Test]
        public void ProcessWithIncorrectVertexType()
        {
            var processor = new InsertVertexProcessor();
            var state = processor.Process(new StartVertex(1), new ScheduleExecutionInfo());
            Assert.AreEqual(ScheduleExecutionState.IncorrectProcessorForVertex, state);
        }

        [Test]
        public void ProcessWithCancellation()
        {
            var info = new ScheduleExecutionInfo();
            info.CancelScheduleExecution();

            var processor = new InsertVertexProcessor();
            var state = processor.Process(new InsertVertex(1), info);
            Assert.AreEqual(ScheduleExecutionState.Canceled, state);
        }

        [Test]
        public void Process()
        {
            var processor = new InsertVertexProcessor();
            var state = processor.Process(new InsertVertex(1), new ScheduleExecutionInfo());
            Assert.AreEqual(ScheduleExecutionState.Executing, state);
        }
    }
}
