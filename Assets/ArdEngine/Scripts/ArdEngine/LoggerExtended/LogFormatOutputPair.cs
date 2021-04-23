using ArdEngine.LoggerExtended.LogFormat;
using ArdEngine.LoggerExtended.LogOutput;

namespace ArdEngine.LoggerExtended
{
    public readonly struct LogFormatOutputPair
    {
        public readonly ILogFormat Format;
        public readonly ILogOutput Output;

        public LogFormatOutputPair(ILogFormat format, ILogOutput output)
        {
            Format = format;
            Output = output;
        }
        
        public void Deconstruct(out ILogFormat format, out ILogOutput output)
        {
            format = Format;
            output = Output;
        }
    }
}