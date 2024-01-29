using Pipelines;

namespace SourceContentInstaller
{
    public class ConsolePauseHandler : IPauseHandler
    {
        IWriter _writer;

        public ConsolePauseHandler(IWriter writer)
        {
            _writer = writer;
        }

        public void Pause()
        {
            _writer.Info("Press any key to continue...");
            Console.ReadKey();
        }
    }
}