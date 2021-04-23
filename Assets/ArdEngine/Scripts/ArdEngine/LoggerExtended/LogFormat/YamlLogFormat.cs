using System.Text;

namespace ArdEngine.LoggerExtended.LogFormat
{
    public sealed class YamlLogFormat : ILogFormat
    {
        private readonly StringBuilder _yamlBuilder;

        public string Header { get; }
        public string Footer => string.Empty;

        public YamlLogFormat(HeaderData[] headerData)
        {
            _yamlBuilder = new StringBuilder();
            for (var i = 0; i < headerData.Length; i++)
            {
                _yamlBuilder.AppendLine($"{headerData[i].Key}: \"{headerData[i].Value}\"");
            }

            _yamlBuilder.Append("Logs:"); 
            Header = _yamlBuilder.ToString();
        }
        
        public string Format(LogEntry log)
        {
            _yamlBuilder.Clear();
            var tag = string.Empty;
            if (log.Message[0] == '[')
            {
                int tagEnd = log.Message.IndexOf(']');
                tag = log.Message.Substring(1, tagEnd - 1);
            }

            string stackTrace = log.Stacktrace.Replace("\n", "\n      ");
            _yamlBuilder.AppendLine($"  - Type: \"{log.LogType}\"")
                        .AppendLine($"    Tag: \"{tag}\"")
                        .AppendLine($"    Time: \"{log.Time:yyyy-MM-dd HH:mm:ss}\"")
                        .AppendLine($"    Log: \"{log.Message}\"")
                        .AppendLine($"    Stack: |")
                        .AppendLine($"      {stackTrace}");

            return _yamlBuilder.ToString();
        }
    }
}