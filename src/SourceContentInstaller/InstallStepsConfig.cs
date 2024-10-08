using Pipelines;
using System.IO.Abstractions;
using System.Text.Json.Serialization;

namespace SourceContentInstaller
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    [JsonDerivedType(typeof(JSONExtractVPKInstallStep), typeDiscriminator: "extract_vpk")]
    [JsonDerivedType(typeof(JSONSaveVariableInstallStep), typeDiscriminator: "save_variable")]
    public class JSONInstallStep
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [JsonPropertyName("depends_on")]
        public List<string>? DependsOn { get; set; }
    }

    public class JSONExtractVPKInstallStepVPK
    {
        [JsonPropertyName("vpk")]
        public string? VPKFile { get; set; }
        [JsonPropertyName("files_to_exclude")]
        public List<string>? FilesToExclude { get; set; }
        [JsonPropertyName("files_to_extract")]
        public List<string>? FilesToExtract { get; set; }
    }

    public class JSONExtractVPKInstallStep : JSONInstallStep
    {
        [JsonPropertyName("vpks")]
        public List<JSONExtractVPKInstallStepVPK>? Vpks { get; set; }
        [JsonPropertyName("files_to_exclude")]
        public List<string>? FilesToExclude { get; set; }
        [JsonPropertyName("files_to_extract")]
        public List<string>? FilesToExtract { get; set; }
        [JsonPropertyName("steamappid")]
        public int? SteamAppID { get; set; }
        [JsonPropertyName("outdir")]
        public string? OutDir { get; set; }
    }

    public class JSONSaveVariableInstallStep : JSONInstallStep
    {
        [JsonPropertyName("variable_name")]
        public string? VariableName { get; set; }
        [JsonPropertyName("variable_value")]
        public string? VariableValue { get; set; }
    }

    public class JSONInstallStepsConfig : Dictionary<string, string>
    {
    }

    public sealed class InstallStepsConfig(IFileSystem fileSystem, IWriter writer, string filePath, IConfigurationSerializer<JSONInstallStepsConfig> configSerializer) : ConfigurationManager<JSONInstallStepsConfig>(fileSystem, writer, filePath, configSerializer)
    {
    }
}