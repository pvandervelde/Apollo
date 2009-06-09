// Copyright (c) P. van der Velde. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MbUnit.Framework;

namespace Apollo.Core.Test.Unit
{
    [TestFixture]
    public sealed class StartupProgressEventArgsTest
    {
        [Test]
        public void Create()
        {
            int progress = 10;
            string action = "SomeAction";

            var args = new StartupProgressEventArgs(progress, action);
            Assert.AreEqual<int>(progress, args.Progress);
            Assert.AreEqual<string>(action, args.CurrentlyProcessing);
        }

        [Test]
        public void CreateWithNullString()
        {
            int progress = 10;
            string action = null;

            var args = new StartupProgressEventArgs(progress, action);
            Assert.AreEqual<int>(progress, args.Progress);
            Assert.AreEqual<string>(string.Empty, args.CurrentlyProcessing);
        }

        [Test]
        public void CreateWithEmptyString()
        {
            int progress = 10;
            string action = string.Empty;

            var args = new StartupProgressEventArgs(progress, action);
            Assert.AreEqual<int>(progress, args.Progress);
            Assert.AreEqual<string>(action, args.CurrentlyProcessing);
        }

        [Test]
        public void CreateWithTooLowNumber()
        {
            var args = new StartupProgressEventArgs(-10, null);
            Assert.Fail("Shouldn't be able to get here...");
        }

        [Test]
        public void CreateWithTooHighNumber()
        {
            var args = new StartupProgressEventArgs(110, null);
            Assert.Fail("Shouldn't be able to get here...");
        }
    }
}
