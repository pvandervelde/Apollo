//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using MbUnit.Framework;
using Moq;

namespace Apollo.Utils
{
    [TestFixture]
    [Description("Tests the Id class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class TimeBasedProgressTrackerTest
    {
        #region internal class - MockMark1

        private sealed class MockMark1 : IProgressMark
        {
        }
        
        #endregion

        #region internal class - MockMark2

        private sealed class MockMark2 : IProgressMark
        {
        }
        
        #endregion

        #region internal class - MockTimer

        private sealed class MockTimer : IProgressTimer
        {
            public void Start()
            {
                // Do nothing
            }

            public void Stop()
            {
                // do nothing
            }

            public event EventHandler<TimerElapsedEventArgs> Elapsed;

            public void RaiseElapsed(DateTime time)
            {
                var local = Elapsed;
                if (local != null)
                {
                    local(this, new TimerElapsedEventArgs(time));
                }
            }
        }

        #endregion

        [Test]
        [Description("Checks that an object cannot be created with a null timer.")]
        public void CreateWithNullTimer()
        {
            Assert.Throws<ArgumentNullException>(() => new TimeBasedProgressTracker(null, -1, new Mock<IStoreMarkerTimes>().Object));
        }

        [Test]
        [Description("Checks that an object cannot be created with an unknownprogress value between 0 and 100.")]
        public void CreateWithIncorrectProgressValue()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new TimeBasedProgressTracker(new Mock<IProgressTimer>().Object, 10, new Mock<IStoreMarkerTimes>().Object));
        }

        [Test]
        [Description("Checks that an object cannot be created without an IStoreMarkerTimes object.")]
        public void CreateWithNullMarkerStore()
        {
            Assert.Throws<ArgumentNullException>(() => new TimeBasedProgressTracker(new Mock<IProgressTimer>().Object, -1, null));
        }

        [Test]
        [Description("Checks that the MarkAdded event is raised correctly.")]
        public void MarkRaised()
        {
            IProgressMark storedMark = null;
            var tracker = new TimeBasedProgressTracker(new Mock<IProgressTimer>().Object, -1, new Mock<IStoreMarkerTimes>().Object);
            tracker.MarkAdded += (s, e) => 
                {
                    storedMark = e.Mark;
                };

            var mark = new Mock<IProgressMark>();
            tracker.Mark(mark.Object);

            Assert.AreSame(mark.Object, storedMark);
        }

        [Test]
        [Description("Checks that the StartupProgress event is raised correctly.")]
        public void StartupProgress()
        {
            var store = new Mock<IStoreMarkerTimes>();
            {
                store.Setup(s => s.TotalTime)
                    .Returns(new TimeSpan(0, 0, 40));
            }

            var timer = new MockTimer();

            IProgressMark storedMark = null;
            int storedProgress = 0;
            var tracker = new TimeBasedProgressTracker(timer, -1, store.Object);
            tracker.StartupProgress += (s, e) => 
                {  
                    storedMark = e.CurrentlyProcessing;
                    storedProgress = e.Progress;
                };

            var mark1 = new MockMark1();
            var mark2 = new MockMark2();

            var now = DateTime.Now;
            tracker.StartTracking();
            tracker.Mark(mark1);

            timer.RaiseElapsed(now.AddSeconds(10.0));

            // wait for a bit so that the threadpool can catch up ...
            Thread.Sleep(20);

            Assert.AreEqual(mark1, storedMark);
            Assert.AreEqual(25, storedProgress);

            tracker.Mark(mark2);

            timer.RaiseElapsed(now.AddSeconds(30.0));

            // wait for a bit so that the threadpool can catch up ...
            Thread.Sleep(20);

            Assert.AreEqual(mark2, storedMark);
            Assert.AreEqual(75, storedProgress);

            tracker.StopTracking();
        }
    }
}
