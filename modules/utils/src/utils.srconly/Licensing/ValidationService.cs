//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Lokad;

namespace Apollo.Utils.Licensing
{
    /// <summary>
    /// Implements the <see cref="IValidationService"/> interface.
    /// </summary>
    internal sealed partial class ValidationService : IValidationService, IDisposable
    {
        /// <summary>
        /// The collection of all the time sequences over which the license should be checked.
        /// </summary>
        private readonly List<ValidationSequence> m_Sequences = new List<ValidationSequence>();

        /// <summary>
        /// The collection of times on which the verification should take place.
        /// </summary>
        private readonly SortedList<DateTimeOffset, ValidationSequence> m_VerificationOrder
            = new SortedList<DateTimeOffset, ValidationSequence>();

        /// <summary>
        /// The delegate that returns the current time.
        /// </summary>
        private readonly Func<DateTimeOffset> m_Now;

        /// <summary>
        /// The action that is invoked in order to indicate that the service is still alive.
        /// </summary>
        private readonly Action<DateTimeOffset> m_OnIsAlive;

        /// <summary>
        /// The object that handles the validation.
        /// </summary>
        private readonly ILicenseValidator m_Validator;

        /// <summary>
        /// The timer which is used to fire the validation event.
        /// </summary>
        private readonly IProgressTimer m_ValidationTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationService"/> class.
        /// </summary>
        /// <param name="now">The delegate that returns the current time and date.</param>
        /// <param name="onIsAlive">The action that is invoked to indicate that the service is still alive.</param>
        /// <param name="timer">The timer that is used to trigger the verification at the right time interval.</param>
        /// <param name="validator">The object that handles the validation.</param>
        /// <param name="sequences">The collection of sequences over which the license should be validated.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="now"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="onIsAlive"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="timer"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="validator"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="sequences"/> is <see langword="null" />.
        /// </exception>
        public ValidationService(Func<DateTimeOffset> now, Action<DateTimeOffset> onIsAlive, IProgressTimer timer, ILicenseValidator validator, IEnumerable<ValidationSequence> sequences)
        {
            {
                Enforce.Argument(() => now);
                Enforce.Argument(() => onIsAlive);
                Enforce.Argument(() => timer);
                Enforce.Argument(() => validator);
                Enforce.Argument(() => sequences);
            }

            m_Now = now;
            m_OnIsAlive = onIsAlive;
            m_ValidationTimer = timer;
            m_Validator = validator;
            m_Sequences.AddRange(sequences);

            // Intialize the timer but do not start it. That will be done
            // once we start the validation process.
            {
                // Define the counter for the number of sequential failure so that
                // the compiler hoists it in the lambda class. No need to make it a 
                // member variable because we don't need external access.
                int sequentialFailedValidationCount = 0;
                m_ValidationTimer.Elapsed += 
                    (s, e) => 
                        {
                            Validate(ref sequentialFailedValidationCount);
                        };
            }
        }

        /// <summary>
        /// Starts the validation service.
        /// </summary>
        public void StartValidation()
        {
            if (m_Sequences.Count == 0)
            {
#if !DEPLOY
                throw new LicenseValidationFailedException();
#else
                // If there is an exception with setting the 
                // watchdog flag then we should immediately fail
                // because we cannot be certain that the other side
                // is still checking the verification status so
                // we simply exit
                Environment.FailFast(
                    string.Format(
                        CultureInfo.InvariantCulture, 
                        SrcOnlyResources.ExceptionMessagesInternalErrorWithCode, 
                        ErrorCodes.ValidationCannotValidateWithoutTimePeriods));
#endif
            }

            // Store all the sequences in a sorted order
            foreach (var sequence in m_Sequences)
            {
                ScheduleVerification(sequence.StartDate, sequence);
            }

            // Start the validation
            m_ValidationTimer.Start();
        }

        private void ScheduleVerification(DateTimeOffset startDate, ValidationSequence sequence)
        {
            var nextVerification = startDate + sequence.Period.RepeatAfter(startDate);

            // If the time already exists in the collection then we 
            // postpone the verification by 1 tick. It is expected that
            // there will not be many sequences which are the same because
            // that would defeat the purpose of periodic checking.
            while (m_VerificationOrder.ContainsKey(nextVerification))
            {
                nextVerification = nextVerification.AddTicks(1);
            }

            m_VerificationOrder.Add(nextVerification, sequence);
        }

        /// <summary>
        /// Run the validation based on the currently expired time sequences.
        /// </summary>
        /// <param name="sequentialFailedValidationCount">
        ///     The number of validations that have failed in sequence since the last
        ///     successful validation.
        /// </param>
        [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#",
            Justification = "Getting a reference to the value makes most sense since we want to update the same reference.")]
        private void Validate(ref int sequentialFailedValidationCount)
        {
            var now = m_Now();

            try
            {
                // Indicate that we're still alive
                m_OnIsAlive(now);
            }
            catch (Exception)
            {
#if !DEPLOY
                throw new LicenseValidationFailedException();
#else
                // If there is an exception with setting the 
                // watchdog flag then we should immediately fail
                // because we cannot be certain that the other side
                // is still checking the verification status so
                // we simply exit
                Environment.FailFast(
                    string.Format(
                        CultureInfo.InvariantCulture, 
                        SrcOnlyResources.ExceptionMessagesInternalErrorWithCode, 
                        ErrorCodes.ValidationWatchdogSetFailure));
#endif
            }

            // Run through all the invokations and invoke them when required. 
            // If an invokation fails then reschedule it to be done
            // the next time we pass through the loop
            var index = 0;
            var nextValidationTime = m_VerificationOrder.Keys[index];
            while (now >= nextValidationTime)
            {
                try
                {
                    var sequence = m_VerificationOrder.Values[index];
                    m_Validator.Verify(sequence.Period);

                    // Remove the current element. Do this first
                    // so that we are certain that we remove the 
                    // correct element
                    m_VerificationOrder.RemoveAt(index);

                    // Reschedule the verification if required
                    if (sequence.Period.IsPeriodic)
                    {
                        ScheduleVerification(now, sequence);
                    }

                    // Reset the failed verification count
                    sequentialFailedValidationCount = 0;
                }
                catch (Exception)
                {
                    // If more than x fail in a row (within a certain amount of time)
                    // then FailFast
                    sequentialFailedValidationCount++;
                    if (sequentialFailedValidationCount > LicensingConstants.MaximumNumberOfSequentialValidationFailures)
                    {
                        // If we aren't in deploy mode then we simply throw an exception. 
                        // Chances are that it'll be unhandled and take the app down. In
                        // deploy mode we take the app down ourselves.
#if !DEPLOY
                        throw new LicenseValidationFailedException();
#else
                        Environment.FailFast(
                            string.Format(
                                CultureInfo.InvariantCulture,
                                SrcOnlyResources.ExceptionMessagesInternalErrorWithCode,
                                ErrorCodes.ValidationExceededMaximumSequentialFailures));
#endif
                    }

                    // We simply leave the current item where it is and move on to the next one
                    index++;
                }

                // If we have handled all verifications then we simply wait for the next 
                // validation sequence.
                if (index >= m_VerificationOrder.Count)
                {
                    break;
                }

                // Get the next verification time
                nextValidationTime = m_VerificationOrder.Keys[index];
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Stop the timer
            m_ValidationTimer.Stop();

            // and then dispose of the timer
            IDisposable disposable = m_ValidationTimer as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}
