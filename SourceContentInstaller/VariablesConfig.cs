using Pipelines;
using System.IO.Abstractions;

namespace SourceContentInstaller
{
    public class JSONVariablesConfig : SortedDictionary<string, string>
    {
    }

    public class VariablesConfig : ConfigurationManager<JSONVariablesConfig>
    {
        public VariablesConfig(IFileSystem fileSystem, IWriter writer, string filePath, IConfigurationSerializer<JSONVariablesConfig> configSerializer) : base(fileSystem, writer, filePath, configSerializer)
        {
        }

        public virtual void SaveVariable(string name, string value)
        {
            Config[name] = value;
            SaveConfig();
        }
    }
}