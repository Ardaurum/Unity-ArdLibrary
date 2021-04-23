using System;
using ArdEngine.LoggerExtended.LogInput;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ArdEngine.LoggerExtended
{
    public sealed class DebugLogXHandler : ILogHandler
    {
        private readonly LogXInput _input;
        private readonly ILogHandler _defaultLogHandler = Debug.unityLogger.logHandler;

        public DebugLogXHandler(LogXInput input)
        {
            _input = input;
        }
        
        public void LogFormat(LogType logType, Object context, string format, params object[] args)
        {
            string stackTrace = StackTraceUtility.ExtractStackTrace();
            _input.Log(new LogEntry(string.Format(format, args), stackTrace, logType));
            _defaultLogHandler.LogFormat(logType, context, format, args);
        }

        public void LogException(Exception exception, Object context)
        {
            string stackTrace = StackTraceUtility.ExtractStringFromException(exception);
            _input.Log(new LogEntry(exception.Message, stackTrace, LogType.Exception));
            _defaultLogHandler.LogException(exception, context);
        }
    }
}