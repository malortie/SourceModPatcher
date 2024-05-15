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

        public void SaveVariable(string name, string value)
        {
        }

        public string GetContentInstallDir(int AppID)
        {
            return string.Empty;
        }

        public string GetVariablesFileName()
        {
            return string.Empty;
        }
    }
}