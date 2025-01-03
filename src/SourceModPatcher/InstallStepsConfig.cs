using Pipelines;
using SourceContentInstaller;
using System.IO.Abstractions;
using System.Text.Json.Serialization;

namespace SourceModPatcher
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    [JsonDerivedType(typeof(JSONCopyFilesInstallStep), typeDiscriminator: "copy_files")]
    [JsonDerivedType(typeof(JSONCopyDirectoryInstallStep), typeDiscriminator: "copy_directory")]
    [JsonDerivedType(typeof(JSONReplaceTokensInstallStep), typeDiscriminator: "replace_tokens")]
    [JsonDerivedType(typeof(JSONValidateVariablesDependenciesInstallStep), typeDiscriminator: "validate_variables_dependencies")]
    public class JSONInstallStep
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [JsonPropertyName("depends_on")]
        public List<string>? DependsOn { get; set; }
    }

    public class JSONCopyFilesInstallStepFile : JSONInstallStep
    {
        [JsonPropertyName("src")]
        public string? Source { get; set; }
        [JsonPropertyName("dest")]
        public string? Destination { get; set; }
    }

    public class JSONCopyFilesInstallStep : JSONInstallStep
    {
        [JsonPropertyName("files")]
        public List<JSONCopyFilesInstallStepFile>? Files { get; set; }
    }

    public class JSONCopyDirectoryInstallStep : JSONInstallStep
    {
        [JsonPropertyName("src")]
        public string? Source { get; set; }
        [JsonPropertyName("dest")]
        public string? Destination { get; set; }
    }

    public class JSONReplaceTokensInstallStep : JSONInstallStep
    {
        [JsonPropertyName("files")]
        public List<string>? Files { get; set; }
    }

    public class JSONValidateVariablesDependenciesInstallStep : JSONInstallStep
    {
        [JsonPropertyName("dependencies")]
        public List<List<string>>? Dependencies { get; set; }
    }

    public class JSONInstallStepsConfig : Dictionary<string, string>
    {
    }

    public sealed class InstallStepsConfig(IFileSystem fileSystem, IWriter writer, string filePath, IConfigurationSerializer<JSONInstallStepsConfig> configSerializer) : ConfigurationManager<JSONInstallStepsConfig>(fileSystem, writer, filePath, configSerializer)
    {
    }
}