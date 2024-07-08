using Pipelines;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;

namespace SourceContentInstaller.Tests
{
    public class SteamAppsConfigMock(IFileSystem fileSystem, IWriter writer, string filePath, IConfigurationSerializer<JSONSteamAppsConfig> configSerializer, ISteamPathFinder steamPathFinder) : SteamAppsConfig(fileSystem, writer, filePath, configSerializer, steamPathFinder)
    {
        public int GetSteamAppNameTotal { get; private set; } = 0;
        public int GetSteamAppInstallDirTotal { get; private set; } = 0;

        public override string GetSteamAppName(int appID)
        {
            ++GetSteamAppNameTotal;
            return string.Empty;
        }

        public override string GetSteamAppInstallDir(int appID)
        {
            ++GetSteamAppInstallDirTotal;
            return string.Empty;
        }
    }

    public class ContentsConfigMock(IFileSystem fileSystem, IWriter writer, string filePath, IConfigurationSerializer<JSONContentsConfig> configSerializer) : ContentsConfig(fileSystem, writer, filePath, configSerializer)
    {
        public int GetContentNameTotal { get; private set; } = 0;
        public int GetContentSteamAppsDependenciesTotal { get; private set; } = 0;

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
    }


    public class InstallSettingsMock(IFileSystem fileSystem, IWriter writer, string filePath, IConfigurationSerializer<JSONInstallSettings> configSerializer) : InstallSettings(fileSystem, writer, filePath, configSerializer)
    {
        public int GetContentInstallDirTotal { get; private set; } = 0;

        public override string GetContentInstallDir(string contentID)
        {
            ++GetContentInstallDirTotal;
            return string.Empty;
        }
    }

    public class VariablesConfigMock(IFileSystem fileSystem, IWriter writer, string filePath, IConfigurationSerializer<JSONVariablesConfig> configSerializer) : VariablesConfig(fileSystem, writer, filePath, configSerializer)
    {
        public int FileNameTotal { get; private set; } = 0;
        public int SaveVariableTotal { get; private set; } = 0;

        public override string GetFileName()
        {
            ++FileNameTotal;
            return string.Empty;
        }

        public override void SaveVariable(string name, string value)
        {
            ++SaveVariableTotal;
        }
    }

    [TestClass]
    public class TestConfiguration
    {
        static readonly IWriter NullWriter = new NullWriter();
        static readonly ISteamPathFinder NullSteamPathFinder = new NullSteamPathFinder();
        static readonly IConfigurationSerializer<JSONSteamAppsConfig> NullSteamAppsConfigSerializer = new NullConfigurationSerializer<JSONSteamAppsConfig>();
        static readonly IConfigurationSerializer<JSONContentsConfig> NullContentsConfigSerializer = new NullConfigurationSerializer<JSONContentsConfig>();
        static readonly IConfigurationSerializer<JSONInstallSettings> NullInstallSettingsSerializer = new NullConfigurationSerializer<JSONInstallSettings>();
        static readonly IConfigurationSerializer<JSONVariablesConfig> NullVariablesConfigSerializer = new NullConfigurationSerializer<JSONVariablesConfig>();

        [TestMethod]
        public void Call_SteamAppsConfig_GetSteamAppName()
        {
            var fileSystem = new MockFileSystem();
            var steamAppsConfig = new SteamAppsConfigMock(fileSystem, NullWriter, string.Empty, NullSteamAppsConfigSerializer, NullSteamPathFinder);
            var contentsConfig = new ContentsConfigMock(fileSystem, NullWriter, string.Empty, NullContentsConfigSerializer);
            var installSettingsConfig = new InstallSettingsMock(fileSystem, NullWriter, string.Empty, NullInstallSettingsSerializer);
            var variablesConfig = new VariablesConfigMock(fileSystem, NullWriter, string.Empty, NullVariablesConfigSerializer);

            var configuration = new Configuration(steamAppsConfig, contentsConfig, installSettingsConfig, variablesConfig);
            configuration.GetSteamAppName(0);
            Assert.AreEqual(1, steamAppsConfig.GetSteamAppNameTotal);
        }

