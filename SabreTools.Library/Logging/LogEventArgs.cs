﻿using System;

namespace SabreTools.Library.Logging
{
    /// <summary>
    /// Generic delegate type for log events
    /// </summary>
    public delegate void LogEventHandler(object sender, LogEventArgs args);

    /// <summary>
    /// Logging specific event arguments
    /// </summary>
    public class LogEventArgs : EventArgs
    {
        /// <summary>
        /// LogLevel for the event
        /// </summary>
        public LogLevel LogLevel { get; set; } = LogLevel.VERBOSE;

        /// <summary>
        /// Log statement to be printed
        /// </summary>
        public string Statement { get; set; } = null;

        /// <summary>
        /// Exception to be passed along to the event handler
        /// </summary>
        public Exception Exception { get; set; } = null;

        /// <summary>
        /// Total count for progress log events
        /// </summary>
        public long? TotalCount { get; set; } = null;

        /// <summary>
        /// Current count for progress log events
        /// </summary>
        public long? CurrentCount { get; set; } = null;

        /// <summary>
        /// Statement and exception constructor
        /// </summary>
        public LogEventArgs(LogLevel logLevel = LogLevel.VERBOSE, string statement = null, Exception exception = null)
        {
            this.LogLevel = logLevel;
            this.Statement = statement;
            this.Exception = exception;
        }

        /// <summary>
        /// Progress constructor
        /// </summary>
        public LogEventArgs(long total, long current, LogLevel logLevel = LogLevel.VERBOSE, string statement = null)
        {
            this.LogLevel = logLevel;
            this.Statement = statement;
            this.TotalCount = total;
            this.CurrentCount = current;
        }
    }
}
