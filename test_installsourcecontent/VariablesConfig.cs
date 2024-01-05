using System.IO.Abstractions;
using Pipelines;
using test_installsourcecontent;

namespace test_installsourcecontent
{
    public class JSONVariablesConfig : SortedDictionary<string, string>
    {
    }

    public class VariablesConfig : ConfigurationManager<JSONVariablesConfig>
    {
        public VariablesConfig(IFileSystem fileSystem, IWriter writer, string filePath, IConfigurationSerializer<JSONVariablesConfig> configSerializer) : base(fileSystem, writer, filePath, configSerializer)
        {
        }

        public void SaveVariable(string name, string value)
        {
            Config[name] = value;
            SaveConfig();
        }
    }
}