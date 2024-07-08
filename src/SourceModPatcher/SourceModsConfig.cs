using Pipelines;
using SourceContentInstaller;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Text.Json.Serialization;

namespace SourceModPatcher
{
    public class JSONSourceModsConfigDependencies : JSONInstallStep
    {
        [JsonPropertyName("required")]
        public List<List<string>> Required { get; set; } = [];
        [JsonPropertyName("optional")]
        public List<List<string>>? Optional { get; set; } = [];
    }

    public class JSONSourceModsConfigEntry
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("sourcemod_folder")]
        public string SourceModFolder { get; set; } = string.Empty;
        [JsonPropertyName("data_dir")]
        public string DataDir { get; set; } = string.Empty;
        [JsonPropertyName("dependencies")]
        public JSONSourceModsConfigDependencies Dependencies { get; set; } = new ();
    }

    public class JSONSourceModsConfig : SortedDictionary<string, JSONSourceModsConfigEntry>
    {
    }

    public class SourceModsConfig(IFileSystem fileSystem, IWriter writer, string filePath, IConfigurationSerializer<JSONSourceModsConfig> configSerializer, string sourceModInstallPath) : ConfigurationManager<JSONSourceModsConfig>(fileSystem, writer, filePath, configSerializer)
    {
        public List<string> SupportedSourceModsKeys = [];

        public string SourceModInstallPath { get; private set; } = sourceModInstallPath;

        public virtual string GetSourceModName(string sourcemodID)
        {
            return Config[sourcemodID].Name;
        }

        public virtual string GetSourceModFolder(string sourcemodID)
        {
            return Config[sourcemodID].SourceModFolder;
        }

        public virtual string GetSourceModDir(string sourcemodID)
        {
            return PathExtensions.JoinWithSeparator(FileSystem, SourceModInstallPath, GetSourceModFolder(sourcemodID));
        }

        public virtual string GetSourceModDataDir(string sourcemodID)
        {
            return Config[sourcemodID].DataDir;
        }

        public bool IsSourceModInstalled(string sourcemodID)
        {
            return FileSystem.Directory.Exists(GetSourceModDir(sourcemodID));
        }

        public List<string> GetInstalledSourceMods()
        {
            var installedModFolders = FileSystem.Directory.GetDirectories(SourceModInstallPath).Select(folder => FileSystem.Path.GetFileNameWithoutExtension(folder));
            return Config.Where(a => installedModFolders.Contains(a.Value.SourceModFolder)).Select(kv => kv.Key).ToList();
        }

        public List<List<string>> GetRequiredContentDependencies(string sourcemodID)
        {
            return Config[sourcemodID].Dependencies.Required;
        }

        public List<List<string>> GetOptionalContentDependencies(string sourcemodID)
        {
            return Config[sourcemodID].Dependencies.Optional ?? [];
        }

        protected override void PostLoadConfig()
        {
            SetupSupportedSourceMods();
        }

        void SetupSupportedSourceMods()
        {
            SupportedSourceModsKeys = [.. Config.Keys];
            SupportedSourceModsKeys.Sort();
        }
    }
}