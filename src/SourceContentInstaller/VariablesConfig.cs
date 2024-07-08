using Pipelines;
using System.IO.Abstractions;

namespace SourceContentInstaller
{
    public class JSONVariablesConfig : SortedDictionary<string, string>
    {
    }

    public class VariablesConfig(IFileSystem fileSystem, IWriter writer, string filePath, IConfigurationSerializer<JSONVariablesConfig> configSerializer) : ConfigurationManager<JSONVariablesConfig>(fileSystem, writer, filePath, configSerializer)
    {
        public bool PurgeVariablesList { get; set; } = true;

        public virtual bool IsVariablePresentAndValid(string variableName)
        {
            return Config.ContainsKey(variableName) &&
                !string.IsNullOrEmpty(Config[variableName]) &&
                FileSystem.Path.Exists(Config[variableName]);
        }

        protected override void PreLoadConfig()
        {
            CreateFileIfItDoesNotExist();
        }

        protected override void PostLoadConfig()
        {
            if (PurgeVariablesList)
                DoPurgeVariablesList();
        }

        void CreateFileIfItDoesNotExist()
        {
            if (!FileSystem.File.Exists(FilePath))
                FileSystem.File.WriteAllText(FilePath, "{\n}\n");
        }

        void DoPurgeVariablesList()
        {
            // Remove any inexistant paths.
            HashSet<string> variablesToRemove = [];

            foreach (var kv in Config)
            {
                if (!FileSystem.Path.Exists(kv.Value))
                    variablesToRemove.Add(kv.Key);
            }

            foreach(var @var in variablesToRemove)
                Config.Remove(@var);
        }

        public virtual void SaveVariable(string name, string value)
        {
            Config[name] = value;
            SaveConfig();
        }

        public virtual void SaveVariables(Dictionary<string, string> variables)
        {
            foreach (var kv in variables)
                Config[kv.Key] = kv.Value;
            SaveConfig();
        }
    }
}