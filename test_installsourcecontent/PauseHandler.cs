namespace test_installsourcecontent
{
    public interface IPauseHandler
    {
        void Pause();
    }

    public class ConsolePauseHandler : IPauseHandler
    {
        IWriter _writer;

        public ConsolePauseHandler(IWriter writer)
        {
            _writer = writer;
        }

        public void Pause()
        {
            _writer.Write("Press any key to continue...");
            Console.ReadKey();
            _writer.WriteLine();
            _writer.WriteLine();
        }
    }
}