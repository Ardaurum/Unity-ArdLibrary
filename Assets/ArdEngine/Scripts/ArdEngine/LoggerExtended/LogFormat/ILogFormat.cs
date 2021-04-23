namespace ArdEngine.LoggerExtended.LogFormat
{
    public interface ILogFormat
    {
        string Header { get; }
        string Footer { get; }
        string Format(LogEntry log);
    }
}