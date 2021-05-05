using System;

namespace ArdEngine.LoggerExtended.LogInput
{
    public sealed class LogXInput : ILogInput
    {
        public event Action<LogQueueEntry> LogEmitted;

        public void Log(LogEntry log)
        {
            LogEmitted?.Invoke(new LogQueueEntry(this, log));
        }

    }
}