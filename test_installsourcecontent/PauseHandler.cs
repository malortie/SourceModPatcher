namespace test_installsourcecontent
{
    public interface IPauseHandler
    {
        void Pause();
    }

    public class ConsolePauseHandler : IPauseHandler
    {
        ILogger _logger;

        public ConsolePauseHandler(ILogger logger)
        {
            _logger = logger;
        }

        public void Pause()
        {
            _logger.LogInfo("Press any key to continue...");
            Console.ReadKey();
        }
    }
}