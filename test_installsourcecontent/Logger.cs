namespace test_installsourcecontent
{
    public interface ILogger
    {
        void LogSuccess(string message);
        void LogInfo(string message);
        void LogWarning(string message);
        void LogError(string message);
    }
}
