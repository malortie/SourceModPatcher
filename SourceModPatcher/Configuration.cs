using System.Collections.Generic;
using System.Collections.ObjectModel;
using test_installsourcecontent;

namespace test_installsourcecontent_modpatcher
{
    public interface IConfiguration
    {
        ReadOnlyDictionary<string, string> GetVariables();
        string GetVariablesFileName();
        ReadOnlyDictionary<string, string> GetInstallVariables();
        string GetSourceModName(string key);
        string GetSourceModFolder(string key);
        string GetSourceModDir(string key);
        string GetSourceModDataDir(string key);
    }

    public class Configuration : IConfiguration
    {
        SourceModsConfig _sourceModsConfig;
        InstallVariablesConfig _installVariablesConfig;
        VariablesConfig _variablesConfig;

        public Configuration(SourceModsConfig sourceModsConfig, InstallVariablesConfig installVariablesConfig, VariablesConfig variablesConfig)
        {
            _sourceModsConfig = sourceModsConfig;
            _installVariablesConfig = installVariablesConfig;
            _variablesConfig = variablesConfig;
        }

        public ReadOnlyDictionary<string, string> GetVariables()
        {
            return new ReadOnlyDictionary<string, string>(_variablesConfig.Config);
        }

        public string GetVariablesFileName()
        {
            return _variablesConfig.GetFileName();
        }

        public ReadOnlyDictionary<string, string> GetInstallVariables()
        {
            return new ReadOnlyDictionary<string, string>(_installVariablesConfig.Config);
        }

        public string GetSourceModName(string key)
        {
            return _sourceModsConfig.GetSourceModName(key);
        }

        public string GetSourceModFolder(string key)
        {
            return _sourceModsConfig.GetSourceModFolder(key);
        }

        public string GetSourceModDir(string key)
        {
            return _sourceModsConfig.GetSourceModDir(key);
        }

        public string GetSourceModDataDir(string key)
        {
            return _sourceModsConfig.GetSourceModDataDir(key);
        }
    }
}