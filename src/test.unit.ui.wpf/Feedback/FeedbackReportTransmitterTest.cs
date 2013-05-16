//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.IO;
using MbUnit.Framework;
using Moq;
using NSarrac.Framework;

namespace Apollo.UI.Wpf.Feedback
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class FeedbackReportTransmitterTest
    {
        [Test]
        public void Send()
        {
            var stream = new MemoryStream();
            var sender = new Mock<IReportSender>();
            {
                sender.Setup(s => s.SendReport(It.IsAny<Stream>()))
                    .Callback<Stream>(s => Assert.AreSame(stream, s))
                    .Verifiable();
            }

            var transmitter = new FeedbackReportTransmitter(sender.Object);
            transmitter.Send(stream);

            sender.Verify(s => s.SendReport(It.IsAny<Stream>()), Times.Once());
        }

        [Test]
        public void SendWithError()
        {
            var stream = new MemoryStream();
            var sender = new Mock<IReportSender>();
            {
                sender.Setup(s => s.SendReport(It.IsAny<Stream>()))
                    .Callback<Stream>(s =>
                    {
                        throw new CouldNotConnectToTheRemoteServiceException();
                    })
                    .Verifiable();
            }

            var transmitter = new FeedbackReportTransmitter(sender.Object);
            Assert.Throws<FailedToSendFeedbackReportException>(() => transmitter.Send(stream));
        }
    }
}
