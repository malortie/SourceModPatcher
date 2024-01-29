using System.Drawing;
using Pipelines;
using Pastel;

namespace SourceContentInstaller
{
    public class ConsoleWriter : IConsoleWriter
    {
        public void Success(string message)
        {
            Console.WriteLine(message.Pastel(Color.FromArgb(0, 255, 0)));
        }

        public void Info(string message)
        {
            Console.WriteLine(message.Pastel(Color.FromArgb(255, 255, 255)));
        }

        public void Warning(string message)
        {
            Console.WriteLine(message.Pastel(Color.FromArgb(255, 255, 0)));
        }

        public void Error(string message)
        {
            Console.WriteLine(message.Pastel(Color.FromArgb(255, 0, 0)));
        }

        public void Failure(string message)
        {
            Console.WriteLine(message.Pastel(Color.FromArgb(255, 0, 0)));
        }

        public void Cancellation(string message)
        {
            Console.WriteLine(message.Pastel(Color.FromArgb(255, 0, 0)));
        }
    }
}