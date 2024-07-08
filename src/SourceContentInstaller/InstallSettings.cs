using Pipelines;
using System.IO.Abstractions;
using System.Text.Json.Serialization;

namespace SourceContentInstaller
{
    public sealed class JSONInstallSettingsEntry
    {
        [JsonPropertyName("install")]
        public bool Install { get; set; } = true;
        [JsonPropertyName("install_dir")]
        public string InstallDir { get; set; } = string.Empty;
    }

    public sealed class JSONInstallSettings : SortedDictionary<string, JSONInstallSettingsEntry>
    {
    }


    public class InstallSettings(IFileSystem fileSystem, IWriter writer, string filePath, IConfigurationSerializer<JSONInstallSettings> configSerializer) : ConfigurationManager<JSONInstallSettings>(fileSystem, writer, filePath, configSerializer)
    {
        public virtual List<string> GetContentsToInstall()
        {
            return Config.Where(kv => kv.Value.Install).Select(kv => kv.Key).ToList();
        }

        public virtual bool IsContentMarkedForInstall(string contentID)
        {
            return Config[contentID].Install;
        }

        public virtual string GetContentInstallDir(string contentID)
        {
            return Config[contentID].InstallDir;
        }
    }
}