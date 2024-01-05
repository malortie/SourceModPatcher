using System.IO.Abstractions;
using System.Text.Json.Serialization;
using Pipelines;
using test_installsourcecontent;

namespace test_installsourcecontent
{
    public sealed class JSONInstallSettingsEntry
    {
        [JsonPropertyName("install")]
        public bool Install { get; set; } = true;
        [JsonPropertyName("install_dir")]
        public string InstallDir { get; set; } = string.Empty;
    }

    public sealed class JSONInstallSettings : SortedDictionary<int, JSONInstallSettingsEntry>
    {
    }


    public sealed class InstallSettings : ConfigurationManager<JSONInstallSettings>
    {
        public InstallSettings(IFileSystem fileSystem, IWriter writer, string filePath, IConfigurationSerializer<JSONInstallSettings> configSerializer) : base(fileSystem, writer, filePath, configSerializer)
        {
        }

        public List<int> GetSteamAppsToInstall()
        {
            return Config.Where(kv => kv.Value.Install).Select(kv => kv.Key).ToList();
        }

        public bool IsSteamAppMarkedForInstall(int appID)
        {
            return Config[appID].Install;
        }

        public string GetContentInstallDir(int appID)
        {
            return Config[appID].InstallDir;
        }
    }
}