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

namespace Apollo.Utilities
{
    [TestFixture]
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

        [Test]
        public void MarkRaised()
        {
            IProgressMark storedMark = null;
            var tracker = new TimeBasedProgressTracker(new Mock<IProgressTimer>().Object, -1, new Mock<IStoreMarkerTimes>().Object);
            tracker.OnMarkAdded += (s, e) => 
                {
                    storedMark = e.Mark;
                };

            var mark = new Mock<IProgressMark>();
            tracker.Mark(mark.Object);

            Assert.AreSame(mark.Object, storedMark);
        }

        [Test]
        public void StartupProgressWithoutCurrentMark()
        {
            var store = new Mock<IStoreMarkerTimes>();
            {
                store.Setup(s => s.TotalTime)
                    .Returns(new TimeSpan(0, 0, 40));
            }

            var timer = new Mock<IProgressTimer>();
            var tracker = new TimeBasedProgressTracker(timer.Object, -1, store.Object);

            Assert.Throws<CurrentProgressMarkNotSetException>(() => tracker.StartTracking());
        }

        [Test]
        public void StartupProgress()
        {
            var store = new Mock<IStoreMarkerTimes>();
            {
                store.Setup(s => s.TotalTime)
                    .Returns(new TimeSpan(0, 0, 40));
            }

            var timer = new Mock<IProgressTimer>();

            IProgressMark storedMark = null;
            int storedProgress = 0;
            var tracker = new TimeBasedProgressTracker(timer.Object, -1, store.Object);
            tracker.OnStartupProgress += (s, e) =>
                {
                    storedMark = e.CurrentlyProcessing;
                    storedProgress = e.Progress;
                };

            var mark1 = new MockMark1();
            var mark2 = new MockMark2();

            var now = DateTime.Now;
            tracker.Mark(mark1);
            tracker.StartTracking();

            timer.Raise(t => t.OnElapsed += null, new TimerElapsedEventArgs(now.AddSeconds(10.0)));

            // wait for a bit so that the threadpool can catch up ...
            Thread.Sleep(20);

            Assert.AreEqual(mark1, storedMark);
            Assert.AreEqual(25, storedProgress);

            tracker.Mark(mark2);

            timer.Raise(t => t.OnElapsed += null, new TimerElapsedEventArgs(now.AddSeconds(30.0)));

            // wait for a bit so that the threadpool can catch up ...
            Thread.Sleep(20);

            Assert.AreEqual(mark2, storedMark);
            Assert.AreEqual(75, storedProgress);

            tracker.StopTracking();
        }

        [Test]
        public void StartupProgressWithProgressPastMaximumProgress()
        {
            var store = new Mock<IStoreMarkerTimes>();
            {
                store.Setup(s => s.TotalTime)
                    .Returns(new TimeSpan(0, 0, 5));
            }

            var timer = new Mock<IProgressTimer>();

            IProgressMark storedMark = null;
            int storedProgress = 0;
            var tracker = new TimeBasedProgressTracker(timer.Object, -1, store.Object);
            tracker.OnStartupProgress += (s, e) =>
            {
                storedMark = e.CurrentlyProcessing;
                storedProgress = e.Progress;
            };

            var mark1 = new MockMark1();

            var now = DateTime.Now;
            tracker.Mark(mark1);
            tracker.StartTracking();

            timer.Raise(t => t.OnElapsed += null, new TimerElapsedEventArgs(now.AddSeconds(10.0)));

            // wait for a bit so that the threadpool can catch up ...
            Thread.Sleep(20);

            Assert.AreEqual(mark1, storedMark);
            Assert.AreEqual(100, storedProgress);

            tracker.StopTracking();
        }
    }
}
