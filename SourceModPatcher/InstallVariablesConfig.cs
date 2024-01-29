using System.Collections.ObjectModel;
using System.IO.Abstractions;
using Pipelines;
using test_installsourcecontent;

namespace test_installsourcecontent_modpatcher
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
