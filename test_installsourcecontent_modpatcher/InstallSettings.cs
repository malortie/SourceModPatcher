using System.IO.Abstractions;
using System.Text.Json.Serialization;
using test_installsourcecontent;

namespace test_installsourcecontent_modpatcher
{
    public sealed class JSONInstallSettingsEntry
    {
        [JsonPropertyName("install")]
        public bool Install { get; set; } = true;
    }

    public sealed class JSONInstallSettings : SortedDictionary<string, JSONInstallSettingsEntry>
    {
    }
    public sealed class InstallSettings : ConfigurationManager<JSONInstallSettings>
    {
        public InstallSettings(IFileSystem fileSystem, IWriter writer, string filePath, IConfigurationSerializer<JSONInstallSettings> configSerializer) : base(fileSystem, writer, filePath, configSerializer)
        {
        }

        public List<string> GetSourceModsToInstall()
        {
            return Config.Where(kv => kv.Value.Install).Select(kv => kv.Key).ToList();
        }

        public bool IsSourceModMarkedForInstall(string key)
        {
            return Config[key].Install;
        }
    }
}