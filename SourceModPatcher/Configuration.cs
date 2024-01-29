using SourceContentInstaller;
using System.Collections.ObjectModel;

namespace SourceModPatcher
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

    public class Configuration(SourceModsConfig sourceModsConfig, InstallVariablesConfig installVariablesConfig, VariablesConfig variablesConfig) : IConfiguration
    {
        readonly SourceModsConfig _sourceModsConfig = sourceModsConfig;
        readonly InstallVariablesConfig _installVariablesConfig = installVariablesConfig;
        readonly VariablesConfig _variablesConfig = variablesConfig;

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