        [TestMethod]
        public void Call_SteamAppsConfig_GetSteamAppInstallDir()
        {
            var fileSystem = new MockFileSystem();
            var steamAppsConfig = new SteamAppsConfigMock(fileSystem, NullWriter, string.Empty, NullSteamAppsConfigSerializer, NullSteamPathFinder);
            var contentsConfig = new ContentsConfigMock(fileSystem, NullWriter, string.Empty, NullContentsConfigSerializer);
            var installSettingsConfig = new InstallSettingsMock(fileSystem, NullWriter, string.Empty, NullInstallSettingsSerializer);
            var variablesConfig = new VariablesConfigMock(fileSystem, NullWriter, string.Empty, NullVariablesConfigSerializer);

            var configuration = new Configuration(steamAppsConfig, contentsConfig, installSettingsConfig, variablesConfig);
            configuration.GetSteamAppInstallDir(0);
            Assert.AreEqual(1, steamAppsConfig.GetSteamAppInstallDirTotal);
        }

        [TestMethod]
        public void Call_VariablesConfig_SaveVariable()
        {
            var fileSystem = new MockFileSystem();
            var steamAppsConfig = new SteamAppsConfigMock(fileSystem, NullWriter, string.Empty, NullSteamAppsConfigSerializer, NullSteamPathFinder);
            var contentsConfig = new ContentsConfigMock(fileSystem, NullWriter, string.Empty, NullContentsConfigSerializer);
            var installSettingsConfig = new InstallSettingsMock(fileSystem, NullWriter, string.Empty, NullInstallSettingsSerializer);
            var variablesConfig = new VariablesConfigMock(fileSystem, NullWriter, string.Empty, NullVariablesConfigSerializer);

            var configuration = new Configuration(steamAppsConfig, contentsConfig, installSettingsConfig, variablesConfig);
            configuration.SaveVariable("hl2_content_path", "C:/Half-Life 2");
            Assert.AreEqual(1, variablesConfig.SaveVariableTotal);
        }

        [TestMethod]
        public void Call_InstallSettings_GetContentInstallDir()
        {
            var fileSystem = new MockFileSystem();
            var steamAppsConfig = new SteamAppsConfigMock(fileSystem, NullWriter, string.Empty, NullSteamAppsConfigSerializer, NullSteamPathFinder);
            var contentsConfig = new ContentsConfigMock(fileSystem, NullWriter, string.Empty, NullContentsConfigSerializer);
            var installSettingsConfig = new InstallSettingsMock(fileSystem, NullWriter, string.Empty, NullInstallSettingsSerializer);
            var variablesConfig = new VariablesConfigMock(fileSystem, NullWriter, string.Empty, NullVariablesConfigSerializer);

            var configuration = new Configuration(steamAppsConfig, contentsConfig, installSettingsConfig, variablesConfig);
            configuration.GetContentInstallDir(string.Empty);
            Assert.AreEqual(1, installSettingsConfig.GetContentInstallDirTotal);
        }

        [TestMethod]
        public void Call_VariablesConfig_GetFileName()
        {
            var fileSystem = new MockFileSystem();
            var steamAppsConfig = new SteamAppsConfigMock(fileSystem, NullWriter, string.Empty, NullSteamAppsConfigSerializer, NullSteamPathFinder);
            var contentsConfig = new ContentsConfigMock(fileSystem, NullWriter, string.Empty, NullContentsConfigSerializer);
            var installSettingsConfig = new InstallSettingsMock(fileSystem, NullWriter, string.Empty, NullInstallSettingsSerializer);
            var variablesConfig = new VariablesConfigMock(fileSystem, NullWriter, string.Empty, NullVariablesConfigSerializer);

            var configuration = new Configuration(steamAppsConfig, contentsConfig, installSettingsConfig, variablesConfig);
            configuration.GetVariablesFileName();
            Assert.AreEqual(1, variablesConfig.FileNameTotal);
        }
    }
}