namespace test_installsourcecontent
{
    public interface IWriter
    {
        void Success(string message);
        void Info(string message);
        void Warning(string message);
        void Error(string message);
        void Failure(string message);
        void Cancellation(string message);
    }

    public class Writer : IWriter
    {
        readonly ILogger _logger;
        readonly IConsoleWriter _consoleWriter;

        public Writer(ILogger logger, IConsoleWriter consoleWriter)
        {
            _logger = logger;
            _consoleWriter = consoleWriter;
        }

        public void Success(string message)
        {
            _consoleWriter.Success(message);
            _logger.Success(message);
        }

        public void Info(string message)
        {
            _consoleWriter.Info(message);
            _logger.Info(message);
        }

        public void Warning(string message)
        {
            _consoleWriter.Warning(message);
            _logger.Warning(message);
        }

        public void Error(string message)
        {
            _consoleWriter.Error(message);
            _logger.Error(message);
        }

        public void Failure(string message)
        {
            _consoleWriter.Failure(message);
            _logger.Failure(message);
        }

        public void Cancellation(string message)
        {
            _consoleWriter.Cancellation(message);
            _logger.Cancellation(message);
        }
    }
}
