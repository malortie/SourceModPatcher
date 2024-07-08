using Pipelines;
using System.IO.Abstractions;

namespace SourceContentInstaller
{
    public interface IConfigurationManager
    {
        void LoadConfig();
        void SaveConfig();
    }

    public class ConfigurationManager<ConfigT>(IFileSystem fileSystem, IWriter writer, string filePath, IConfigurationSerializer<ConfigT> configSerializer) : IConfigurationManager where ConfigT : new()
    {
        readonly IFileSystem _fileSystem = fileSystem;
        readonly IWriter _writer = writer;
        readonly string _filePath = filePath;
        readonly IConfigurationSerializer<ConfigT> _configSerializer = configSerializer;

        public virtual string GetFileName() { return _fileSystem.Path.GetFileName(FilePath); }
        public string FilePath => filePath;

        public ConfigT Config { get; protected set; } = new();

        protected IFileSystem FileSystem { get { return _fileSystem; } }
        protected IWriter Writer { get { return _writer; } }

        public void LoadConfig()
        {
            PreLoadConfig();
            Writer.Info($"Reading {_fileSystem.Path.GetFileName(FilePath)}");
            var deserializedData = _configSerializer.Deserialize(_fileSystem.File.ReadAllText(FilePath));
            if (null == deserializedData)
                throw new Exception($"Failed to deserialize {FilePath}");
            Config = deserializedData;
            PostLoadConfig();
        }

        public void SaveConfig()
        {
            var serializedConfig = _configSerializer.Serialize(Config);
            _fileSystem.File.WriteAllText(FilePath, serializedConfig);
        }

        protected virtual void PreLoadConfig()
        {
        }

        protected virtual void PostLoadConfig()
        {
        }
    }
}