using System.IO.Abstractions;
using Pipelines;
using SourceContentInstaller;

namespace SourceModPatcher
{
    public class JSONCommonConfig : Dictionary<string, string>
    {
    }

    public sealed class CommonConfig : ConfigurationManager<JSONCommonConfig>
    {
        public CommonConfig(IFileSystem fileSystem, IWriter writer, string filePath, IConfigurationSerializer<JSONCommonConfig> configSerializer) : base(fileSystem, writer, filePath, configSerializer)
        {
        }

        public string GetSourceModsPath()
        {
            return Config["sourcemods_path"];
        }
    }
}