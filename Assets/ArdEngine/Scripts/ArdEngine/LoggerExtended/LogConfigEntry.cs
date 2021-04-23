using ArdEngine.LoggerExtended.LogFormat;
using ArdEngine.LoggerExtended.LogInput;
using ArdEngine.LoggerExtended.LogOutput;

namespace ArdEngine.LoggerExtended
{
    public readonly struct LogConfigEntry
    {
        public readonly ILogInput Input;
        public readonly ILogFormat Format;
        public readonly ILogOutput[] Outputs;

        public LogConfigEntry(ILogInput input, ILogFormat format, params ILogOutput[] outputs)
        {
            Input = input;
            Format = format;
            Outputs = outputs;
        }
    }
}