//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows.Input;
using Apollo.UI.Wpf.Feedback;
using Moq;
using NSarrac.Framework;
using NUnit.Framework;

namespace Apollo.UI.Wpf.Views.Feedback
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class FeedbackModelTest
    {
        [Test]
        public void CanSendReport()
        {
            var context = new Mock<IContextAware>();
            var command = new Mock<ICommand>();
            Func<IBuildReports> builder = () => new Mock<IBuildReports>().Object;

            var model = new FeedbackModel(context.Object, command.Object, builder);
            Assert.IsFalse(model.CanSendReport);

            model.Level = FeedbackLevel.Bad;
            Assert.IsTrue(model.CanSendReport);

            model.Level = FeedbackLevel.None;
            Assert.IsFalse(model.CanSendReport);

            model.Level = FeedbackLevel.Neutral;
            Assert.IsTrue(model.CanSendReport);

            model.Level = FeedbackLevel.None;
            Assert.IsFalse(model.CanSendReport);

            model.Level = FeedbackLevel.Good;
            Assert.IsTrue(model.CanSendReport);
        }

        [Test]
        public void PropertyChangedForLevel()
        {
            var context = new Mock<IContextAware>();
            {
                context.Setup(c => c.IsSynchronized)
                    .Returns(true);
            }

            var command = new Mock<ICommand>();
            Func<IBuildReports> builder = () => new Mock<IBuildReports>().Object;

            var model = new FeedbackModel(context.Object, command.Object, builder);

            var propertyChangedWasRaised = 0;
            var properties = new List<string>();
            model.PropertyChanged += (s, e) =>
            {
                propertyChangedWasRaised++;
                properties.Add(e.PropertyName);
            };

            model.Level = FeedbackLevel.Neutral;
            Assert.AreEqual(FeedbackLevel.Neutral, model.Level);
            Assert.AreEqual(2, propertyChangedWasRaised);
            Assert.That(
                properties,
                Is.EquivalentTo(
                    new List<string>
                    {
                        "Level",
                        "CanSendReport"
                    }));
        }

        [Test]
        public void PropertyChangedForDescription()
        {
            var context = new Mock<IContextAware>();
            {
                context.Setup(c => c.IsSynchronized)
                    .Returns(true);
            }

            var command = new Mock<ICommand>();
            Func<IBuildReports> builder = () => new Mock<IBuildReports>().Object;

            var model = new FeedbackModel(context.Object, command.Object, builder);

            var propertyChangedWasRaised = 0;
            var properties = new List<string>();
            model.PropertyChanged += (s, e) =>
            {
                propertyChangedWasRaised++;
                properties.Add(e.PropertyName);
            };

            var text = "a";
            model.Description = text;
            Assert.AreEqual(text, model.Description);
            Assert.AreEqual(1, propertyChangedWasRaised);
            Assert.That(
                properties,
                Is.EquivalentTo(
                    new List<string>
                    {
                        "Description",
                    }));
        }

        [Test]
        public void SendReport()
        {
            var context = new Mock<IContextAware>();
            var command = new Mock<ICommand>();
            {
                command.Setup(c => c.Execute(It.IsAny<object>()))
                    .Verifiable();
            }

            using (var stream = new MemoryStream())
            {
                var level = FeedbackLevel.Good;
                var text = "a";
                var builder = new Mock<IBuildReports>();
                {
                    builder.Setup(b => b.AtTime(It.IsAny<DateTimeOffset>(), It.IsAny<TimeZoneInfo>()))
                        .Returns(builder.Object);
                    builder.Setup(b => b.InApplication(It.IsAny<ApplicationData>()))
                        .Returns(builder.Object);
                    builder.Setup(b => b.OnMachine(It.IsAny<MachineData>()))
                        .Returns(builder.Object);
                    builder.Setup(b => b.OnOperatingSystem(It.IsAny<OperatingSystemData>()))
                        .Callback<OperatingSystemData>(
                            o =>
                            {
                                Assert.AreEqual(Environment.OSVersion.Platform.ToString(), o.Name);
                                Assert.AreEqual(Environment.OSVersion.Version, o.PlatformVersion);
                            })
                        .Returns(builder.Object);
                    builder.Setup(b => b.WithFeedback(It.IsAny<FeedbackData>()))
                        .Callback<FeedbackData>(
                            f =>
                            {
                                Assert.AreEqual(level, f.Level);
                                Assert.AreEqual(text, f.Description);
                            })
                        .Returns(builder.Object);
                    builder.Setup(b => b.ToReport())
                        .Returns(stream);
                }

                Func<IBuildReports> builderFunc = () => builder.Object;

                var model = new FeedbackModel(context.Object, command.Object, builderFunc);
                model.Level = level;
                model.Description = text;
                model.SendReport();

                command.Verify(c => c.Execute(It.IsAny<object>()), Times.Once());

                Assert.AreEqual(FeedbackLevel.None, model.Level);
                Assert.AreEqual(string.Empty, model.Description);
                Assert.IsFalse(model.CanSendReport);
            }
        }

        [Test]
        public void SendReportWithException()
        {
            var context = new Mock<IContextAware>();
            var command = new Mock<ICommand>();
            {
                command.Setup(c => c.Execute(It.IsAny<object>()))
                    .Throws<FailedToSendFeedbackReportException>()
                    .Verifiable();
            }

            using (var stream = new MemoryStream())
            {
                var level = FeedbackLevel.Good;
                var text = "a";
                var builder = new Mock<IBuildReports>();
                {
                    builder.Setup(b => b.AtTime(It.IsAny<DateTimeOffset>(), It.IsAny<TimeZoneInfo>()))
                        .Returns(builder.Object);
                    builder.Setup(b => b.InApplication(It.IsAny<ApplicationData>()))
                        .Returns(builder.Object);
                    builder.Setup(b => b.OnMachine(It.IsAny<MachineData>()))
                        .Returns(builder.Object);
                    builder.Setup(b => b.OnOperatingSystem(It.IsAny<OperatingSystemData>()))
                        .Callback<OperatingSystemData>(
                            o =>
                            {
                                Assert.AreEqual(Environment.OSVersion.Platform.ToString(), o.Name);
                                Assert.AreEqual(Environment.OSVersion.Version, o.PlatformVersion);
                            })
                        .Returns(builder.Object);
                    builder.Setup(b => b.WithFeedback(It.IsAny<FeedbackData>()))
                        .Callback<FeedbackData>(
                            f =>
                            {
                                Assert.AreEqual(level, f.Level);
                                Assert.AreEqual(text, f.Description);
                            })
                        .Returns(builder.Object);
                    builder.Setup(b => b.ToReport())
                        .Returns(stream);
                }

                Func<IBuildReports> builderFunc = () => builder.Object;

                var model = new FeedbackModel(context.Object, command.Object, builderFunc);
                model.Level = level;
                model.Description = text;
                model.SendReport();

                command.Verify(c => c.Execute(It.IsAny<object>()), Times.Once());

                Assert.AreEqual(FeedbackLevel.None, model.Level);
                Assert.AreEqual(string.Empty, model.Description);
                Assert.IsFalse(model.CanSendReport);
            }
        }
    }
}
