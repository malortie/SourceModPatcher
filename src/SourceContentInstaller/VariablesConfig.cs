using Pipelines;
using System.IO.Abstractions;

namespace SourceContentInstaller
{
    public class JSONVariablesConfig : SortedDictionary<string, string>
    {
    }

    public class VariablesConfig(IFileSystem fileSystem, IWriter writer, string filePath, IConfigurationSerializer<JSONVariablesConfig> configSerializer) : ConfigurationManager<JSONVariablesConfig>(fileSystem, writer, filePath, configSerializer)
    {
        public virtual void SaveVariable(string name, string value)
        {
            Config[name] = value;
            SaveConfig();
        }
    }
}