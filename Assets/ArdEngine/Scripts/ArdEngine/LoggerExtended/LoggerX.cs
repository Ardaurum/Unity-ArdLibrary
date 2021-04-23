using System;
using System.Collections.Generic;
using ArdEngine.LoggerExtended.LogFormat;
using ArdEngine.LoggerExtended.LogOutput;
using UniRx;
using UnityEngine;
using MainThreadDispatcher = ArdEngine.ThreadTools.MainThreadDispatcher;

namespace ArdEngine.LoggerExtended
{
    public sealed class LoggerX : IDisposable
    {
        private readonly LogConfigEntry[] _logConfig;
        private readonly List<LogFormatOutputPair> _outputs;
        private readonly CompositeDisposable _disposables;

        public LoggerX(LogConfigEntry[] logConfig)
        {
            _logConfig = logConfig;
            _outputs = new List<LogFormatOutputPair>();
            _disposables = new CompositeDisposable();

            for (var i = 0; i < _logConfig.Length; i++)
            {
                LogConfigEntry config = logConfig[i];
                config.Input.LogStream
                    .Subscribe(entry => MainThreadDispatcher.QueueTask(() => PostLog(entry)))
                    .AddTo(_disposables);

                for (var j = 0; j < config.Outputs.Length; j++)
                {
                    ILogOutput output = config.Outputs[j];
                    (ILogFormat format, ILogOutput _) = _outputs.Find(pair => pair.Output == output);
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

        public void SubmitLogs()
        {
            for (var i = 0; i < _outputs.Count; i++)
            {
                (ILogFormat format, ILogOutput output) = _outputs[i];
                output.Flush(format.Footer);
            }
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}