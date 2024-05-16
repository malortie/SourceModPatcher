using System.IO.Abstractions;

namespace SourceContentInstaller
{
    public class Context(IFileSystem fileSystem, IConfiguration configuration)
    {
        readonly IFileSystem _fileSystem = fileSystem;
        readonly IConfiguration _configuration = configuration;

        public IFileSystem FileSystem { get { return _fileSystem; } }

        public int AppID { get; set; } = 0;

        public string GetSteamAppName()
        {
            return _configuration.GetSteamAppName(AppID);
        }

        public string GetSteamAppInstallDir()
        {
            return _configuration.GetSteamAppInstallDir(AppID);
        }

        public void SaveVariable(string name, string value)
        {
            _configuration.SaveVariable(name, value);
        }

        public string GetContentInstallDir()
        {
            return _configuration.GetContentInstallDir(AppID);
        }

        public string GetVariablesFileName()
        {
            return _configuration.GetVariablesFileName();
        }
    }
}