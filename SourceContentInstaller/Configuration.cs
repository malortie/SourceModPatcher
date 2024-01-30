namespace SourceContentInstaller
{
    public interface IConfiguration
    {
        string GetSteamAppName(int AppID);
        string GetSteamAppInstallDir(int AppID);
        void SaveVariable(string name, string value);
        string GetContentInstallDir(int AppID);
        string GetVariablesFileName();
    }

    public class Configuration(SteamAppsConfig steamAppsConfig, InstallSettings installSettings, VariablesConfig variablesConfig) : IConfiguration
    {
        readonly SteamAppsConfig _steamAppsConfig = steamAppsConfig;
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

        public void SaveVariable(string name, string value)
        {
            _variablesConfig.SaveVariable(name, value);
        }

        public string GetContentInstallDir(int AppID)
        {
            return _installSettings.GetContentInstallDir(AppID);
        }

        public string GetVariablesFileName()
        {
            return _variablesConfig.GetFileName();
        }
    }
}