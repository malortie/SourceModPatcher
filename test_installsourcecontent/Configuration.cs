namespace test_installsourcecontent
{
    public interface IConfiguration
    {
        string GetSteamAppName(int AppID);
        string GetSteamAppInstallDir(int AppID);
        void SaveVariable(string name, string value);
        string GetContentInstallDir(int AppID);
    }

    public class Configuration : IConfiguration
    {
        SteamAppsConfig _steamAppsConfig;
        InstallSettings _installSettings;
        VariablesConfig _variablesConfig;

        public Configuration(SteamAppsConfig steamAppsConfig, InstallSettings installSettings, VariablesConfig variablesConfig)
        {
            _steamAppsConfig = steamAppsConfig;
            _installSettings = installSettings;
            _variablesConfig = variablesConfig;
        }

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
    }
}