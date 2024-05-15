using Pipelines;
using SourceContentInstaller;
using System.IO.Abstractions;

namespace SourceModPatcher
{
    public class JSONCommonConfig : Dictionary<string, string>
    {
    }

    public sealed class CommonConfig(IFileSystem fileSystem, IWriter writer, string filePath, IConfigurationSerializer<JSONCommonConfig> configSerializer) : ConfigurationManager<JSONCommonConfig>(fileSystem, writer, filePath, configSerializer)
    {
        public string GetSourceModsPath()
        {
            return Config["sourcemods_path"];
        }
    }
}