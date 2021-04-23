using ArdEngine.LoggerExtended;
using ArdEngine.LoggerExtended.LogFormat;
using ArdEngine.LoggerExtended.LogInput;
using ArdEngine.LoggerExtended.LogOutput;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace EditorTests.LoggerXTests
{
    public class LoggerXTest
    {
        private LoggerX _logger;
        private ILogHandler _defaultLogHandler;

        [SetUp]
        public void SetUp()
        {
            var logXInput = new LogXInput();
            var yamlLogFormat = new YamlLogFormat(new []
            {
                new HeaderData("Version", Application.version),
                new HeaderData("Game", Application.productName)
            });

            var logFileOutput = new FileOutput(Application.persistentDataPath, "yaml");
            
            _logger = new LoggerX(new[]
            {
                new LogConfigEntry(logXInput, yamlLogFormat, logFileOutput)
            });
            _defaultLogHandler = Debug.unityLogger.logHandler;
            Debug.unityLogger.logHandler = new DebugLogXHandler(logXInput);
        }

        [TearDown]
        public void TearDown()
        {
            _logger.SubmitLogs();
            _logger.Dispose();
            Debug.unityLogger.logHandler = _defaultLogHandler;
        }

        [Test]
        public void Log_ShowsInConsole()
        {
            const string logMessage = "[Audio] Some log regarding audio stuff.";
            LogAssert.Expect(LogType.Log, logMessage);
            Debug.Log(logMessage);
        }
    }
}