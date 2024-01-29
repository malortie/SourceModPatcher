using Pipelines;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using SourceContentInstaller;
using Configuration = SourceModPatcher.Configuration;

namespace SourceModPatcher.Tests
{
    public class NullConfigurationSerializer<ConfigT> : IConfigurationSerializer<ConfigT>
    {
        public ConfigT? Deserialize(string value)
        {
            return default(ConfigT);
        }

        public string Serialize(ConfigT value)
        {
            return string.Empty;
        }
    }

    public class SourceModsConfigMock : SourceModsConfig
    {
        public SourceModsConfigMock(IFileSystem fileSystem, IWriter writer, string filePath, IConfigurationSerializer<JSONSourceModsConfig> configSerializer, string sourceModInstallPath) : base(fileSystem, writer, filePath, configSerializer, sourceModInstallPath)
        {
        }

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

    public class InstallVariablesConfigMock : InstallVariablesConfig
    {
        public InstallVariablesConfigMock(IFileSystem fileSystem, IWriter writer, string filePath, IConfigurationSerializer<JSONInstallVariablesConfig> configSerializer) : base(fileSystem, writer, filePath, configSerializer)
        {
        }
        public JSONInstallVariablesConfig ConfigSetter { set { Config = value; } }
    }

    public class VariablesConfigMock : VariablesConfig
    {
        public VariablesConfigMock(IFileSystem fileSystem, IWriter writer, string filePath, IConfigurationSerializer<JSONVariablesConfig> configSerializer) : base(fileSystem, writer, filePath, configSerializer)
        {
        }

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
        static IWriter NullWriter = new NullWriter();
        static IConfigurationSerializer<JSONSourceModsConfig> NullSourceModsConfigSerializer = new NullConfigurationSerializer<JSONSourceModsConfig>();
        static IConfigurationSerializer<JSONInstallVariablesConfig> NullInstallVariablesConfigSerializer = new NullConfigurationSerializer<JSONInstallVariablesConfig>();
        static IConfigurationSerializer<JSONVariablesConfig> NullVariablesConfigSerializer = new NullConfigurationSerializer<JSONVariablesConfig>();

        [TestMethod]
        public void GetVariables_Returns_VariablesConfig_Variables()
        {
            var fileSystem = new MockFileSystem();
            var sourceModsConfig = new SourceModsConfigMock(fileSystem, NullWriter, string.Empty, NullSourceModsConfigSerializer, string.Empty);
            var installVariablesConfig = new InstallVariablesConfigMock(fileSystem, NullWriter, string.Empty, NullInstallVariablesConfigSerializer);
            var variablesConfig = new VariablesConfigMock(fileSystem, NullWriter, string.Empty, NullVariablesConfigSerializer);

            variablesConfig.ConfigSetter = new JSONVariablesConfig
            {
                { "hl2_content_path", "C:/Half-Life 2" },
                { "sdkbase2006_content_path", "C:/Source SDK Base" },
                { "sdkbase2007_content_path", "C:/Source SDK Base 2007" }
            };

            var configuration = new Configuration(sourceModsConfig, installVariablesConfig, variablesConfig);
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
            var installVariablesConfig = new InstallVariablesConfigMock(fileSystem, NullWriter, string.Empty, NullInstallVariablesConfigSerializer);
            var variablesConfig = new VariablesConfigMock(fileSystem, NullWriter, string.Empty, NullVariablesConfigSerializer);

            var configuration = new Configuration(sourceModsConfig, installVariablesConfig, variablesConfig);
            configuration.GetVariablesFileName();

            Assert.AreEqual(1, variablesConfig.FileNameTotal);
        }

