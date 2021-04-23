namespace ArdEngine.LoggerExtended.LogFormat
{
    public sealed class EmptyFormat : ILogFormat
    {
        public static readonly EmptyFormat Instance = new EmptyFormat();

        private EmptyFormat() {}
        
        public string Header => string.Empty;
        public string Footer => string.Empty;
        
        public string Format(LogEntry log)
        {
            return string.Empty;
        }
    }
}