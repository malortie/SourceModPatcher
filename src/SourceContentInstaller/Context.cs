using System.IO.Abstractions;

namespace SourceContentInstaller
{
    public class Context(IFileSystem fileSystem, IConfiguration configuration)
    {
        readonly IFileSystem _fileSystem = fileSystem;
        readonly IConfiguration _configuration = configuration;

        public IFileSystem FileSystem { get { return _fileSystem; } }

        public string ContentID { get; set; } = string.Empty;

        public string GetSteamAppName(int AppID)
        {
            return _configuration.GetSteamAppName(AppID);
        }

        public string GetSteamAppInstallDir(int AppID)
        {
            return _configuration.GetSteamAppInstallDir(AppID);
        }

        public Dictionary<string, string> GetSteamAppsInstallDirVariables()
        {
            return _configuration.GetSteamAppsInstallDirVariables();
        }

        public void SaveVariable(string name, string value)
        {
            _configuration.SaveVariable(name, value);
        }

        public string GetContentName()
        {
            return _configuration.GetContentName(ContentID);
        }

        public string GetContentInstallDir()
        {
            return _configuration.GetContentInstallDir(ContentID);
        }

        public string GetVariablesFileName()
        {
            return _configuration.GetVariablesFileName();
        }
    }
}