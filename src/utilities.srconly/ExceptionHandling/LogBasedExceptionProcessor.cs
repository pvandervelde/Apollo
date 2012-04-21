﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Apollo.Utilities.Logging;
using Lokad;

namespace Apollo.Utilities.ExceptionHandling
{
    /// <summary>
    /// An exception processor that writes the exception out to an <see cref="ILogger"/> object.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal sealed class LogBasedExceptionProcessor : IExceptionProcessor
    {
        /// <summary>
        /// The logger which is used to store the exception message.
        /// </summary>
        private readonly ILogger m_Logger;

        /// <summary>
        /// Indicates if the object has been disposed or not.
        /// </summary>
        private volatile bool m_WasDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogBasedExceptionProcessor"/> class.
        /// </summary>
        /// <param name="logger">
        ///     The logger which is used to store the exception message.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="logger"/> is <see langword="null" />.
        /// </exception>
        public LogBasedExceptionProcessor(ILogger logger)
        {
            {
                Enforce.Argument(() => logger);
            }

            m_Logger = logger;
        }

        /// <summary>
        /// Processes the given exception.
        /// </summary>
        /// <param name="exception">The exception to process.</param>
        public void Process(Exception exception)
        {
            if (m_WasDisposed)
            {
                return;
            }

            string text = string.Empty;
            if (exception != null)
            {
                text = string.Format(
                    CultureInfo.InvariantCulture,
                    "Fatal exception occurred during application execution. Exception message was: {0}",
                    exception);
            }
            else
            {
                text = string.Format(
                    CultureInfo.InvariantCulture,
                    "Fatal exception occurred during application execution. No exception or stack trace provided.");
            }

            var msg = new LogMessage(
                LevelToLog.Fatal,
                text,
                new Dictionary<string, object>() 
                        { 
                            { AdditionalLogMessageProperties.EventCategory, EventTypeToEventCategoryMap.EventCategory(EventType.Exception) },
                            { AdditionalLogMessageProperties.EventId, ExceptionTypeToEventIdMap.EventIdForException(exception) },
                        });

            try
            {
                m_Logger.Log(msg);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (m_WasDisposed)
            {
                return;
            }

            m_Logger.Close();
            m_WasDisposed = true;
        }
    }
}
