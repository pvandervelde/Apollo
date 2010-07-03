//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MbUnit.Framework;

namespace Apollo.Utils.Licensing
{
    [TestFixture]
    [Description("Tests the ValidationService class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class ValidationServiceTest
    {
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

        #region internal class - MockLicenseValidator

        private sealed class MockLicenseValidator : ILicenseValidator
        {
            private readonly bool m_ShouldThrow = false;
            private readonly int m_ThrowFor = 0;

            private int m_CalledSoFar = 0;

            public MockLicenseValidator()
            { 
            }

            public MockLicenseValidator(bool shouldThrow, int throwFor)
            {
                m_ShouldThrow = shouldThrow;
                m_ThrowFor = throwFor;
            }

            public void Verify()
            {
                WasVerified = true;
                m_CalledSoFar++;

                if (m_ShouldThrow && (m_CalledSoFar <= m_ThrowFor))
                {
                    throw new Exception();
                }
            }

            public void Verify(TimePeriod nextExpiration)
            {
                WasVerified = true;
                Period = nextExpiration;
                m_CalledSoFar++;

                if (m_ShouldThrow && (m_CalledSoFar <= m_ThrowFor))
                {
                    throw new Exception();
                }
            }

            public bool WasVerified
            {
                get;
                set;
            }

            public TimePeriod Period
            {
                get;
                set;
            }

            public int CalledSoFar
            {
                get
                {
                    return m_CalledSoFar;
                }
            }
        }
        
        #endregion

        [Test]
        [Description("Checks that an ValidationService instance cannot be created with a null Date delegate reference.")]
        public void CreateWithNullDateDelegate()
        { 
            Action<DateTimeOffset> isAlive = time => { };
            var timer = new MockTimer();
            var validator = new MockLicenseValidator();
            var periods = new List<TimePeriod>();

            Assert.Throws<ArgumentNullException>(() => new ValidationService(null, isAlive, timer, validator, periods));
        }

        [Test]
        [Description("Checks that an ValidationService instance cannot be created with a null IsAlive delegate reference.")]
        public void CreateWithNullIsAliveDelegate()
        {
            Func<DateTimeOffset> datetime = () => DateTimeOffset.Now;
            var timer = new MockTimer();
            var validator = new MockLicenseValidator();
            var periods = new List<TimePeriod>();

            Assert.Throws<ArgumentNullException>(() => new ValidationService(datetime, null, timer, validator, periods));
        }

        [Test]
        [Description("Checks that an ValidationService instance cannot be created with a null timer reference.")]
        public void CreateWithNullTimer()
        {
            Func<DateTimeOffset> datetime = () => DateTimeOffset.Now;
            Action<DateTimeOffset> isAlive = time => { };
            var validator = new MockLicenseValidator();
            var periods = new List<TimePeriod>();

            Assert.Throws<ArgumentNullException>(() => new ValidationService(datetime, isAlive, null, validator, periods));
        }

        [Test]
        [Description("Checks that an ValidationService instance cannot be created with a null license validator reference.")]
        public void CreateWithNullLicenseValidator()
        {
            Func<DateTimeOffset> datetime = () => DateTimeOffset.Now;
            Action<DateTimeOffset> isAlive = time => { };
            var timer = new MockTimer();
            var periods = new List<TimePeriod>();

            Assert.Throws<ArgumentNullException>(() => new ValidationService(datetime, isAlive, timer, null, periods));
        }

        [Test]
        [Description("Checks that an ValidationService instance cannot be created with a null reference to period collection.")]
        public void CreateWithNullPeriodCollection()
        {
            Func<DateTimeOffset> datetime = () => DateTimeOffset.Now;
            Action<DateTimeOffset> isAlive = time => { };
            var timer = new MockTimer();
            var validator = new MockLicenseValidator();

            Assert.Throws<ArgumentNullException>(() => new ValidationService(datetime, isAlive, timer, validator, null));
        }

        [Test]
        [Description("Checks that starting the verification without any verification periods leads to an exception.")]
        public void StartValidationWithoutVerificationPeriods()
        {
            var now = DateTimeOffset.Now;
            Func<DateTimeOffset> datetime = () => now;
            Action<DateTimeOffset> isAlive = time => { };
            var timer = new MockTimer();
            var validator = new MockLicenseValidator();
            var periods = new List<TimePeriod>();

            var service = new ValidationService(datetime, isAlive, timer, validator, periods);
            Assert.Throws<LicenseVerificationFailedException>(() => service.StartValidation());
        }

        [Test]
        [Description("Checks that running the verification with a failing IsAlive delegate leads to an exception.")]
        public void StartValidationWithFailingIsAliveDelegate()
        {
            var now = DateTimeOffset.Now;
            Func<DateTimeOffset> datetime = () => now;
            Action<DateTimeOffset> isAlive = time => { throw new Exception(); };
            var timer = new MockTimer();
            var validator = new MockLicenseValidator();
            var periods = new List<TimePeriod> { new TimePeriod(RepeatPeriod.Hourly) };

            var service = new ValidationService(datetime, isAlive, timer, validator, periods);
            service.StartValidation();

            Assert.Throws<LicenseVerificationFailedException>(() => timer.RaiseElapsed(DateTime.Now));
        }

        [Test]
        [Description("Checks that running the verification with a single period is successful.")]
        [Ignore("This test makes MbUnit hang. Will not run for now.")]
        public void StartValidationWithSinglePeriod()
        {
            var now = DateTimeOffset.Now;
            Func<DateTimeOffset> datetime = () => now;
            Action<DateTimeOffset> isAlive = time => { };
            var timer = new MockTimer();
            var validator = new MockLicenseValidator();
            var periods = new List<TimePeriod> 
                { 
                    new TimePeriod(RepeatPeriod.Hourly),
                    new TimePeriod(RepeatPeriod.Hourly, 2),
                    new TimePeriod(RepeatPeriod.Hourly, 3) 
                };

            var service = new ValidationService(datetime, isAlive, timer, validator, periods);
            service.StartValidation();

            now = DateTimeOffset.Now.AddHours(1);
            timer.RaiseElapsed(DateTime.Now);
            Assert.AreEqual(1, validator.CalledSoFar);

            // Rerun the validation. This should hit the
            // 2-hourly period and the 1 hourly period (which
            // has been rescheduled from the previous pass
            now = DateTimeOffset.Now.AddHours(2);
            timer.RaiseElapsed(DateTime.Now);
            Assert.AreEqual(3, validator.CalledSoFar);
        }

        [Test]
        [Description("Checks that running the verification with sequential failing verifications can be successful if there are fewer exceptions than the maximum.")]
        public void StartValidationWithSequentialFailuresBelowMaximumForSequentialVerifications()
        {
            var now = DateTimeOffset.Now;
            Func<DateTimeOffset> datetime = () => now;
            Action<DateTimeOffset> isAlive = time => { };
            var timer = new MockTimer();
            var validator = new MockLicenseValidator(true, 2);
            var periods = new List<TimePeriod> 
                { 
                    new TimePeriod(RepeatPeriod.Hourly),
                    new TimePeriod(RepeatPeriod.Hourly, 2),
                    new TimePeriod(RepeatPeriod.Hourly, 3) 
                };

            var service = new ValidationService(datetime, isAlive, timer, validator, periods);
            service.StartValidation();

            now = DateTimeOffset.Now.AddHours(4);
            timer.RaiseElapsed(DateTime.Now);
            Assert.AreEqual(periods.Count, validator.CalledSoFar);
        }

        [Test]
        [Description("Checks that running the verification with non-sequential failing verifications can be successful if there are fewer exceptions than the maximum.")]
        public void StartValidationWithSequentialFailuresBelowMaximumForNonSequentialVerifications()
        {
            var now = DateTimeOffset.Now;
            Func<DateTimeOffset> datetime = () => now;
            Action<DateTimeOffset> isAlive = time => { };
            var timer = new MockTimer();
            var validator = new MockLicenseValidator(true, 2);
            var periods = new List<TimePeriod> 
                { 
                    new TimePeriod(RepeatPeriod.Hourly),
                    new TimePeriod(RepeatPeriod.Hourly, 2),
                    new TimePeriod(RepeatPeriod.Hourly, 3) 
                };

            var service = new ValidationService(datetime, isAlive, timer, validator, periods);
            service.StartValidation();

            now = DateTimeOffset.Now.AddHours(2);
            timer.RaiseElapsed(DateTime.Now);
            Assert.AreEqual(2, validator.CalledSoFar);

            now = DateTimeOffset.Now.AddHours(4);
            timer.RaiseElapsed(DateTime.Now);
            Assert.AreEqual(5, validator.CalledSoFar);
        }

        [Test]
        [Description("Checks that running the verification with a failing verification throws an exception if there are more exceptions than the maximum.")]
        public void StartValidationWithSequentialFailuresAboveMaximumForSequentialVerifications()
        {
            var now = DateTimeOffset.Now;
            Func<DateTimeOffset> datetime = () => now;
            Action<DateTimeOffset> isAlive = time => { };
            var timer = new MockTimer();
            var validator = new MockLicenseValidator(true, 4);
            var periods = new List<TimePeriod> 
                { 
                    new TimePeriod(RepeatPeriod.Hourly),
                    new TimePeriod(RepeatPeriod.Hourly, 2),
                    new TimePeriod(RepeatPeriod.Hourly, 3),
                    new TimePeriod(RepeatPeriod.Hourly, 4),
                };

            var service = new ValidationService(datetime, isAlive, timer, validator, periods);
            service.StartValidation();

            now = DateTimeOffset.Now.AddHours(5);
            Assert.Throws<LicenseVerificationFailedException>(() => timer.RaiseElapsed(DateTime.Now));
            Assert.AreEqual(periods.Count, validator.CalledSoFar);
        }

        [Test]
        [Description("Checks that running the verification with a failing verification throws an exception if there are more exceptions than the maximum.")]
        public void StartValidationWithSequentialFailuresAboveMaximumForNonSequentialVerifications()
        {
            var now = DateTimeOffset.Now;
            Func<DateTimeOffset> datetime = () => now;
            Action<DateTimeOffset> isAlive = time => { };
            var timer = new MockTimer();
            var validator = new MockLicenseValidator(true, 4);
            var periods = new List<TimePeriod> 
                { 
                    new TimePeriod(RepeatPeriod.Hourly),
                    new TimePeriod(RepeatPeriod.Hourly, 2),
                    new TimePeriod(RepeatPeriod.Hourly, 3),
                    new TimePeriod(RepeatPeriod.Hourly, 4),
                };

            var service = new ValidationService(datetime, isAlive, timer, validator, periods);
            service.StartValidation();

            now = DateTimeOffset.Now.AddHours(2);
            timer.RaiseElapsed(DateTime.Now);
            Assert.AreEqual(2, validator.CalledSoFar);

            now = DateTimeOffset.Now.AddHours(4);
            Assert.Throws<LicenseVerificationFailedException>(() => timer.RaiseElapsed(DateTime.Now));
            Assert.AreEqual(4, validator.CalledSoFar);
        }

        [Test]
        [Description("Checks that running the verification with multiple periods is successful.")]
        [Ignore("This test makes MbUnit hang. Will not run for now.")]
        public void StartValidationWithMultiplePeriods()
        {
            var now = DateTimeOffset.Now;
            Func<DateTimeOffset> datetime = () => now;
            Action<DateTimeOffset> isAlive = time => { };
            var timer = new MockTimer();
            var validator = new MockLicenseValidator();
            var periods = new List<TimePeriod> 
                { 
                    new TimePeriod(RepeatPeriod.Hourly),
                    new TimePeriod(RepeatPeriod.Hourly, 2),
                    new TimePeriod(RepeatPeriod.Hourly, 3),
                    new TimePeriod(RepeatPeriod.Hourly, 4),
                };

            var service = new ValidationService(datetime, isAlive, timer, validator, periods);
            service.StartValidation();

            now = DateTimeOffset.Now.AddHours(1);
            timer.RaiseElapsed(DateTime.Now);
            Assert.AreEqual(1, validator.CalledSoFar);

            now = DateTimeOffset.Now.AddHours(2);
            timer.RaiseElapsed(DateTime.Now);
            Assert.AreEqual(3, validator.CalledSoFar);

            now = DateTimeOffset.Now.AddHours(8);
            timer.RaiseElapsed(DateTime.Now);
            Assert.AreEqual(7, validator.CalledSoFar);
        }
    }
}
