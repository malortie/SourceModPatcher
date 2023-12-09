using System.IO.Abstractions;

namespace test_installsourcecontent
{
    public class Context
    {
        IFileSystem _fileSystem;
        IConfiguration _configuration;

        public Context(IFileSystem fileSystem, IConfiguration configuration)
        {
            _fileSystem = fileSystem;
            _configuration = configuration;
        }

        public IFileSystem FileSystem { get { return _fileSystem; } }

        public int AppID { get; set; } = 0;

        public Dictionary<string, string> ContextVariables = new();

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
    }
}