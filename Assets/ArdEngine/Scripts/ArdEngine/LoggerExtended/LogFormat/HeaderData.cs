namespace ArdEngine.LoggerExtended.LogFormat
{
    public readonly struct HeaderData
    {
        public readonly string Key;
        public readonly string Value;

        public HeaderData(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}