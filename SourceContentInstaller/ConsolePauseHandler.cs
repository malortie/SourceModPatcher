using Pipelines;

namespace SourceContentInstaller
{
    public class ConsolePauseHandler(IWriter writer) : IPauseHandler
    {
        readonly IWriter _writer = writer;

        public void Pause()
        {
            _writer.Info("Press any key to continue...");
            Console.ReadKey();
        }
    }
}