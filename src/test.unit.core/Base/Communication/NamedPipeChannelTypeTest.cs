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
using Apollo.Core.Base.Communication;
using Apollo.Utilities.Configuration;
using MbUnit.Framework;
using Moq;

namespace Apollo.Base.Communication
{
    [TestFixture]
    [Description("Tests the NamedPipeChannelType class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class NamedPipeChannelTypeTest
    {
        private static readonly int s_Pattern = int.Parse("ABCD", NumberStyles.HexNumber);

        private static FileInfo CreateRandomFile(long fileSize)
        {
            var fileName = Path.GetTempFileName();
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
            const int BufferSize = 4096;

            var buffer1 = new byte[BufferSize];
            var buffer2 = new byte[BufferSize];
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
        [Description("Checks that data can be streamed across a named pipe.")]
        public void StreamData()
        {
            var config = new Mock<IConfiguration>();
            {
                config.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false);
            }

            var sender = new NamedPipeChannelType(config.Object);
            var recipient = new NamedPipeChannelType(config.Object);

            var outputPath = Path.GetTempFileName();
            var pair = recipient.PrepareForDataReception(outputPath, new CancellationToken());

            Task sendingTask = null;
            var file = CreateRandomFile(1 * 1024 * 1024);
            sendingTask = sender.TransferData(file.FullName, pair.Item1, new CancellationToken());
            sendingTask.ContinueWith(
                t =>
                {
                    Assert.IsTrue(t.IsCompleted);
                    Assert.IsFalse(t.IsFaulted);

                    using (var fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                    {
                        Assert.IsTrue(AreStreamsEqual(fileStream, new FileStream(outputPath, FileMode.Open, FileAccess.Read)));
                    }
                })
                .Wait();

            // Check that nothing went wrong on the other task
            pair.Item2.Wait();
        }

        [Test]
        [Description("Checks that data can be streamed across a named pipe even if the transfer is interrupted from the senders side.")]
        public void StreamDataWithDisturbanceOnSenderSide()
        {
            var config = new Mock<IConfiguration>();
            {
                config.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false);
            }

            var sender = new NamedPipeChannelType(config.Object);
            var recipient = new NamedPipeChannelType(config.Object);

            var outputPath = Path.GetTempFileName();
            var pair = recipient.PrepareForDataReception(outputPath, new CancellationTokenSource().Token);

            long size = 1 * 1024 * 1024;
            var file = CreateRandomFile(size);
            var token = new CancellationTokenSource();
            Task sendingTask = sender.TransferData(file.FullName, pair.Item1, token.Token);

            // Wait till we have written a decent amount of the file, then
            // kill the transfer.
            while (new FileInfo(outputPath).Length < size / 2)
            {
                Thread.Sleep(1);
            }

            // Kill the transfer
            token.Cancel();

            // check the status of the streaming processes
            try
            {
                sendingTask.Wait();
            }
            catch (AggregateException)
            {
                // We cancelled the task
            }

            Assert.IsFalse(sendingTask.IsFaulted);
            Assert.IsTrue(sendingTask.IsCompleted);
            Assert.IsTrue(sendingTask.IsCanceled);

            pair.Item2.Wait();
            Assert.IsFalse(pair.Item2.IsCanceled);
            Assert.IsFalse(pair.Item2.IsFaulted);
            Assert.IsTrue(pair.Item2.IsCompleted);

            // Restart the operation.
            pair = recipient.PrepareForDataReception(outputPath, new CancellationToken());
            sendingTask = sender.TransferData(file.FullName, pair.Item1, new CancellationToken());
            sendingTask.ContinueWith(
                t =>
                {
                    Assert.IsTrue(t.IsCompleted);
                    Assert.IsFalse(t.IsFaulted);

                    using (var fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                    {
                        Assert.IsTrue(AreStreamsEqual(fileStream, new FileStream(outputPath, FileMode.Open, FileAccess.Read)));
                    }
                })
                .Wait();

            // Check that nothing went wrong on the other task
            pair.Item2.Wait();
        }

        [Test]
        [Description("Checks that data can be streamed across a named pipe even if the transfer is interrupted from the receivers side.")]
        public void StreamDataWithDisturbanceOnReceiverSide()
        {
            var config = new Mock<IConfiguration>();
            {
                config.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false);
            }

            var sender = new NamedPipeChannelType(config.Object);
            var recipient = new NamedPipeChannelType(config.Object);

            var outputPath = Path.GetTempFileName();
            var token = new CancellationTokenSource();
            var pair = recipient.PrepareForDataReception(outputPath, token.Token);

            long size = 1 * 1024 * 1024;
            var file = CreateRandomFile(size);
            Task sendingTask = null;
            sendingTask = sender.TransferData(file.FullName, pair.Item1, new CancellationTokenSource().Token);

            // Wait till we have written a decent amount of the file, then
            // kill the transfer.
            while (new FileInfo(outputPath).Length < size / 2)
            {
                Thread.Sleep(1);
            }

            // Kill the transfer
            token.Cancel();

            // check the status of the streaming processes
            try
            {
                pair.Item2.Wait();
            }
            catch (AggregateException)
            {
                // We know it has been cancelled.
            }

            Assert.IsTrue(pair.Item2.IsCanceled);
            Assert.IsFalse(pair.Item2.IsFaulted);
            Assert.IsTrue(pair.Item2.IsCompleted);

            try
            {
                sendingTask.Wait();
            }
            catch (AggregateException)
            {
                // Pipe has died because the other side killed it.
            }

            Assert.IsFalse(sendingTask.IsCanceled);
            Assert.IsTrue(sendingTask.IsFaulted);
            Assert.IsTrue(sendingTask.IsCompleted);

            // Restart the operation.
            pair = recipient.PrepareForDataReception(outputPath, new CancellationToken());
            sendingTask = sender.TransferData(file.FullName, pair.Item1, new CancellationToken());
            sendingTask.ContinueWith(
                t =>
                {
                    Assert.IsTrue(t.IsCompleted);
                    Assert.IsFalse(t.IsFaulted);

                    using (var fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                    {
                        Assert.IsTrue(AreStreamsEqual(fileStream, new FileStream(outputPath, FileMode.Open, FileAccess.Read)));
                    }
                })
                .Wait();

            // Check that nothing went wrong on the other task
            pair.Item2.Wait();
        }
    }
}
