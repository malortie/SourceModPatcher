namespace Pipelines.Tests
{
    public class LoggerMock : ILogger
    {
        public int SuccessTotal { get; private set; } = 0;
        public int InfoTotal { get; private set; } = 0;
        public int WarningTotal { get; private set; } = 0;
        public int ErrorTotal { get; private set; } = 0;
        public int FailureTotal { get; private set; } = 0;
        public int CancellationTotal { get; private set; } = 0;

        public void Success(string message)
        {
            ++SuccessTotal;
        }

        public void Info(string message)
        {
            ++InfoTotal;
        }

        public void Warning(string message)
        {
            ++WarningTotal;
        }

        public void Error(string message)
        {
            ++ErrorTotal;
        }

        public void Failure(string message)
        {
            ++FailureTotal;
        }

        public void Cancellation(string message)
        {
            ++CancellationTotal;
        }
    }

    public class ConsoleWriterMock : IConsoleWriter
    {
        public int SuccessTotal { get; private set; } = 0;
        public int InfoTotal { get; private set; } = 0;
        public int WarningTotal { get; private set; } = 0;
        public int ErrorTotal { get; private set; } = 0;
        public int FailureTotal { get; private set; } = 0;
        public int CancellationTotal { get; private set; } = 0;

        public void Success(string message)
        {
            ++SuccessTotal;
        }

        public void Info(string message)
        {
            ++InfoTotal;
        }

        public void Warning(string message)
        {
            ++WarningTotal;
        }

        public void Error(string message)
        {
            ++ErrorTotal;
        }

        public void Failure(string message)
        {
            ++FailureTotal;
        }

        public void Cancellation(string message)
        {
            ++CancellationTotal;
        }
    }

    [TestClass]
    public class TestWriter
    {
        [TestMethod] public void Success_Calls_Logger_And_ConsoleWriter_Success() 
        {
            var logger = new LoggerMock();
            var consoleWriter = new ConsoleWriterMock();
            var writer = new Writer(logger, consoleWriter);

            writer.Success(string.Empty);

            Assert.AreEqual(1, logger.SuccessTotal);
            Assert.AreEqual(1, consoleWriter.SuccessTotal);
        }

        [TestMethod]
        public void Info_Calls_Logger_And_ConsoleWriter_Info()
        {
            var logger = new LoggerMock();
            var consoleWriter = new ConsoleWriterMock();
            var writer = new Writer(logger, consoleWriter);

            writer.Info(string.Empty);

            Assert.AreEqual(1, logger.InfoTotal);
            Assert.AreEqual(1, consoleWriter.InfoTotal);
        }

        [TestMethod]
        public void Warning_Calls_Logger_And_ConsoleWriter_Warning()
        {
            var logger = new LoggerMock();
            var consoleWriter = new ConsoleWriterMock();
            var writer = new Writer(logger, consoleWriter);

            writer.Warning(string.Empty);

            Assert.AreEqual(1, logger.WarningTotal);
            Assert.AreEqual(1, consoleWriter.WarningTotal);
        }

        [TestMethod]
        public void Error_Calls_Logger_And_ConsoleWriter_Error()
        {
            var logger = new LoggerMock();
            var consoleWriter = new ConsoleWriterMock();
            var writer = new Writer(logger, consoleWriter);

            writer.Error(string.Empty);

            Assert.AreEqual(1, logger.ErrorTotal);
            Assert.AreEqual(1, consoleWriter.ErrorTotal);
        }

        [TestMethod]
        public void Failure_Calls_Logger_And_ConsoleWriter_Failure()
        {
            var logger = new LoggerMock();
            var consoleWriter = new ConsoleWriterMock();
            var writer = new Writer(logger, consoleWriter);

            writer.Failure(string.Empty);

            Assert.AreEqual(1, logger.FailureTotal);
            Assert.AreEqual(1, consoleWriter.FailureTotal);
        }

        [TestMethod]
        public void Cancellation_Calls_Logger_And_ConsoleWriter_Cancellation()
        {
            var logger = new LoggerMock();
            var consoleWriter = new ConsoleWriterMock();
            var writer = new Writer(logger, consoleWriter);

            writer.Cancellation(string.Empty);

            Assert.AreEqual(1, logger.CancellationTotal);
            Assert.AreEqual(1, consoleWriter.CancellationTotal);
        }
    }
}