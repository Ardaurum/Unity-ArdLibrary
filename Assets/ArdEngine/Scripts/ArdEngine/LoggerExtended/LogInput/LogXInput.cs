using System;
using UniRx;

namespace ArdEngine.LoggerExtended.LogInput
{
    public sealed class LogXInput : ILogInput
    {
        private readonly ReplaySubject<LogQueueEntry> _logSubject;
        public IObservable<LogQueueEntry> LogStream => _logSubject;

        public LogXInput()
        {
            _logSubject = new ReplaySubject<LogQueueEntry>(3);
        }

        public void Log(LogEntry log)
        {
            _logSubject.OnNext(new LogQueueEntry(this, log));
        }
    }
}