namespace ArdEngine.LoggerExtended.LogOutput
{
    public interface ILogOutput
    {
        void Open(string header);
        void Log(string logEntry);
        void Flush(string footer);
    }
}