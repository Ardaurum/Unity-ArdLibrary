using System;
using System.IO;
using UnityEngine;

namespace ArdEngine.LoggerExtended.LogOutput
{
    public sealed class FileOutput : ILogOutput
    {
        private readonly StreamWriter _streamWriter;

        public FileOutput(string path, string extension)
        {
            var fileName = $"{Application.productName}.{DateTime.Now:yyyy.MM.dd.HH.mm.ss}.{extension}";
            _streamWriter = new StreamWriter(Path.Combine(path, fileName), false);
        }
        
        public void Open(string header)
        {
            _streamWriter.WriteLine(header);
        }

        public void Log(string logEntry)
        {
            _streamWriter.WriteLine(logEntry);
        }

        public void Flush(string footer)
        {
            _streamWriter.WriteLine(footer);
            _streamWriter.Close();
        }
    }
}