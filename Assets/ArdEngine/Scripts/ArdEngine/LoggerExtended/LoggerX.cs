using System;
using System.Collections.Generic;
using ArdEngine.LoggerExtended.LogFormat;
using ArdEngine.LoggerExtended.LogOutput;
using UnityEngine;
using ArdEngine.ThreadTools;

namespace ArdEngine.LoggerExtended
{
    public sealed class LoggerX : IDisposable
    {
        private readonly LogConfigEntry[] _logConfig;
        private readonly List<LogFormatOutputPair> _outputs;
        private readonly Action<LogQueueEntry> _postLogDelegate;

        private bool _disposed;

        public LoggerX(LogConfigEntry[] logConfig)
        {
            _logConfig = logConfig;
            _outputs = new List<LogFormatOutputPair>();
            _postLogDelegate = PostLogToMainThread;

            for (var i = 0; i < _logConfig.Length; i++)
            {
                LogConfigEntry config = logConfig[i];
                config.Input.LogEmitted += _postLogDelegate;

                for (var j = 0; j < config.Outputs.Length; j++)
                {
                    ILogOutput output = config.Outputs[j];
                    ILogFormat format = FindFormatForOutput(output);
                    
                    if (format == null)
                    {
                        _outputs.Add(new LogFormatOutputPair(config.Format, output));
                        output.Open(config.Format.Header);
                        continue;
                    }
                    
                    if (format != config.Format)
                    {
                        Debug.LogError("The Logger setup is incorrect. " +
                                       $"There exist more than one format: {format} and {config.Format} " +
                                       $"for one output {output}");
                        _outputs.Clear();
                        return;
                    }
                }
            }
        }
        
        ~LoggerX()
        {
            Dispose(false);
        }
        
        public void SubmitLogs()
        {
            for (var i = 0; i < _outputs.Count; i++)
            {
                (ILogFormat format, ILogOutput output) = _outputs[i];
                output.Flush(format.Footer);
            }
        }
        
        private ILogFormat FindFormatForOutput(ILogOutput output)
        {
            ILogFormat format = null;
            for (var i = 0; i < _outputs.Count; i++)
            {
                if (_outputs[i].Output == output)
                {
                    format = _outputs[i].Format;
                }
            }

            return format;
        }

        private void PostLogToMainThread(LogQueueEntry log)
        {
            MainThreadDispatcher.QueueTask(() => PostLog(log));
        }

        private void PostLog(LogQueueEntry log)
        {
            ILogFormat format = EmptyFormat.Instance;
            ILogOutput[] outputs = Array.Empty<ILogOutput>();
            for (var i = 0; i < _logConfig.Length; i++)
            {
                if (_logConfig[i].Input == log.Input)
                {
                    format = _logConfig[i].Format;
                    outputs = _logConfig[i].Outputs;
                    break;
                }
            }

            string outputLog = format.Format(log.Entry);            
            for (var i = 0; i < outputs.Length; i++)
            {
                outputs[i].Log(outputLog);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                for (var i = 0; i < _logConfig.Length; i++)
                {
                    _logConfig[i].Input.LogEmitted -= _postLogDelegate;
                }
            }

            _disposed = true;
        }
    }
}