//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Apollo.Core.Extensions.Scheduling;
using Moq;
using NUnit.Framework;

namespace Apollo.Core.Base.Scheduling
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class FixedScheduleBuilderTest
    {
        private static bool AreVerticesEqual(IScheduleVertex first, IScheduleVertex second)
        {
            return first.Equals(second);
        }

        [Test]
        public void CreateWithTemplateSchedule()
        {
            var conditionId1 = new ScheduleElementId();
            var conditionId2 = new ScheduleElementId();

            var templateBuilder = new FixedScheduleBuilder();
            var insertVertex = templateBuilder.AddInsertPoint(1);
            templateBuilder.LinkFromStart(insertVertex, conditionId1);
            templateBuilder.LinkToEnd(insertVertex, conditionId2);

            var templateSchedule = templateBuilder.Build();

            var builder = new FixedScheduleBuilder(templateSchedule);
            var markHistoryVertex = new MarkHistoryVertex(10);
            builder.InsertIn(insertVertex, markHistoryVertex);
            var schedule = builder.Build();

            int index = 0;
            var vertexTypes = new List<Type> 
                { 
                    typeof(StartVertex), 
                    typeof(MarkHistoryVertex), 
                    typeof(EndVertex) 
                };
            var conditions = new List<ScheduleElementId> 
                { 
                    conditionId1, 
                    conditionId2 
                };

            schedule.TraverseAllScheduleVertices(
                schedule.Start,
                (vertex, edges) =>
                {
                    Assert.AreEqual(vertexTypes[index], vertex.GetType());
                    if (index < conditions.Count)
                    {
                        Assert.AreEqual(1, edges.Count());
                        Assert.AreEqual(conditions[index], edges.First().Item1);
                    }

                    index++;
                    return true;
                });
        }

        [Test]
        public void AddExecutingAction()
        {
            var actionId = new ScheduleElementId();
            
            var builder = new FixedScheduleBuilder();
            var actionVertex = builder.AddExecutingAction(actionId);
            builder.LinkFromStart(actionVertex);
            builder.LinkToEnd(actionVertex);
            Assert.AreEqual(actionId, actionVertex.ActionToExecute);
            Assert.AreEqual(2, actionVertex.Index);

            var schedule = builder.Build();
            Assert.AreEqual(3, schedule.Vertices.Count());
            Assert.IsTrue(AreVerticesEqual(schedule.Start, schedule.Vertices.ElementAt(0)));
            Assert.IsTrue(AreVerticesEqual(actionVertex, schedule.Vertices.ElementAt(1)));
            Assert.IsTrue(AreVerticesEqual(schedule.End, schedule.Vertices.ElementAt(2)));
        }

        [Test]
        public void AddSubSchedule()
        {
            var scheduleId = new ScheduleId();
            var builder = new FixedScheduleBuilder();
            var subScheduleVertex = builder.AddSubSchedule(scheduleId);
            builder.LinkFromStart(subScheduleVertex);
            builder.LinkToEnd(subScheduleVertex);
            Assert.AreEqual(scheduleId, subScheduleVertex.ScheduleToExecute);
            Assert.AreEqual(2, subScheduleVertex.Index);

            var schedule = builder.Build();
            Assert.AreEqual(3, schedule.Vertices.Count());
            Assert.IsTrue(AreVerticesEqual(schedule.Start, schedule.Vertices.ElementAt(0)));
            Assert.IsTrue(AreVerticesEqual(subScheduleVertex, schedule.Vertices.ElementAt(1)));
            Assert.IsTrue(AreVerticesEqual(schedule.End, schedule.Vertices.ElementAt(2)));
        }

        [Test]
        public void AddSynchronizationStartPointWithoutVariables()
        {
            var builder = new FixedScheduleBuilder();
            Assert.Throws<CannotCreateASynchronizationBlockWithoutVariablesException>(
                () => builder.AddSynchronizationStart(new IScheduleVariable[] { }));
        }

        [Test]
        public void AddSynchronizationStartPoint()
        {
            var variable = new Mock<IScheduleVariable>();

            var builder = new FixedScheduleBuilder();
            var synchronizationVertex = builder.AddSynchronizationStart(new[] { variable.Object });
            builder.LinkFromStart(synchronizationVertex);
            builder.LinkToEnd(synchronizationVertex);
            Assert.That(synchronizationVertex.VariablesToSynchronizeOn, Is.EquivalentTo(new[] { variable.Object }));
            Assert.AreEqual(2, synchronizationVertex.Index);

            var schedule = builder.Build();
            Assert.AreEqual(3, schedule.Vertices.Count());
            Assert.IsTrue(AreVerticesEqual(schedule.Start, schedule.Vertices.ElementAt(0)));
            Assert.IsTrue(AreVerticesEqual(synchronizationVertex, schedule.Vertices.ElementAt(1)));
            Assert.IsTrue(AreVerticesEqual(schedule.End, schedule.Vertices.ElementAt(2)));
        }

        [Test]
        public void AddSynchronizationEndPointWithMissingStartPoint()
        {
            var variable = new Mock<IScheduleVariable>();

            var builder = new FixedScheduleBuilder();
            Assert.Throws<UnknownScheduleVertexException>(
                () => builder.AddSynchronizationEnd(
                    new SynchronizationStartVertex(
                        10, 
                        new[] 
                            { 
                                variable.Object 
                            })));
        }

        [Test]
        public void AddSynchronizationEndPoint()
        {
            var variable = new Mock<IScheduleVariable>();

            var builder = new FixedScheduleBuilder();
            var synchronizationStartVertex = builder.AddSynchronizationStart(new[] { variable.Object });
            builder.LinkFromStart(synchronizationStartVertex);
            
            var synchronizationEndVertex = builder.AddSynchronizationEnd(synchronizationStartVertex);
            builder.LinkTo(synchronizationStartVertex, synchronizationEndVertex);
            builder.LinkToEnd(synchronizationEndVertex);
            Assert.AreEqual(3, synchronizationEndVertex.Index);

            var schedule = builder.Build();
            Assert.AreEqual(3, schedule.Vertices.Count());
            Assert.IsTrue(AreVerticesEqual(schedule.Start, schedule.Vertices.ElementAt(0)));
            Assert.IsTrue(AreVerticesEqual(synchronizationStartVertex, schedule.Vertices.ElementAt(1)));
            Assert.IsTrue(AreVerticesEqual(synchronizationEndVertex, schedule.Vertices.ElementAt(1)));
            Assert.IsTrue(AreVerticesEqual(schedule.End, schedule.Vertices.ElementAt(3)));
        }

        [Test]
        public void AddHistoryMarkingPoint()
        {
            var builder = new FixedScheduleBuilder();
            var historyMarkingVertex = builder.AddHistoryMarkingPoint();
            builder.LinkFromStart(historyMarkingVertex);
            builder.LinkToEnd(historyMarkingVertex);
            Assert.AreEqual(2, historyMarkingVertex.Index);

            var schedule = builder.Build();
            Assert.AreEqual(3, schedule.Vertices.Count());
            Assert.IsTrue(AreVerticesEqual(schedule.Start, schedule.Vertices.ElementAt(0)));
            Assert.IsTrue(AreVerticesEqual(historyMarkingVertex, schedule.Vertices.ElementAt(1)));
            Assert.IsTrue(AreVerticesEqual(schedule.End, schedule.Vertices.ElementAt(2)));
        }

        [Test]
        public void AddInsertPoint()
        {
            var builder = new FixedScheduleBuilder();
            var insertVertex = builder.AddInsertPoint();
            builder.LinkFromStart(insertVertex);
            builder.LinkToEnd(insertVertex);
            Assert.AreEqual(2, insertVertex.Index);

            var schedule = builder.Build();
            Assert.AreEqual(3, schedule.Vertices.Count());
            Assert.IsTrue(AreVerticesEqual(schedule.Start, schedule.Vertices.ElementAt(0)));
            Assert.IsTrue(AreVerticesEqual(insertVertex, schedule.Vertices.ElementAt(1)));
            Assert.IsTrue(AreVerticesEqual(schedule.End, schedule.Vertices.ElementAt(2)));
        }

        [Test]
        public void AddInsertPointWithInvalidCount()
        {
            var builder = new FixedScheduleBuilder();
            Assert.Throws<ArgumentOutOfRangeException>(() => builder.AddInsertPoint(0));
        }

        [Test]
        public void AddInsertPointWithCount()
        {
            var count = 10;
            var builder = new FixedScheduleBuilder();
            var insertVertex = builder.AddInsertPoint(count);
            builder.LinkFromStart(insertVertex);
            builder.LinkToEnd(insertVertex);
            Assert.AreEqual(2, insertVertex.Index);
            Assert.AreEqual(count, insertVertex.RemainingInserts);

            var schedule = builder.Build();
            Assert.AreEqual(3, schedule.Vertices.Count());
            Assert.IsTrue(AreVerticesEqual(schedule.Start, schedule.Vertices.ElementAt(0)));
            Assert.IsTrue(AreVerticesEqual(insertVertex, schedule.Vertices.ElementAt(1)));
            Assert.IsTrue(AreVerticesEqual(schedule.End, schedule.Vertices.ElementAt(2)));
        }

        [Test]
        public void InsertInWithUnknownInsertVertex()
        {
            var builder = new FixedScheduleBuilder();
            var markHistoryVertex = builder.AddHistoryMarkingPoint();
            Assert.Throws<UnknownScheduleVertexException>(
                () => builder.InsertIn(new InsertVertex(10), markHistoryVertex));
        }

        [Test]
        public void InsertInWithKnownVertexToInsert()
        {
            var builder = new FixedScheduleBuilder();
            var insertVertex = builder.AddInsertPoint();
            var markHistoryVertex = builder.AddHistoryMarkingPoint();
            Assert.Throws<CannotInsertExistingVertexException>(
                () => builder.InsertIn(insertVertex, markHistoryVertex));
        }

        [Test]
        public void InsertInWithNoMoreRemainingInserts()
        {
            var builder = new FixedScheduleBuilder();
            var insertVertex = builder.AddInsertPoint(1);
            builder.LinkFromStart(insertVertex);
            builder.LinkToEnd(insertVertex);

            var markHistoryVertex1 = new MarkHistoryVertex(10);
            var newInserts = builder.InsertIn(insertVertex, markHistoryVertex1);
            Assert.IsNull(newInserts.Item1);
            Assert.IsNull(newInserts.Item2);
        }

        [Test]
        public void InsertIn()
        {
            var conditionId1 = new ScheduleElementId();
            var conditionId2 = new ScheduleElementId();

            var builder = new FixedScheduleBuilder();
            var insertVertex = builder.AddInsertPoint();
            builder.LinkFromStart(insertVertex, conditionId1);
            builder.LinkToEnd(insertVertex, conditionId2);

            var markHistoryVertex = new MarkHistoryVertex(10);
            builder.InsertIn(insertVertex, markHistoryVertex);
            var schedule = builder.Build();

            int index = 0;
            var vertexTypes = new List<Type> 
                { 
                    typeof(StartVertex), 
                    typeof(InsertVertex), 
                    typeof(MarkHistoryVertex), 
                    typeof(InsertVertex), 
                    typeof(EndVertex) 
                };
            var conditions = new List<ScheduleElementId> 
                { 
                    conditionId1, 
                    null, 
                    null, 
                    conditionId2 
                };

            schedule.TraverseAllScheduleVertices(
                schedule.Start,
                (vertex, edges) =>
                {
                    Assert.AreEqual(vertexTypes[index], vertex.GetType());
                    if (index < conditions.Count)
                    {
                        Assert.AreEqual(1, edges.Count());
                        Assert.AreEqual(conditions[index], edges.First().Item1);
                    }

                    index++;
                    return true;
                });
        }

        [Test]
        public void InsertInWithOneInsertAllowed()
        {
            var conditionId1 = new ScheduleElementId();
            var conditionId2 = new ScheduleElementId();

            var builder = new FixedScheduleBuilder();
            var insertVertex = builder.AddInsertPoint(1);
            builder.LinkFromStart(insertVertex, conditionId1);
            builder.LinkToEnd(insertVertex, conditionId2);

            var markHistoryVertex = new MarkHistoryVertex(10);
            builder.InsertIn(insertVertex, markHistoryVertex);
            var schedule = builder.Build();

            int index = 0;
            var vertexTypes = new List<Type> 
                { 
                    typeof(StartVertex), 
                    typeof(MarkHistoryVertex), 
                    typeof(EndVertex) 
                };
            var conditions = new List<ScheduleElementId> 
                { 
                    conditionId1, 
                    conditionId2 
                };

            schedule.TraverseAllScheduleVertices(
                schedule.Start,
                (vertex, edges) =>
                {
                    Assert.AreEqual(vertexTypes[index], vertex.GetType());
                    if (index < conditions.Count)
                    {
                        Assert.AreEqual(1, edges.Count());
                        Assert.AreEqual(conditions[index], edges.First().Item1);
                    }

                    index++;
                    return true;
                });
        }

        [Test]
        public void InsertInWithSubSchedule()
        {
            var conditionId1 = new ScheduleElementId();
            var conditionId2 = new ScheduleElementId();

            var builder = new FixedScheduleBuilder();
            var insertVertex = builder.AddInsertPoint();
            builder.LinkFromStart(insertVertex, conditionId1);
            builder.LinkToEnd(insertVertex, conditionId2);

            var scheduleId = new ScheduleId();
            builder.InsertIn(insertVertex, scheduleId);
            var schedule = builder.Build();

            int index = 0;
            var vertexTypes = new List<Type> 
                { 
                    typeof(StartVertex), 
                    typeof(InsertVertex), 
                    typeof(SubScheduleVertex), 
                    typeof(InsertVertex), 
                    typeof(EndVertex) 
                };
            var conditions = new List<ScheduleElementId> 
                { 
                    conditionId1, 
                    null, 
                    null, 
                    conditionId2 
                };

            schedule.TraverseAllScheduleVertices(
                schedule.Start,
                (vertex, edges) =>
                {
                    Assert.AreEqual(vertexTypes[index], vertex.GetType());
                    if (index < conditions.Count)
                    {
                        Assert.AreEqual(1, edges.Count());
                        Assert.AreEqual(conditions[index], edges.First().Item1);
                    }

                    index++;
                    return true;
                });
        }

        [Test]
        public void LinkToWithUnknownStart()
        {
            var builder = new FixedScheduleBuilder();
            var insertVertex = builder.AddInsertPoint();
            var otherVertex = new InsertVertex(10);
            Assert.Throws<UnknownScheduleVertexException>(() => builder.LinkTo(otherVertex, insertVertex));
        }

        [Test]
        public void LinkToWithUnknownEnd()
        {
            var builder = new FixedScheduleBuilder();
            var insertVertex = builder.AddInsertPoint();
            var otherVertex = new InsertVertex(10);
            Assert.Throws<UnknownScheduleVertexException>(() => builder.LinkTo(insertVertex, otherVertex));
        }

        [Test]
        public void LinkToWithStartAndEndEqual()
        {
            var builder = new FixedScheduleBuilder();
            var insertVertex = builder.AddInsertPoint();
            var otherVertex = new InsertVertex(10);
            Assert.Throws<CannotLinkAVertexToItselfException>(() => builder.LinkTo(insertVertex, insertVertex));
        }

        [Test]
        public void LinkFromStartWithUnknownVertex()
        {
            var builder = new FixedScheduleBuilder();
            var otherVertex = new InsertVertex(10);
            Assert.Throws<UnknownScheduleVertexException>(() => builder.LinkFromStart(otherVertex));
        }

        [Test]
        public void LinkToEndWithUnknownVertex()
        {
            var builder = new FixedScheduleBuilder();
            var otherVertex = new InsertVertex(10);
            Assert.Throws<UnknownScheduleVertexException>(() => builder.LinkToEnd(otherVertex));
        }
    }
}
