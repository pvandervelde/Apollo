//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Test.Regression.Explorer
{
    /// <summary>
    /// Defines methods that retry an action several times.
    /// </summary>
    internal static class Retry
    {
        /// <summary>
        /// Retries the given action a number of times before giving up.
        /// </summary>
        /// <typeparam name="T">The type of object that is returned.</typeparam>
        /// <param name="func">The function that should return the object.</param>
        /// <returns>The object or <see langword="null" /> if no value could be obtained.</returns>
        public static T Times<T>(Func<T> func) where T : class
        {
            return Times(func, TestConstants.MaximumRetryCount);
        }

        /// <summary>
        /// Retries the given action a number of times before giving up.
        /// </summary>
        /// <typeparam name="T">The type of object that is returned.</typeparam>
        /// <param name="func">The function that should return the object.</param>
        /// <param name="maximumNumberOfTries">The number of times the action should be attempted before giving up.</param>
        /// <returns>The object or <see langword="null" /> if no value could be obtained.</returns>
        public static T Times<T>(Func<T> func, int maximumNumberOfTries) where T : class
        {
            int retryCount = 0;
            T value = null;
            while ((value == null) && (retryCount < maximumNumberOfTries))
            {
                try
                {
                    value = func();
                }
                catch (Exception)
                {
                    value = null;
                }

                retryCount++;
            }

            return value;
        }
    }
}
