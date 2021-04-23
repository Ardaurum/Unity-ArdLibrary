using System.Collections.Generic;

namespace ArdEngine.LoggerExtended.LogOutput
{
    public sealed class BufferOutput : ILogOutput
    {
        private readonly ILogOutput[] _outputs;
        private readonly List<string> _entries;
        private string _header;
        
        public BufferOutput(params ILogOutput[] outputs)
        {
            _outputs = outputs;
            _entries = new List<string>();
        }

        public void Open(string header)
        {
            _header = header;
        }

        public void Log(string logEntry)
        {
            _entries.Add(logEntry);
        }

        public void Flush(string footer)
        {
            for (var i = 0; i < _outputs.Length; i++)
            {
                ILogOutput output = _outputs[i];
                output.Open(_header);
                for (var j = 0; j < _entries.Count; j++)
                {
                    output.Log(_entries[j]);
                }
                output.Flush(footer);
            }
        }
    }
}