using Pipelines;
using System.IO.Abstractions;
using System.Text.Json.Serialization;

namespace SourceContentInstaller
{
    public class JSONContentsConfigEntry
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("steamapps_dependencies")]
        public List<int> SteamAppsDependencies { get; set; } = [];
        [JsonPropertyName("output_variables")]
        public List<string> OutputVariables { get; set; } = [];
    }

    public class JSONContentsConfig : SortedDictionary<string, JSONContentsConfigEntry>
    {
    }

    public class ContentsConfig(IFileSystem fileSystem, IWriter writer, string filePath, IConfigurationSerializer<JSONContentsConfig> configSerializer) : ConfigurationManager<JSONContentsConfig>(fileSystem, writer, filePath, configSerializer)
    {
        public List<string> SupportedContentIDs = [];

        public virtual string GetContentName(string contentID)
        {
            return Config[contentID].Name;
        }

        public virtual List<int> GetContentSteamAppsDependencies(string contentID)
        {
            return Config[contentID].SteamAppsDependencies;
        }

        public virtual List<string> GetContentOutputVariables(string contentID)
        {
            return Config[contentID].OutputVariables;
        }

        protected override void PostLoadConfig()
        {
            SetupSupportedContents();
        }

        void SetupSupportedContents()
        {
            SupportedContentIDs = [.. Config.Keys];
            SupportedContentIDs.Sort();
        }
    }
}