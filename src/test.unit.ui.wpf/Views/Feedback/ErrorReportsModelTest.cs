//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using System.Reflection;
using System.Threading;
using System.Windows.Input;
using Apollo.UI.Wpf.Feedback;
using Moq;
using Nuclei;
using NUnit.Framework;
using Test.Mocks;

namespace Apollo.UI.Wpf.Views.Feedback
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class ErrorReportsModelTest
    {
        [Test]
        public void Create()
        {
            var context = new Mock<IContextAware>();
            {
                context.Setup(c => c.IsSynchronized)
                    .Returns(true);
            }

            var command = new Mock<ICommand>();
            var file = new FileInfo(Assembly.GetExecutingAssembly().LocalFilePath());
            var collector = new Mock<ICollectFeedbackReports>();
            {
                collector.Setup(c => c.LocateFeedbackReports())
                    .Returns(
                        new List<FileInfo>
                            {
                                file
                            });
            }

            var fileSystem = new Mock<IFileSystem>();

            var model = new ErrorReportsModel(context.Object, command.Object, collector.Object, fileSystem.Object);
            
            var wasRaised = false;
            model.PropertyChanged += (s, e) =>
            {
                wasRaised = true;
            };

            while (!wasRaised && !model.HasErrorReports)
            {
                Thread.Yield();
            }

            Assert.IsTrue(model.HasErrorReports);
            Assert.That(
                model.Reports,
                Is.EquivalentTo(
                    new List<FeedbackFileModel>
                    {
                        new FeedbackFileModel(context.Object, file.FullName, file.CreationTime)
                    }));
        }

        [Test]
        public void SendReports()
        {
            var context = new Mock<IContextAware>();
            {
                context.Setup(c => c.IsSynchronized)
                    .Returns(true);
            }

            var command = new Mock<ICommand>();
            {
                command.Setup(c => c.Execute(It.IsAny<object>()))
                    .Verifiable();
            }

            var file = new FileInfo(Assembly.GetExecutingAssembly().LocalFilePath());

            var counter = 0;
            var collector = new Mock<ICollectFeedbackReports>();
            {
                collector.Setup(c => c.LocateFeedbackReports())
                    .Returns(
                        () =>
                        {
                            counter++;
                            var result = new List<FileInfo>();
                            if (counter <= 1)
                            {
                                result.Add(file);
                            }

                            return result;
                        });
            }

            var mockFile = new MockFile(file.FullName, "a");
            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.File)
                    .Returns(mockFile);
            }

            var model = new ErrorReportsModel(context.Object, command.Object, collector.Object, fileSystem.Object);
            while (!model.HasErrorReports)
            {
                Thread.Yield();
            }

            var wasRaised = false;
            model.PropertyChanged += (s, e) =>
            {
                wasRaised = true;
            };

            model.SendReports(
                new[]
                    {
                        new FeedbackFileModel(context.Object, file.FullName, file.CreationTime), 
                    });

            command.Verify(c => c.Execute(It.IsAny<object>()), Times.Once());

            while (!wasRaised)
            {
                Thread.Yield();
            }

            // For some reason in release mode this doesn't quite work, except if we slow things down a bit
            // so ....
            Thread.Sleep(50);
            Assert.IsFalse(model.HasErrorReports);
        }
        
        [Test]
        public void SendReportsWithAccessException()
        {
            var context = new Mock<IContextAware>();
            {
                context.Setup(c => c.IsSynchronized)
                    .Returns(true);
            }

            var command = new Mock<ICommand>();
            {
                command.Setup(c => c.Execute(It.IsAny<object>()))
                    .Throws<UnauthorizedAccessException>();
            }

            var file = new FileInfo(Assembly.GetExecutingAssembly().LocalFilePath());

            var collector = new Mock<ICollectFeedbackReports>();
            {
                collector.Setup(c => c.LocateFeedbackReports())
                    .Returns(
                        () =>
                        {
                            var result = new List<FileInfo>();
                            result.Add(file);
                            return result;
                        });
            }

            var mockFile = new MockFile(file.FullName, "a");
            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.File)
                    .Returns(mockFile);
            }

            var model = new ErrorReportsModel(context.Object, command.Object, collector.Object, fileSystem.Object);
            while (!model.HasErrorReports)
            {
                Thread.Yield();
            }

            var wasRaised = false;
            model.PropertyChanged += (s, e) =>
            {
                wasRaised = true;
            };

            model.SendReports(
                new[]
                    {
                        new FeedbackFileModel(context.Object, file.FullName, file.CreationTime), 
                    });

            while (!wasRaised)
            {
                Thread.Yield();
            }

            // For some reason in release mode this doesn't quite work, except if we slow things down a bit
            // so ....
            Thread.Sleep(50);
            Assert.IsTrue(model.HasErrorReports);
        }

        [Test]
        public void SendReportsWithIOException()
        {
            var context = new Mock<IContextAware>();
            {
                context.Setup(c => c.IsSynchronized)
                    .Returns(true);
            }

            var command = new Mock<ICommand>();
            {
                command.Setup(c => c.Execute(It.IsAny<object>()))
                    .Throws<IOException>();
            }

            var file = new FileInfo(Assembly.GetExecutingAssembly().LocalFilePath());

            var collector = new Mock<ICollectFeedbackReports>();
            {
                collector.Setup(c => c.LocateFeedbackReports())
                    .Returns(
                        () =>
                        {
                            var result = new List<FileInfo>();
                            result.Add(file);
                            return result;
                        });
            }

            var mockFile = new MockFile(file.FullName, "a");
            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.File)
                    .Returns(mockFile);
            }

            var model = new ErrorReportsModel(context.Object, command.Object, collector.Object, fileSystem.Object);
            while (!model.HasErrorReports)
            {
                Thread.Yield();
            }

            var wasRaised = false;
            model.PropertyChanged += (s, e) =>
            {
                wasRaised = true;
            };

            model.SendReports(
                new[]
                    {
                        new FeedbackFileModel(context.Object, file.FullName, file.CreationTime), 
                    });

            while (!wasRaised)
            {
                Thread.Yield();
            }

            // For some reason in release mode this doesn't quite work, except if we slow things down a bit
            // so ....
            Thread.Sleep(50);
            Assert.IsTrue(model.HasErrorReports);
        }

        [Test]
        public void SendReportsWithFeedbackSendException()
        {
            var context = new Mock<IContextAware>();
            {
                context.Setup(c => c.IsSynchronized)
                    .Returns(true);
            }

            var command = new Mock<ICommand>();
            {
                command.Setup(c => c.Execute(It.IsAny<object>()))
                    .Throws<FailedToSendFeedbackReportException>();
            }

            var file = new FileInfo(Assembly.GetExecutingAssembly().LocalFilePath());

            var collector = new Mock<ICollectFeedbackReports>();
            {
                collector.Setup(c => c.LocateFeedbackReports())
                    .Returns(
                        () =>
                        {
                            var result = new List<FileInfo>();
                            result.Add(file);
                            return result;
                        });
            }

            var mockFile = new MockFile(file.FullName, "a");
            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.File)
                    .Returns(mockFile);
            }

            var model = new ErrorReportsModel(context.Object, command.Object, collector.Object, fileSystem.Object);
            while (!model.HasErrorReports)
            {
                Thread.Yield();
            }

            var wasRaised = false;
            model.PropertyChanged += (s, e) =>
            {
                wasRaised = true;
            };

            model.SendReports(
                new[]
                    {
                        new FeedbackFileModel(context.Object, file.FullName, file.CreationTime), 
                    });

            while (!wasRaised)
            {
                Thread.Yield();
            }

            // For some reason in release mode this doesn't quite work, except if we slow things down a bit
            // so ....
            Thread.Sleep(50);
            Assert.IsTrue(model.HasErrorReports);
        }
    }
}
