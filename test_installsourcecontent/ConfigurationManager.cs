using System.IO.Abstractions;

namespace test_installsourcecontent
{
    public interface IConfigurationManager
    {
        void LoadConfig();
        void SaveConfig();
    }

    public abstract class ConfigurationManager<ConfigT> : IConfigurationManager where ConfigT : new()
    {
        readonly IFileSystem _fileSystem;
        readonly ILogger _logger;
        readonly string _filePath;
        readonly IConfigurationSerializer<ConfigT> _configSerializer;

        public ConfigurationManager(IFileSystem fileSystem, ILogger logger, string filePath, IConfigurationSerializer<ConfigT> configSerializer)
        {
            _fileSystem = fileSystem;
            _logger = logger;
            _filePath = filePath;
            _configSerializer = configSerializer;
        }

        public string FileName { get { return _fileSystem.Path.GetFileName(_filePath); } }

        public ConfigT Config { get; private set; } = new();

        protected IFileSystem FileSystem { get { return _fileSystem; } }
        protected ILogger Logger { get { return _logger; } }

        public void LoadConfig() 
        {
            _logger.LogInfo($"Reading {_fileSystem.Path.GetFileName(_filePath)}");
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