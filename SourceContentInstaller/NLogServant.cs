using NLog;
using NLog.Targets;
using System.IO.Abstractions;

namespace SourceContentInstaller
{
    public class NLogServant
    {
        public void ClearAllLogFiles(NLog.Config.LoggingConfiguration loggingConfiguration, IFileSystem fileSystem)
        {
            foreach (var target in loggingConfiguration.AllTargets.OfType<FileTarget>())
            {
                var dummyEventInfo = new LogEventInfo { TimeStamp = DateTime.UtcNow };
                var logFilePath = target.FileName.Render(dummyEventInfo);
                var fullLogFilePath = fileSystem.Path.GetFullPath(logFilePath);
                if (fileSystem.File.Exists(fullLogFilePath))
                    fileSystem.File.WriteAllText(fullLogFilePath, string.Empty);
            }
        }
    }
}
