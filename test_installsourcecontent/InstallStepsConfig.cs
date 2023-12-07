using System.IO.Abstractions;
using System.Text.Json.Serialization;

namespace test_installsourcecontent
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

    public class JSONExtractVPKInstallStep : JSONInstallStep
    {
        [JsonPropertyName("vpks")]
        public List<string>? Vpks { get; set; }
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

    public class JSONInstallStepsConfig : Dictionary<int, List<JSONInstallStep>>
    {
    }

    public sealed class InstallStepsConfig : ConfigurationManager<JSONInstallStepsConfig>
    {
        public InstallStepsConfig(IFileSystem fileSystem, IWriter writer, string filePath, IConfigurationSerializer<JSONInstallStepsConfig> configSerializer) : base(fileSystem, writer,filePath, configSerializer)
        {
        }
    }
 }