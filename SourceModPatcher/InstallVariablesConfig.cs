using Pipelines;
using SourceContentInstaller;
using System.IO.Abstractions;

namespace SourceModPatcher
{
    public class JSONInstallVariablesConfig : Dictionary<string, string>
    {
    }

    public class InstallVariablesConfig : ConfigurationManager<JSONInstallVariablesConfig>
    {
        public InstallVariablesConfig(IFileSystem fileSystem, IWriter writer, string filePath, IConfigurationSerializer<JSONInstallVariablesConfig> configSerializer) : base(fileSystem, writer, filePath, configSerializer)
        {
        }
    }
}
