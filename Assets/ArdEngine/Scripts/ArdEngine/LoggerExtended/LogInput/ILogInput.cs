using System;

namespace ArdEngine.LoggerExtended.LogInput
{
    public interface ILogInput
    {
        event Action<LogQueueEntry> LogEmitted;
    }
}