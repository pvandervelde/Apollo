//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using Apollo.Core.Base.Communication;
using Apollo.Utilities;
using Apollo.Utilities.Configuration;
using Gallio.Framework;
using MbUnit.Framework;
using Moq;

namespace Apollo.Base.Communication
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class NamedPipeChannelTypeTest
    {
        private static readonly int s_Pattern = int.Parse("ABCD", NumberStyles.HexNumber);

        private static string GetRandomFileName()
        {
            var assemblyPath = typeof(NamedPipeChannelType).Assembly.LocalFilePath();
            return Path.Combine(Path.GetDirectoryName(assemblyPath), Path.GetRandomFileName());
        }

        private static FileInfo CreateRandomFile(long fileSize)
        {
            var fileName = GetRandomFileName();
            using (var writer = new BinaryWriter(new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write)))
            {
                while (writer.BaseStream.Length < fileSize)
                {
                    writer.Write(s_Pattern);
                }
            }

            return new FileInfo(fileName);
        }

        private static bool AreStreamsEqual(Stream stream1, Stream stream2)
        {
            // Take a random buffer size. For now we use 4Kb.
            const int bufferSize = 4096;

            var buffer1 = new byte[bufferSize];
            var buffer2 = new byte[bufferSize];
            while (true)
            {
                int count1 = stream1.Read(buffer1, 0, buffer1.Length);
                int count2 = stream2.Read(buffer2, 0, buffer2.Length);

                if (count1 != count2)
                {
                    // Files are not equal in size
                    return false;
                }

                if (count1 == 0)
                {
                    // Files are equal in size and we got to
                    // the end of the file
                    return true;
                }

                if (!buffer1.Take(count1).SequenceEqual(buffer2.Take(count2)))
                {
                    // The bytes in the buffer are not equal
                    return false;
                }
            }
        }

        [Test]
        public void StreamData()
        {
            var config = new Mock<IConfiguration>();
            {
                config.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false);
            }

            var sender = new NamedPipeChannelType(config.Object);
            var recipient = new NamedPipeChannelType(config.Object);

            var outputPath = GetRandomFileName();
            var file = CreateRandomFile(1 * 1024 * 128);
            long outputProgress = 0;
            Action<IProgressMark, long> outputReporter = (m, p) => outputProgress = p;
            var pair = recipient.PrepareForDataReception(outputPath, outputReporter, new CancellationToken(), null);
            var outputTask = pair.Item2.ContinueWith(
                t =>
                {
                    Assert.IsTrue(t.IsCompleted);
                    Assert.IsFalse(t.IsFaulted);
                    Assert.AreEqual(file.Length, outputProgress);

                    using (var fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                    using (var outputStream = new FileStream(outputPath, FileMode.Open, FileAccess.Read))
                    {
                        Assert.IsTrue(AreStreamsEqual(fileStream, outputStream));
                    }
                });

            long inputProgress = 0;
            Action<IProgressMark, long> inputReporter = (m, p) => inputProgress = p;
            Task sendingTask = sender.TransferData(
                file.FullName, 
                pair.Item1, 
                inputReporter, 
                new CancellationToken(), 
                new CurrentThreadTaskScheduler());

            Assert.AreEqual(file.Length, inputProgress);

            // Check that nothing went wrong on the other task
            outputTask.Wait();
        }

        [Test]
        public void StreamDataWithDisturbanceOnSenderSide()
        {
            var config = new Mock<IConfiguration>();
            {
                config.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false);
            }

            var sender = new NamedPipeChannelType(config.Object);
            var recipient = new NamedPipeChannelType(config.Object);

            long size = 1 * 1024 * 256;
            var file = CreateRandomFile(size);

            var outputPath = GetRandomFileName();
            var pair = recipient.PrepareForDataReception(outputPath, null, new CancellationTokenSource().Token, null);
            var token = new CancellationTokenSource();
            Action<IProgressMark, long> inputReporter =
                (m, p) => 
                {
                    if (p > size / 2)
                    {
                        token.Cancel();

                        DiagnosticLog.WriteLine(
                        string.Format(
                            "Writing to file: {0}. Current size: {1}. Expected size: {2}",
                            outputPath,
                            new FileInfo(outputPath).Length,
                            size));
                    }
                };

            Task sendingTask = sender.TransferData(file.FullName, pair.Item1, inputReporter, token.Token, null);

            // check the status of the streaming processes
            try
            {
                sendingTask.Wait();
            }
            catch (AggregateException)
            {
                // We cancelled the task
            }

            Assert.IsFalse(sendingTask.IsFaulted, "1");
            Assert.IsTrue(sendingTask.IsCompleted, "2");
            Assert.IsTrue(sendingTask.IsCanceled, "3");

            pair.Item2.Wait();
            Assert.IsFalse(pair.Item2.IsCanceled, "4");
            Assert.IsFalse(pair.Item2.IsFaulted, "5");
            Assert.IsTrue(pair.Item2.IsCompleted, "6");

            // Restart the operation.
            pair = recipient.PrepareForDataReception(outputPath, null, new CancellationToken(), null);
            var outputTask = pair.Item2.ContinueWith(
                t =>
                {
                    Assert.IsTrue(t.IsCompleted, "8");
                    Assert.IsFalse(t.IsFaulted, "9");

                    using (var fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                    using (var outputStream = new FileStream(outputPath, FileMode.Open, FileAccess.Read))
                    {
                        Assert.IsTrue(AreStreamsEqual(fileStream, outputStream), "10");
                    }
                });

            sendingTask = sender.TransferData(file.FullName, pair.Item1, null, new CancellationToken(), new CurrentThreadTaskScheduler());

            // Check that nothing went wrong on the other task
            outputTask.Wait();
        }

        [Test]
        public void StreamDataWithDisturbanceOnReceiverSide()
        {
            var config = new Mock<IConfiguration>();
            {
                config.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false);
            }

            var sender = new NamedPipeChannelType(config.Object);
            var recipient = new NamedPipeChannelType(config.Object);

            long size = 1 * 1024 * 256;
            var file = CreateRandomFile(size);

            var outputPath = GetRandomFileName();
            var token = new CancellationTokenSource();
            Action<IProgressMark, long> outputReporter =
                (m, p) =>
                {
                    if (p > size / 2)
                    {
                        token.Cancel();

                        DiagnosticLog.WriteLine(
                           string.Format(
                               "Writing to file: {0}. Current size: {1}. Expected size: {2}",
                               outputPath,
                               new FileInfo(outputPath).Length,
                               size));
                    }
                };
            var pair = recipient.PrepareForDataReception(outputPath, outputReporter, token.Token, null);
            
            Task sendingTask = sender.TransferData(file.FullName, pair.Item1, null, new CancellationTokenSource().Token, null);

            // check the status of the streaming processes
            try
            {
                pair.Item2.Wait();
            }
            catch (AggregateException)
            {
                // We know it has been cancelled.
            }

            Assert.IsTrue(pair.Item2.IsCanceled, "1");
            Assert.IsFalse(pair.Item2.IsFaulted, "2");
            Assert.IsTrue(pair.Item2.IsCompleted, "3");

            try
            {
                sendingTask.Wait();
            }
            catch (AggregateException)
            {
                // Pipe has died because the other side killed it.
            }

            Assert.IsFalse(sendingTask.IsCanceled, "4");
            Assert.IsTrue(sendingTask.IsFaulted, "5");
            Assert.IsTrue(sendingTask.IsCompleted, "6");

            // Restart the operation.
            pair = recipient.PrepareForDataReception(outputPath, null, new CancellationToken(), null);
            var outputTask = pair.Item2.ContinueWith(
                t =>
                {
                    Assert.IsTrue(t.IsCompleted, "7");
                    Assert.IsFalse(t.IsFaulted, "8");

                    using (var fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                    using (var outputStream = new FileStream(outputPath, FileMode.Open, FileAccess.Read))
                    {
                        Assert.IsTrue(AreStreamsEqual(fileStream, outputStream), "10");
                    }
                });

            sendingTask = sender.TransferData(file.FullName, pair.Item1, null, new CancellationToken(), new CurrentThreadTaskScheduler());

            // Check that nothing went wrong on the other task
            outputTask.Wait();
        }
    }
}
