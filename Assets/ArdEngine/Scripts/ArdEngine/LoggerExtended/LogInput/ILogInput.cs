using System;

namespace ArdEngine.LoggerExtended.LogInput
{
    public interface ILogInput
    {
        IObservable<LogQueueEntry> LogStream { get; }
    }
}