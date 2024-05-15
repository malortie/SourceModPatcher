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

        public virtual string GetFileName() { return _fileSystem.Path.GetFileName(_filePath); }

        public ConfigT Config { get; protected set; } = new();

        protected IFileSystem FileSystem { get { return _fileSystem; } }
        protected IWriter Writer { get { return _writer; } }

        public void LoadConfig()
        {
            Writer.Info($"Reading {_fileSystem.Path.GetFileName(_filePath)}");
            var deserializedData = _configSerializer.Deserialize(_fileSystem.File.ReadAllText(_filePath));
            if (null == deserializedData)
                throw new Exception($"Failed to deserialize {_filePath}");
            Config = deserializedData;
            PostLoadConfig();
        }

        public void SaveConfig()
        {
            var serializedConfig = _configSerializer.Serialize(Config);
            _fileSystem.File.WriteAllText(_filePath, serializedConfig);
        }

        protected virtual void PostLoadConfig()
        {
        }
    }
}