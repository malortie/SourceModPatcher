using Pipelines;
using SourceContentInstaller;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;

namespace SourceModPatcher.Tests
{
    public class NullConfigurationSerializer<ConfigT> : IConfigurationSerializer<ConfigT>
    {
        public ConfigT? Deserialize(string value)
        {
            return default;
        }

        public string Serialize(ConfigT value)
        {
            return string.Empty;
        }
    }

    public class SourceModsConfigMock(IFileSystem fileSystem, IWriter writer, string filePath, IConfigurationSerializer<JSONSourceModsConfig> configSerializer, string sourceModInstallPath) : SourceModsConfig(fileSystem, writer, filePath, configSerializer, sourceModInstallPath)
    {
        public int GetSourceModNameTotal { get; private set; } = 0;
        public int GetSourceModFolderTotal { get; private set; } = 0;
        public int GetSourceModDirTotal { get; private set; } = 0;
        public int GetSourceModDataDirTotal { get; private set; } = 0;

        public override string GetSourceModName(string key)
        {
            ++GetSourceModNameTotal;
            return string.Empty;
        }

        public override string GetSourceModFolder(string key)
        {
            ++GetSourceModFolderTotal;
            return string.Empty;
        }

        public override string GetSourceModDir(string key)
        {
            ++GetSourceModDirTotal;
            return string.Empty;
        }

        public override string GetSourceModDataDir(string key)
        {
            ++GetSourceModDataDirTotal;
            return string.Empty;
        }
    }

    public class ContentsConfigMock(IFileSystem fileSystem, IWriter writer, string filePath, IConfigurationSerializer<JSONContentsConfig> configSerializer) : ContentsConfig(fileSystem, writer, filePath, configSerializer)
    {
        public int GetContentNameTotal { get; private set; } = 0;
        public int GetContentSteamAppsDependenciesTotal { get; private set; } = 0;
        public int GetContentOutputVariablesTotal { get; private set; } = 0;

        public override string GetContentName(string contentID)
        {
            ++GetContentNameTotal;
            return string.Empty;
        }

        public override List<int> GetContentSteamAppsDependencies(string contentID)
        {
            ++GetContentSteamAppsDependenciesTotal;
            return [];
        }

        public override List<string> GetContentOutputVariables(string contentID)
        {
            ++GetContentOutputVariablesTotal;
            return [];
        }
    }

    public class VariablesConfigMock(IFileSystem fileSystem, IWriter writer, string filePath, IConfigurationSerializer<JSONVariablesConfig> configSerializer) : VariablesConfig(fileSystem, writer, filePath, configSerializer)
    {
        public JSONVariablesConfig ConfigSetter { set { Config = value; } }

        public int FileNameTotal { get; private set; } = 0;

        public override string GetFileName()
        {
            ++FileNameTotal;
            return string.Empty;
        }
    }


    [TestClass]
    public class TestConfiguration
    {
        static readonly IWriter NullWriter = new NullWriter();
        static readonly IConfigurationSerializer<JSONSourceModsConfig> NullSourceModsConfigSerializer = new NullConfigurationSerializer<JSONSourceModsConfig>();
        static readonly IConfigurationSerializer<JSONContentsConfig> NullContentsConfigSerializer = new NullConfigurationSerializer<JSONContentsConfig>();
        static readonly IConfigurationSerializer<JSONInstallVariablesConfig> NullInstallVariablesConfigSerializer = new NullConfigurationSerializer<JSONInstallVariablesConfig>();
        static readonly IConfigurationSerializer<JSONVariablesConfig> NullVariablesConfigSerializer = new NullConfigurationSerializer<JSONVariablesConfig>();

        [TestMethod]
        public void GetVariables_Returns_VariablesConfig_Variables()
        {
            var fileSystem = new MockFileSystem();
            var sourceModsConfig = new SourceModsConfigMock(fileSystem, NullWriter, string.Empty, NullSourceModsConfigSerializer, string.Empty);
            var contentsConfig = new ContentsConfigMock(fileSystem, NullWriter, string.Empty, NullContentsConfigSerializer);
            var variablesConfig = new VariablesConfigMock(fileSystem, NullWriter, string.Empty, NullVariablesConfigSerializer)
            {
                ConfigSetter = new JSONVariablesConfig
            {
                { "hl2_content_path", "C:/Half-Life 2" },
                { "sdkbase2006_content_path", "C:/Source SDK Base" },
                { "sdkbase2007_content_path", "C:/Source SDK Base 2007" }
            }
            };

            var configuration = new Configuration(sourceModsConfig, contentsConfig, variablesConfig);
            var variables = configuration.GetVariables();

            Assert.AreEqual(3, variables.Keys.Count);
            Assert.IsTrue(variables.ContainsKey("hl2_content_path"));
            Assert.IsTrue(variables.ContainsKey("sdkbase2006_content_path"));
            Assert.IsTrue(variables.ContainsKey("sdkbase2007_content_path"));
            Assert.AreEqual("C:/Half-Life 2", variables["hl2_content_path"]);
            Assert.AreEqual("C:/Source SDK Base", variables["sdkbase2006_content_path"]);
            Assert.AreEqual("C:/Source SDK Base 2007", variables["sdkbase2007_content_path"]);
        }

