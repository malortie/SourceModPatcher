using System.IO.Abstractions;

namespace test_installsourcecontent
{
    public class JSONVariablesConfig : SortedDictionary<string, string>
    {
    }

    public class VariablesConfig : ConfigurationManager<JSONVariablesConfig>
    {
        public VariablesConfig(IFileSystem fileSystem, ILogger logger, string filePath,IConfigurationSerializer<JSONVariablesConfig> configSerializer) : base(fileSystem, logger, filePath, configSerializer)
        {
        }

        public void SaveVariable(string name, string value)
        {
            Config[name] = value;
            SaveConfig();
        }
    }
}