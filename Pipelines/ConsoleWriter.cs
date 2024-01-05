namespace Pipelines
{
    public interface IConsoleWriter
    {
        void Success(string message);
        void Info(string message);
        void Warning(string message);
        void Error(string message);
        void Failure(string message);
        void Cancellation(string message);
    }
}