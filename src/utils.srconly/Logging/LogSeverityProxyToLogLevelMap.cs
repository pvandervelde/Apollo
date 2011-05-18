//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Utils.Logging
{
    /// <summary>
    /// Maps a <see cref="LogSeverityProxy"/> value to a <see cref="LevelToLog"/> value.
    /// </summary>
    internal static class LogSeverityProxyToLogLevelMap
    {
        /// <summary>
        /// Returns the correct value for the <see cref="LevelToLog"/> enum based
        /// on the given value of the <see cref="LogSeverityProxy"/> input value.
        /// </summary>
        /// <param name="proxy">The input value.</param>
        /// <returns>
        /// The <see cref="LevelToLog"/> value that matches the given 
        /// <see cref="LogSeverityProxy"/> value.
        /// </returns>
        public static LevelToLog FromLogSeverityProxy(LogSeverityProxy proxy)
        {
            switch (proxy)
            {
                case LogSeverityProxy.Trace:
                    return LevelToLog.Trace;
                case LogSeverityProxy.Debug:
                    return LevelToLog.Debug;
                case LogSeverityProxy.Info:
                    return LevelToLog.Info;
                case LogSeverityProxy.Warning:
                    return LevelToLog.Warn;
                case LogSeverityProxy.Error:
                    return LevelToLog.Error;
                case LogSeverityProxy.Fatal:
                    return LevelToLog.Fatal;
                case LogSeverityProxy.None:
                    return LevelToLog.None;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
