using Pipelines;
using SourceContentInstaller;
using System.IO.Abstractions;

namespace SourceContentInstaller
{
    public class JSONInstallVariablesConfig : Dictionary<string, string>
    {
    }

    public class InstallVariablesConfig(IFileSystem fileSystem, IWriter writer, string filePath, IConfigurationSerializer<JSONInstallVariablesConfig> configSerializer) : ConfigurationManager<JSONInstallVariablesConfig>(fileSystem, writer, filePath, configSerializer)
    {
        public Dictionary<string, string> InstallVariables = new();

        protected override void PostLoadConfig()
        {
            SetupInstallVariables();
        }

        void SetupInstallVariables()
        {
            // Convert relative path to absolute path in install variables.
            foreach (var kv in Config)
                InstallVariables[kv.Key] = PathExtensions.ConvertToUnixDirectorySeparator(FileSystem, FileSystem.Path.GetFullPath(kv.Value));
        }
    }
}
