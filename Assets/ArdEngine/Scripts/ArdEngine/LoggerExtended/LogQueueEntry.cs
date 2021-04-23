using ArdEngine.LoggerExtended.LogInput;

namespace ArdEngine.LoggerExtended
{
    public readonly struct LogQueueEntry
    {
        public readonly ILogInput Input;
        public readonly LogEntry Entry;

        public LogQueueEntry(ILogInput input, LogEntry entry)
        {
            Input = input;
            Entry = entry;
        }
    }
}