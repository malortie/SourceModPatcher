using Pipelines;

namespace test_installsourcecontent
{
    public class Logger : ILogger
    {
        readonly NLog.Logger _logger;
        public Logger(NLog.Logger logger)
        {
            _logger = logger;
        }

        public void Cancellation(string message)
        {
            _logger.Error(message);
        }

        public void Error(string message)
        {
            _logger.Error(message);
        }

        public void Failure(string message)
        {
            _logger.Error(message);
        }

        public void Info(string message)
        {
            _logger.Info(message);
        }

        public void Success(string message)
        {
            _logger.Info(message);
        }

        public void Warning(string message)
        {
            _logger.Warn(message);
        }
    }
}