using Pipelines;
using SourceContentInstaller;
using System.IO.Abstractions;
using System.Text.Json.Serialization;

namespace SourceModPatcher
{
    public sealed class JSONInstallSettingsEntry
    {
        [JsonPropertyName("install")]
        public bool Install { get; set; } = true;
    }

    public sealed class JSONInstallSettings : SortedDictionary<string, JSONInstallSettingsEntry>
    {
    }
    public sealed class InstallSettings(IFileSystem fileSystem, IWriter writer, string filePath, IConfigurationSerializer<JSONInstallSettings> configSerializer) : ConfigurationManager<JSONInstallSettings>(fileSystem, writer, filePath, configSerializer)
    {
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