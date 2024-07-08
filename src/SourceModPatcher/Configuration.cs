using SourceContentInstaller;
using System.Collections.ObjectModel;

namespace SourceModPatcher
{
    public interface IConfiguration
    {
        ReadOnlyDictionary<string, string> GetVariables();
        string GetVariablesFileName();
        string GetSourceModName(string sourcemodID);
        string GetSourceModFolder(string sourcemodID);
        string GetSourceModDir(string sourcemodID);
        string GetSourceModDataDir(string sourcemodID);
        List<List<string>> GetRequiredContentDependencies(string sourcemodID);
        List<List<string>> GetOptionalContentDependencies(string sourcemodID);
        string GetContentName(string contentID);
        List<string> GetContentOutputVariables(string contentID);
    }

    public class Configuration(SourceModsConfig sourceModsConfig, ContentsConfig contentsConfig, VariablesConfig variablesConfig) : IConfiguration
    {
        readonly SourceModsConfig _sourceModsConfig = sourceModsConfig;
        readonly ContentsConfig _contentsConfig = contentsConfig;
        readonly VariablesConfig _variablesConfig = variablesConfig;

        public ReadOnlyDictionary<string, string> GetVariables()
        {
            return new ReadOnlyDictionary<string, string>(_variablesConfig.Config);
        }

        public string GetVariablesFileName()
        {
            return _variablesConfig.GetFileName();
        }

        public string GetSourceModName(string sourcemodID)
        {
            return _sourceModsConfig.GetSourceModName(sourcemodID);
        }

        public string GetSourceModFolder(string sourcemodID)
        {
            return _sourceModsConfig.GetSourceModFolder(sourcemodID);
        }

        public string GetSourceModDir(string sourcemodID)
        {
            return _sourceModsConfig.GetSourceModDir(sourcemodID);
        }

        public string GetSourceModDataDir(string sourcemodID)
        {
            return _sourceModsConfig.GetSourceModDataDir(sourcemodID);
        }

        public List<List<string>> GetRequiredContentDependencies(string sourcemodID)
        {
            return _sourceModsConfig.GetRequiredContentDependencies(sourcemodID);
        }

        public List<List<string>> GetOptionalContentDependencies(string sourcemodID)
        {
            return _sourceModsConfig.GetOptionalContentDependencies(sourcemodID);
        }
        public string GetContentName(string contentID)
        {
            return _contentsConfig.GetContentName(contentID);
        }

        public List<string> GetContentOutputVariables(string contentID)
        {
            return _contentsConfig.GetContentOutputVariables(contentID);
        }
    }
}