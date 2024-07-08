namespace SourceContentInstaller.Tests
{
    public class NullConfiguration : IConfiguration
    {
        public NullConfiguration()
        {
        }

        public string GetSteamAppName(int AppID)
        {
            return string.Empty;
        }

        public string GetSteamAppInstallDir(int AppID)
        {
            return string.Empty;
        }

        public Dictionary<string, string> GetSteamAppsInstallDirVariables()
        {
            return new();
        }

        public void SaveVariable(string name, string value)
        {
        }
        public string GetContentName(string ContentID)
        {
            return string.Empty;
        }

        public string GetContentInstallDir(string ContentID)
        {
            return string.Empty;
        }

        public string GetVariablesFileName()
        {
            return string.Empty;
        }
    }
}