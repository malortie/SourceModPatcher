using Pipelines;
using SourceContentInstaller;
using System.IO.Abstractions;

namespace SourceModPatcher
{
    public class JSONInstallVariablesConfig : Dictionary<string, string>
    {
    }

    public class InstallVariablesConfig(IFileSystem fileSystem, IWriter writer, string filePath, IConfigurationSerializer<JSONInstallVariablesConfig> configSerializer) : ConfigurationManager<JSONInstallVariablesConfig>(fileSystem, writer, filePath, configSerializer)
    {
    }
}
