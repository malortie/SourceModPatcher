namespace SourceContentInstaller
{
    public interface IConfiguration
    {
        string GetSteamAppName(int AppID);
        string GetSteamAppInstallDir(int AppID);
        Dictionary<string, string> GetSteamAppsInstallDirVariables();
        void SaveVariable(string name, string value);
        string GetContentName(string contentID);
        string GetContentInstallDir(string contentID);
        string GetVariablesFileName();
    }

    public class Configuration(SteamAppsConfig steamAppsConfig, ContentsConfig contentsConfig, InstallSettings installSettings, VariablesConfig variablesConfig) : IConfiguration
    {
        readonly SteamAppsConfig _steamAppsConfig = steamAppsConfig;
        readonly ContentsConfig _contentsConfig = contentsConfig;
        readonly InstallSettings _installSettings = installSettings;
        readonly VariablesConfig _variablesConfig = variablesConfig;

        public string GetSteamAppName(int AppID)
        {
            return _steamAppsConfig.GetSteamAppName(AppID);
        }

        public string GetSteamAppInstallDir(int AppID)
        {
            return _steamAppsConfig.GetSteamAppInstallDir(AppID);
        }

        public Dictionary<string, string> GetSteamAppsInstallDirVariables()
        {
            return _steamAppsConfig.GetSteamAppsInstallDirVariables();
        }

        public void SaveVariable(string name, string value)
        {
            _variablesConfig.SaveVariable(name, value);
        }
        public string GetContentName(string contentID)
        {
            return _contentsConfig.GetContentName(contentID);
        }

        public string GetContentInstallDir(string ContentID)
        {
            return _installSettings.GetContentInstallDir(ContentID);
        }

        public string GetVariablesFileName()
        {
            return _variablesConfig.GetFileName();
        }
    }
}