//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MbUnit.Framework;
using Moq;

namespace Apollo.Utilities.Licensing
{
    [TestFixture]
    [Description("Tests the ValidationService class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class ValidationServiceTest
    {
        [Test]
        [Description("Checks that an ValidationService instance cannot be created with a null Date delegate reference.")]
        public void CreateWithNullDateDelegate()
        { 
            Action<DateTimeOffset> isAlive = time => { };
            var timer = new Mock<IProgressTimer>();
            var validator = new Mock<ILicenseValidator>();
            var sequences = new List<ValidationSequence>();

            Assert.Throws<ArgumentNullException>(() => new ValidationService(null, isAlive, timer.Object, validator.Object, sequences));
        }

        [Test]
        [Description("Checks that an ValidationService instance cannot be created with a null IsAlive delegate reference.")]
        public void CreateWithNullIsAliveDelegate()
        {
            Func<DateTimeOffset> datetime = () => DateTimeOffset.Now;
            var timer = new Mock<IProgressTimer>();
            var validator = new Mock<ILicenseValidator>();
            var sequences = new List<ValidationSequence>();

            Assert.Throws<ArgumentNullException>(() => new ValidationService(datetime, null, timer.Object, validator.Object, sequences));
        }

        [Test]
        [Description("Checks that an ValidationService instance cannot be created with a null timer reference.")]
        public void CreateWithNullTimer()
        {
            Func<DateTimeOffset> datetime = () => DateTimeOffset.Now;
            Action<DateTimeOffset> isAlive = time => { };
            var validator = new Mock<ILicenseValidator>();
            var sequences = new List<ValidationSequence>();

            Assert.Throws<ArgumentNullException>(() => new ValidationService(datetime, isAlive, null, validator.Object, sequences));
        }

        [Test]
        [Description("Checks that an ValidationService instance cannot be created with a null license validator reference.")]
        public void CreateWithNullLicenseValidator()
        {
            Func<DateTimeOffset> datetime = () => DateTimeOffset.Now;
            Action<DateTimeOffset> isAlive = time => { };
            var timer = new Mock<IProgressTimer>();
            var sequences = new List<ValidationSequence>();

            Assert.Throws<ArgumentNullException>(() => new ValidationService(datetime, isAlive, timer.Object, null, sequences));
        }

        [Test]
        [Description("Checks that an ValidationService instance cannot be created with a null reference to period collection.")]
        public void CreateWithNullPeriodCollection()
        {
            Func<DateTimeOffset> datetime = () => DateTimeOffset.Now;
            Action<DateTimeOffset> isAlive = time => { };
            var timer = new Mock<IProgressTimer>();
            var validator = new Mock<ILicenseValidator>();

            Assert.Throws<ArgumentNullException>(() => new ValidationService(datetime, isAlive, timer.Object, validator.Object, null));
        }

        [Test]
        [Description("Checks that starting the verification without any verification periods leads to an exception.")]
        public void StartValidationWithoutVerificationPeriods()
        {
            var now = DateTimeOffset.Now;
            Func<DateTimeOffset> datetime = () => now;
            Action<DateTimeOffset> isAlive = time => { };
            var timer = new Mock<IProgressTimer>();
            var validator = new Mock<ILicenseValidator>();
            var sequences = new List<ValidationSequence>();

            var service = new ValidationService(datetime, isAlive, timer.Object, validator.Object, sequences);
            Assert.Throws<LicenseValidationFailedException>(() => service.StartValidation());
        }

        [Test]
        [Description("Checks that running the verification with a failing IsAlive delegate leads to an exception.")]
        public void StartValidationWithFailingIsAliveDelegate()
        {
            var now = DateTimeOffset.Now;
            Func<DateTimeOffset> datetime = () => now;
            Action<DateTimeOffset> isAlive = time => { throw new Exception(); };
            var timer = new Mock<IProgressTimer>();
            var validator = new Mock<ILicenseValidator>();
            var sequences = new List<ValidationSequence> { new ValidationSequence(new TimePeriod(RepeatPeriod.Hourly), now) };

            var service = new ValidationService(datetime, isAlive, timer.Object, validator.Object, sequences);
            service.StartValidation();

            Assert.Throws<LicenseValidationFailedException>(() => timer.Raise(t => t.Elapsed += null, new TimerElapsedEventArgs(DateTime.Now)));
        }

        [Test]
        [Description("Checks that running the verification with a single period is successful.")]
        public void StartValidationWithSinglePeriod()
        {
            var now = DateTimeOffset.Now;
            Func<DateTimeOffset> datetime = () => now;
            Action<DateTimeOffset> isAlive = time => { };
            var timer = new Mock<IProgressTimer>();
            var validator = new Mock<ILicenseValidator>();
            {
                validator.Setup(v => v.Verify())
                    .Verifiable();
                validator.Setup(v => v.Verify(It.IsAny<TimePeriod>()))
                    .Verifiable();
            }

            var sequences = new List<ValidationSequence> 
                { 
                    new ValidationSequence(new TimePeriod(RepeatPeriod.Hourly), now),
                    new ValidationSequence(new TimePeriod(RepeatPeriod.Hourly, 2), now),
                    new ValidationSequence(new TimePeriod(RepeatPeriod.Hourly, 3), now) 
                };

            var service = new ValidationService(datetime, isAlive, timer.Object, validator.Object, sequences);
            service.StartValidation();

            // Make sure we don't match the time exactly, otherwise if we're off by
            // 1 tick, then we don't match. By adding an extra minute we are safe-guarding
            // against near-misses or near-hits
            now = DateTimeOffset.Now.AddMinutes(61);
            timer.Raise(t => t.Elapsed += null, new TimerElapsedEventArgs(DateTime.Now));
            validator.Verify(v => v.Verify(It.IsAny<TimePeriod>()), Times.Exactly(1));

            // Rerun the validation. This should hit the
            // 2-hourly sequence and the 1 hourly sequence (which
            // has been rescheduled from the previous pass.
            //
            // Make sure we don't match the time exactly, otherwise if we're off by
            // 1 tick, then we don't match. By adding an extra minute we are safe-guarding
            // against near-misses or near-hits
            now = DateTimeOffset.Now.AddMinutes(121);
            timer.Raise(t => t.Elapsed += null, new TimerElapsedEventArgs(DateTime.Now));
            validator.Verify(v => v.Verify(It.IsAny<TimePeriod>()), Times.Exactly(3));
        }

        [Test]
        [Description("Checks that running the verification with sequential failing verifications can be successful if there are fewer exceptions than the maximum.")]
        public void StartValidationWithSequentialFailuresBelowMaximumForSequentialVerifications()
        {
            var now = DateTimeOffset.Now;
            Func<DateTimeOffset> datetime = () => now;
            Action<DateTimeOffset> isAlive = time => { };
            var timer = new Mock<IProgressTimer>();
            var validator = new Mock<ILicenseValidator>();
            {
                int callCount = 0;
                validator.Setup(v => v.Verify())
                    .Verifiable();
                validator.Setup(v => v.Verify(It.IsAny<TimePeriod>()))
                    .Callback<TimePeriod>(p => 
                        {
                            callCount++;
                            if (callCount <= 2)
                            {
                                throw new Exception();
                            }
                        })
                    .Verifiable();
            }

            var periods = new List<ValidationSequence> 
                { 
                    new ValidationSequence(new TimePeriod(RepeatPeriod.Hourly), now),
                    new ValidationSequence(new TimePeriod(RepeatPeriod.Hourly, 2), now),
                    new ValidationSequence(new TimePeriod(RepeatPeriod.Hourly, 3), now) 
                };

            var service = new ValidationService(datetime, isAlive, timer.Object, validator.Object, periods);
            service.StartValidation();

            // Make sure we don't match the time exactly, otherwise if we're off by
            // 1 tick, then we don't match. By adding an extra minute we are safe-guarding
            // against near-misses or near-hits
            now = DateTimeOffset.Now.AddMinutes(241);
            timer.Raise(t => t.Elapsed += null, new TimerElapsedEventArgs(DateTime.Now));
            validator.Verify(v => v.Verify(It.IsAny<TimePeriod>()), Times.Exactly(3));
        }

        [Test]
        [Description("Checks that running the verification with non-sequential failing verifications can be successful if there are fewer exceptions than the maximum.")]
        public void StartValidationWithSequentialFailuresBelowMaximumForNonSequentialVerifications()
        {
            var now = DateTimeOffset.Now;
            Func<DateTimeOffset> datetime = () => now;
            Action<DateTimeOffset> isAlive = time => { };
            var timer = new Mock<IProgressTimer>();
            var validator = new Mock<ILicenseValidator>();
            {
                int callCount = 0;
                validator.Setup(v => v.Verify())
                    .Verifiable();
                validator.Setup(v => v.Verify(It.IsAny<TimePeriod>()))
                    .Callback<TimePeriod>(p =>
                    {
                        callCount++;
                        if (callCount <= 2)
                        {
                            throw new Exception();
                        }
                    })
                    .Verifiable();
            }
            
            var periods = new List<ValidationSequence> 
                { 
                    new ValidationSequence(new TimePeriod(RepeatPeriod.Hourly), now),
                    new ValidationSequence(new TimePeriod(RepeatPeriod.Hourly, 2), now),
                    new ValidationSequence(new TimePeriod(RepeatPeriod.Hourly, 3), now) 
                };

            var service = new ValidationService(datetime, isAlive, timer.Object, validator.Object, periods);
            service.StartValidation();

            // Make sure we don't match the time exactly, otherwise if we're off by
            // 1 tick, then we don't match. By adding an extra minute we are safe-guarding
            // against near-misses or near-hits
            now = DateTimeOffset.Now.AddMinutes(121);
            timer.Raise(t => t.Elapsed += null, new TimerElapsedEventArgs(DateTime.Now));
            validator.Verify(v => v.Verify(It.IsAny<TimePeriod>()), Times.Exactly(2));

            // Make sure we don't match the time exactly, otherwise if we're off by
            // 1 tick, then we don't match. By adding an extra minute we are safe-guarding
            // against near-misses or near-hits
            now = DateTimeOffset.Now.AddMinutes(241);
            timer.Raise(t => t.Elapsed += null, new TimerElapsedEventArgs(DateTime.Now));
            validator.Verify(v => v.Verify(It.IsAny<TimePeriod>()), Times.Exactly(5));
        }

        [Test]
        [Description("Checks that running the verification with a failing verification throws an exception if there are more exceptions than the maximum.")]
        public void StartValidationWithSequentialFailuresAboveMaximumForSequentialVerifications()
        {
            var now = DateTimeOffset.Now;
            Func<DateTimeOffset> datetime = () => now;
            Action<DateTimeOffset> isAlive = time => { };
            var timer = new Mock<IProgressTimer>();
            var validator = new Mock<ILicenseValidator>();
            {
                int callCount = 0;
                validator.Setup(v => v.Verify())
                    .Verifiable();
                validator.Setup(v => v.Verify(It.IsAny<TimePeriod>()))
                    .Callback<TimePeriod>(p =>
                    {
                        callCount++;
                        if (callCount <= 4)
                        {
                            throw new Exception();
                        }
                    })
                    .Verifiable();
            }

            var periods = new List<ValidationSequence> 
                { 
                    new ValidationSequence(new TimePeriod(RepeatPeriod.Hourly), now),
                    new ValidationSequence(new TimePeriod(RepeatPeriod.Hourly, 2), now),
                    new ValidationSequence(new TimePeriod(RepeatPeriod.Hourly, 3), now),
                    new ValidationSequence(new TimePeriod(RepeatPeriod.Hourly, 4), now),
                };

            var service = new ValidationService(datetime, isAlive, timer.Object, validator.Object, periods);
            service.StartValidation();

            // Make sure we don't match the time exactly, otherwise if we're off by
            // 1 tick, then we don't match. By adding an extra minute we are safe-guarding
            // against near-misses or near-hits
            now = DateTimeOffset.Now.AddMinutes(301);
            Assert.Throws<LicenseValidationFailedException>(() => timer.Raise(t => t.Elapsed += null, new TimerElapsedEventArgs(DateTime.Now)));
            validator.Verify(v => v.Verify(It.IsAny<TimePeriod>()), Times.Exactly(4));
        }

        [Test]
        [Description("Checks that running the verification with a failing verification throws an exception if there are more exceptions than the maximum.")]
        public void StartValidationWithSequentialFailuresAboveMaximumForNonSequentialVerifications()
        {
            var now = DateTimeOffset.Now;
            Func<DateTimeOffset> datetime = () => now;
            Action<DateTimeOffset> isAlive = time => { };
            var timer = new Mock<IProgressTimer>();
            var validator = new Mock<ILicenseValidator>();
            {
                int callCount = 0;
                validator.Setup(v => v.Verify())
                    .Verifiable();
                validator.Setup(v => v.Verify(It.IsAny<TimePeriod>()))
                    .Callback<TimePeriod>(p =>
                    {
                        callCount++;
                        if (callCount <= 4)
                        {
                            throw new Exception();
                        }
                    })
                    .Verifiable();
            }

            var periods = new List<ValidationSequence> 
                { 
                    new ValidationSequence(new TimePeriod(RepeatPeriod.Hourly), now),
                    new ValidationSequence(new TimePeriod(RepeatPeriod.Hourly, 2), now),
                    new ValidationSequence(new TimePeriod(RepeatPeriod.Hourly, 3), now),
                    new ValidationSequence(new TimePeriod(RepeatPeriod.Hourly, 4), now),
                };

            var service = new ValidationService(datetime, isAlive, timer.Object, validator.Object, periods);
            service.StartValidation();

            // Make sure we don't match the time exactly, otherwise if we're off by
            // 1 tick, then we don't match. By adding an extra minute we are safe-guarding
            // against near-misses or near-hits
            now = DateTimeOffset.Now.AddMinutes(121);
            timer.Raise(t => t.Elapsed += null, new TimerElapsedEventArgs(DateTime.Now));
            validator.Verify(v => v.Verify(It.IsAny<TimePeriod>()), Times.Exactly(2));

            // Make sure we don't match the time exactly, otherwise if we're off by
            // 1 tick, then we don't match. By adding an extra minute we are safe-guarding
            // against near-misses or near-hits
            now = DateTimeOffset.Now.AddMinutes(241);
            Assert.Throws<LicenseValidationFailedException>(() => timer.Raise(t => t.Elapsed += null, new TimerElapsedEventArgs(DateTime.Now)));
            validator.Verify(v => v.Verify(It.IsAny<TimePeriod>()), Times.Exactly(4));
        }

        [Test]
        [Description("Checks that running the verification with multiple periods is successful.")]
        public void StartValidationWithMultiplePeriods()
        {
            var now = DateTimeOffset.Now;
            Func<DateTimeOffset> datetime = () => now;
            Action<DateTimeOffset> isAlive = time => { };
            var timer = new Mock<IProgressTimer>();
            var validator = new Mock<ILicenseValidator>();
            {
                validator.Setup(v => v.Verify())
                    .Verifiable();
                validator.Setup(v => v.Verify(It.IsAny<TimePeriod>()))
                    .Verifiable();
            }

            var periods = new List<ValidationSequence> 
                { 
                    new ValidationSequence(new TimePeriod(RepeatPeriod.Hourly), now),
                    new ValidationSequence(new TimePeriod(RepeatPeriod.Hourly, 2), now),
                    new ValidationSequence(new TimePeriod(RepeatPeriod.Hourly, 3), now),
                    new ValidationSequence(new TimePeriod(RepeatPeriod.Hourly, 4), now),
                };

            var service = new ValidationService(datetime, isAlive, timer.Object, validator.Object, periods);
            service.StartValidation();

            // Make sure we don't match the time exactly, otherwise if we're off by
            // 1 tick, then we don't match. By adding an extra minute we are safe-guarding
            // against near-misses or near-hits
            now = DateTimeOffset.Now.AddMinutes(61);
            timer.Raise(t => t.Elapsed += null, new TimerElapsedEventArgs(DateTime.Now));
            validator.Verify(v => v.Verify(It.IsAny<TimePeriod>()), Times.Exactly(1));

            // Make sure we don't match the time exactly, otherwise if we're off by
            // 1 tick, then we don't match. By adding an extra minute we are safe-guarding
            // against near-misses or near-hits
            now = DateTimeOffset.Now.AddMinutes(121);
            timer.Raise(t => t.Elapsed += null, new TimerElapsedEventArgs(DateTime.Now));
            validator.Verify(v => v.Verify(It.IsAny<TimePeriod>()), Times.Exactly(3));

            // Make sure we don't match the time exactly, otherwise if we're off by
            // 1 tick, then we don't match. By adding an extra minute we are safe-guarding
            // against near-misses or near-hits
            now = DateTimeOffset.Now.AddMinutes(481);
            timer.Raise(t => t.Elapsed += null, new TimerElapsedEventArgs(DateTime.Now));
            validator.Verify(v => v.Verify(It.IsAny<TimePeriod>()), Times.Exactly(7));
        }
    }
}
