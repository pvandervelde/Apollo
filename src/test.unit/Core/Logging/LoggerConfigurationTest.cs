//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using MbUnit.Framework;

namespace Apollo.Core.Logging
{
    [TestFixture]
    [Description("Tests the LoggerConfiguration class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class LoggerConfigurationTest
    {
        [Test]
        [Description("Checks a LoggerConfiguration cannot be created with a null target directory path.")]
        public void CreateWithNullTargetDirectoryPath()
        {
            Assert.Throws<ArgumentNullException>(() => new LoggerConfiguration(null, 1, 1));
        }

        [Test]
        [Description("Checks a LoggerConfiguration cannot be created with an empty target directory path.")]
        public void CreateWithEmptyTargetDirectoryPath()
        {
            Assert.Throws<ArgumentException>(() => new LoggerConfiguration(string.Empty, 1, 1));
        }

        [Test]
        [Description("Checks a LoggerConfiguration cannot be created with less than 1 message to buffer.")]
        public void CreateWithToFewMessagesToBuffer()
        {
            Assert.Throws<ArgumentException>(() => new LoggerConfiguration(Directory.GetCurrentDirectory(), 0, 1));
        }

        [Test]
        [Description("Checks a LoggerConfiguration cannot be created with a negative flush time.")]
        public void CreateWithTooLowFlushTime()
        {
            Assert.Throws<ArgumentException>(() => new LoggerConfiguration(Directory.GetCurrentDirectory(), 1, -1));
        }

        [Test]
        [Description("Checks a LoggerConfiguration can be created.")]
        public void Create()
        {
            var path = Directory.GetCurrentDirectory();
            var bufferSize = 10;
            var flushTime = 15;

            var config = new LoggerConfiguration(path, bufferSize, flushTime);

            Assert.AreEqual(path, config.TargetDirectory);
            Assert.AreEqual(bufferSize, config.NumberOfMessagesToBuffer);
            Assert.AreEqual(flushTime, config.FlushAfter);
        }
    }
}