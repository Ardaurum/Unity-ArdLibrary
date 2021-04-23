using System;
using UnityEngine;

namespace ArdEngine.LoggerExtended
{
    public readonly struct LogEntry : IEquatable<LogEntry>
    {
        public readonly string Message;
        public readonly string Stacktrace;
        public readonly LogType LogType;
        public readonly DateTime Time;

        public LogEntry(string message, string stacktrace, LogType logType, DateTime time)
        {
            Message = message;
            Stacktrace = stacktrace;
            LogType = logType;
            Time = time;
        }

        public LogEntry(string message, string stacktrace, LogType logType) 
            : this(message, stacktrace, logType, DateTime.Now)
        {
        }
        
        public bool Equals(LogEntry other)
        {
            return Message == other.Message
                   && Stacktrace == other.Stacktrace
                   && LogType == other.LogType;
        }

        public override bool Equals(object obj)
        {
            return obj is LogEntry other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Message != null ? Message.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (Stacktrace != null ? Stacktrace.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int) LogType;
                return hashCode;
            }
        }

        public static bool operator ==(LogEntry left, LogEntry right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(LogEntry left, LogEntry right)
        {
            return !left.Equals(right);
        }
    }
}