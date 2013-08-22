//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.IO;
using Apollo.UI.Wpf.Feedback;
using Moq;
using NUnit.Framework;

namespace Apollo.UI.Wpf.Commands
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class SendFeedbackReportCommandTest
    {
        [Test]
        public void CanSendWithoutSender()
        {
            var command = new SendFeedbackReportCommand(null);
            Assert.IsFalse(command.CanExecute(new MemoryStream()));
        }

        [Test]
        public void CanSendWithoutReport()
        {
            var sender = new Mock<ISendFeedbackReports>();

            var command = new SendFeedbackReportCommand(sender.Object);
            Assert.IsFalse(command.CanExecute(null));
        }

        [Test]
        public void Send()
        {
            var sender = new Mock<ISendFeedbackReports>();
            {
                sender.Setup(s => s.Send(It.IsAny<Stream>()))
                    .Verifiable();
            }

            var command = new SendFeedbackReportCommand(sender.Object);
            command.Execute(new MemoryStream());

            sender.Verify(s => s.Send(It.IsAny<Stream>()), Times.Once());
        }
    }
}