        [TestMethod]
        public void Call_VariablesConfig_GetFileName()
        {
            var fileSystem = new MockFileSystem();
            var sourceModsConfig = new SourceModsConfigMock(fileSystem, NullWriter, string.Empty, NullSourceModsConfigSerializer, string.Empty);
            var contentsConfig = new ContentsConfigMock(fileSystem, NullWriter, string.Empty, NullContentsConfigSerializer);
            var variablesConfig = new VariablesConfigMock(fileSystem, NullWriter, string.Empty, NullVariablesConfigSerializer);

            var configuration = new Configuration(sourceModsConfig, contentsConfig, variablesConfig);
            configuration.GetVariablesFileName();

            Assert.AreEqual(1, variablesConfig.FileNameTotal);
        }

        [TestMethod]
        public void Call_SourceModsConfig_GetSourceModName()
        {
            var fileSystem = new MockFileSystem();
            var sourceModsConfig = new SourceModsConfigMock(fileSystem, NullWriter, string.Empty, NullSourceModsConfigSerializer, string.Empty);
            var contentsConfig = new ContentsConfigMock(fileSystem, NullWriter, string.Empty, NullContentsConfigSerializer);
            var variablesConfig = new VariablesConfigMock(fileSystem, NullWriter, string.Empty, NullVariablesConfigSerializer);

            var configuration = new Configuration(sourceModsConfig, contentsConfig, variablesConfig);
            configuration.GetSourceModName(string.Empty);

            Assert.AreEqual(1, sourceModsConfig.GetSourceModNameTotal);
        }

        [TestMethod]
        public void Call_SourceModsConfig_GetSourceModFolder()
        {
            var fileSystem = new MockFileSystem();
            var sourceModsConfig = new SourceModsConfigMock(fileSystem, NullWriter, string.Empty, NullSourceModsConfigSerializer, string.Empty);
            var contentsConfig = new ContentsConfigMock(fileSystem, NullWriter, string.Empty, NullContentsConfigSerializer);
            var variablesConfig = new VariablesConfigMock(fileSystem, NullWriter, string.Empty, NullVariablesConfigSerializer);

            var configuration = new Configuration(sourceModsConfig, contentsConfig, variablesConfig);
            configuration.GetSourceModFolder(string.Empty);

            Assert.AreEqual(1, sourceModsConfig.GetSourceModFolderTotal);
        }

        [TestMethod]
        public void Call_SourceModsConfig_GetSourceModDir()
        {
            var fileSystem = new MockFileSystem();
            var sourceModsConfig = new SourceModsConfigMock(fileSystem, NullWriter, string.Empty, NullSourceModsConfigSerializer, string.Empty);
            var contentsConfig = new ContentsConfigMock(fileSystem, NullWriter, string.Empty, NullContentsConfigSerializer);
            var variablesConfig = new VariablesConfigMock(fileSystem, NullWriter, string.Empty, NullVariablesConfigSerializer);

            var configuration = new Configuration(sourceModsConfig, contentsConfig, variablesConfig);
            configuration.GetSourceModDir(string.Empty);

            Assert.AreEqual(1, sourceModsConfig.GetSourceModDirTotal);
        }

        [TestMethod]
        public void Call_SourceModsConfig_GetSourceModDataDir()
        {
            var fileSystem = new MockFileSystem();
            var sourceModsConfig = new SourceModsConfigMock(fileSystem, NullWriter, string.Empty, NullSourceModsConfigSerializer, string.Empty);
            var contentsConfig = new ContentsConfigMock(fileSystem, NullWriter, string.Empty, NullContentsConfigSerializer);
            var variablesConfig = new VariablesConfigMock(fileSystem, NullWriter, string.Empty, NullVariablesConfigSerializer);

            var configuration = new Configuration(sourceModsConfig, contentsConfig, variablesConfig);
            configuration.GetSourceModDataDir(string.Empty);

            Assert.AreEqual(1, sourceModsConfig.GetSourceModDataDirTotal);
        }
    }
}