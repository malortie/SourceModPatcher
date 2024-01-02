using System.IO.Abstractions;
using System.Text.Json.Serialization;
using test_installsourcecontent;

namespace test_installsourcecontent_modpatcher
{
    public class JSONSourceModsConfigEntry
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("sourcemod_folder")]
        public string SourceModFolder { get; set; } = string.Empty;
        [JsonPropertyName("data_dir")]
        public string DataDir { get; set; } = string.Empty;
    }

    public class JSONSourceModsConfig : SortedDictionary<string, JSONSourceModsConfigEntry>
    {
    }

    public sealed class SourceModsConfig : ConfigurationManager<JSONSourceModsConfig>
    {
        public SourceModsConfig(IFileSystem fileSystem, IWriter writer, string filePath, IConfigurationSerializer<JSONSourceModsConfig> configSerializer, string sourceModInstallPath) : base(fileSystem, writer, filePath, configSerializer)
        {
            SourceModInstallPath = sourceModInstallPath;
        }

        public List<string> SupportedSourceModsKeys = [];

        public string SourceModInstallPath { get; private set; }

        public string GetSourceModName(string key)
        {
            return Config[key].Name;
        }

        public string GetSourceModFolder(string key)
        {
            return Config[key].SourceModFolder;
        }

        public string GetSourceModDir(string key)
        {
            return PathExtensions.JoinWithSeparator(FileSystem, SourceModInstallPath, GetSourceModFolder(key));
        }

        public string GetSourceModDataDir(string key)
        {
            return Config[key].DataDir;
        }

        public bool IsSourceModInstalled(string key)
        {
            return FileSystem.Directory.Exists(GetSourceModDir(key));
        }

        public List<string> GetInstalledSourceMods()
        {
            var installedModFolders = FileSystem.Directory.GetDirectories(SourceModInstallPath).Select(folder => FileSystem.Path.GetFileNameWithoutExtension(folder));
            return Config.Where(a => installedModFolders.Contains(a.Value.SourceModFolder)).Select(kv => kv.Key).ToList();
        }

        protected override void PostLoadConfig()
        {
            SetupSupportedSourceMods();
        }

        void SetupSupportedSourceMods()
        {
            SupportedSourceModsKeys = Config.Keys.ToList();
            SupportedSourceModsKeys.Sort();
        }
    }
}