using System.Collections.ObjectModel;

namespace SourceModPatcher.Tests
{
    public class NullConfiguration : IConfiguration
    {
        public ReadOnlyDictionary<string, string> GetInstallVariables()
        {
            return new ReadOnlyDictionary<string, string>(new Dictionary<string, string>());
        }

        public string GetSourceModDataDir(string key)
        {
            return string.Empty;
        }

        public string GetSourceModDir(string key)
        {
            return string.Empty;
        }

        public string GetSourceModFolder(string key)
        {
            return string.Empty;
        }

        public string GetSourceModName(string key)
        {
            return string.Empty;
        }

        public ReadOnlyDictionary<string, string> GetVariables()
        {
            return new ReadOnlyDictionary<string, string>(new Dictionary<string, string>());
        }

        public string GetVariablesFileName()
        {
            return string.Empty;
        }

        public bool HasVariable(string variableName)
        {
            return false;
        }
    }
}