        [TestMethod]
        public void GetInstallVariables_Returns_InstallVariablesConfig_Variables()
        {
            var fileSystem = new MockFileSystem();
            var sourceModsConfig = new SourceModsConfigMock(fileSystem, NullWriter, string.Empty, NullSourceModsConfigSerializer, string.Empty);
            var installVariablesConfig = new InstallVariablesConfigMock(fileSystem, NullWriter, string.Empty, NullInstallVariablesConfigSerializer);
            var variablesConfig = new VariablesConfigMock(fileSystem, NullWriter, string.Empty, NullVariablesConfigSerializer);

            installVariablesConfig.ConfigSetter = new JSONInstallVariablesConfig
            {
                { "data_dir", "C:/data" },
                { "data_mods_dir", "C:/data/mods" },
                { "data_sdks_dir", "C:/data/sdks" }
            };

            var configuration = new Configuration(sourceModsConfig, installVariablesConfig, variablesConfig);
            var variables = configuration.GetInstallVariables();

            Assert.AreEqual(3, variables.Keys.Count);
            Assert.IsTrue(variables.ContainsKey("data_dir"));
            Assert.IsTrue(variables.ContainsKey("data_mods_dir"));
            Assert.IsTrue(variables.ContainsKey("data_sdks_dir"));
            Assert.AreEqual("C:/data", variables["data_dir"]);
            Assert.AreEqual("C:/data/mods", variables["data_mods_dir"]);
            Assert.AreEqual("C:/data/sdks", variables["data_sdks_dir"]);
        }

        [TestMethod]
        public void Call_SourceModsConfig_GetSourceModName()
        {
            var fileSystem = new MockFileSystem();
            var sourceModsConfig = new SourceModsConfigMock(fileSystem, NullWriter, string.Empty, NullSourceModsConfigSerializer, string.Empty);
            var installVariablesConfig = new InstallVariablesConfigMock(fileSystem, NullWriter, string.Empty, NullInstallVariablesConfigSerializer);
            var variablesConfig = new VariablesConfigMock(fileSystem, NullWriter, string.Empty, NullVariablesConfigSerializer);

            var configuration = new Configuration(sourceModsConfig, installVariablesConfig, variablesConfig);
            configuration.GetSourceModName(string.Empty);

            Assert.AreEqual(1, sourceModsConfig.GetSourceModNameTotal);
        }

        [TestMethod]
        public void Call_SourceModsConfig_GetSourceModFolder()
        {
            var fileSystem = new MockFileSystem();
            var sourceModsConfig = new SourceModsConfigMock(fileSystem, NullWriter, string.Empty, NullSourceModsConfigSerializer, string.Empty);
            var installVariablesConfig = new InstallVariablesConfigMock(fileSystem, NullWriter, string.Empty, NullInstallVariablesConfigSerializer);
            var variablesConfig = new VariablesConfigMock(fileSystem, NullWriter, string.Empty, NullVariablesConfigSerializer);

            var configuration = new Configuration(sourceModsConfig, installVariablesConfig, variablesConfig);
            configuration.GetSourceModFolder(string.Empty);

            Assert.AreEqual(1, sourceModsConfig.GetSourceModFolderTotal);
        }

        [TestMethod]
        public void Call_SourceModsConfig_GetSourceModDir()
        {
            var fileSystem = new MockFileSystem();
            var sourceModsConfig = new SourceModsConfigMock(fileSystem, NullWriter, string.Empty, NullSourceModsConfigSerializer, string.Empty);
            var installVariablesConfig = new InstallVariablesConfigMock(fileSystem, NullWriter, string.Empty, NullInstallVariablesConfigSerializer);
            var variablesConfig = new VariablesConfigMock(fileSystem, NullWriter, string.Empty, NullVariablesConfigSerializer);

            var configuration = new Configuration(sourceModsConfig, installVariablesConfig, variablesConfig);
            configuration.GetSourceModDir(string.Empty);

            Assert.AreEqual(1, sourceModsConfig.GetSourceModDirTotal);
        }

        [TestMethod]
        public void Call_SourceModsConfig_GetSourceModDataDir()
        {
            var fileSystem = new MockFileSystem();
            var sourceModsConfig = new SourceModsConfigMock(fileSystem, NullWriter, string.Empty, NullSourceModsConfigSerializer, string.Empty);
            var installVariablesConfig = new InstallVariablesConfigMock(fileSystem, NullWriter, string.Empty, NullInstallVariablesConfigSerializer);
            var variablesConfig = new VariablesConfigMock(fileSystem, NullWriter, string.Empty, NullVariablesConfigSerializer);

            var configuration = new Configuration(sourceModsConfig, installVariablesConfig, variablesConfig);
            configuration.GetSourceModDataDir(string.Empty);

            Assert.AreEqual(1, sourceModsConfig.GetSourceModDataDirTotal);
        }
    }
}