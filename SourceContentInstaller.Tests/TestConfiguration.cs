using Pipelines;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using test_installsourcecontent;

namespace test_installsourcecontent_tests
{
    public class SteamAppsConfigMock : SteamAppsConfig
    {
        public SteamAppsConfigMock(IFileSystem fileSystem, IWriter writer, string filePath, IConfigurationSerializer<JSONSteamAppsConfig> configSerializer, ISteamPathFinder steamPathFinder) : base(fileSystem, writer, filePath, configSerializer, steamPathFinder)
        {
        }

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

    public class InstallSettingsMock : InstallSettings
    {
        public InstallSettingsMock(IFileSystem fileSystem, IWriter writer, string filePath, IConfigurationSerializer<JSONInstallSettings> configSerializer) : base(fileSystem, writer, filePath, configSerializer)
        {
        }

        public int GetContentInstallDirTotal { get; private set; } = 0;

        public override string GetContentInstallDir(int appID)
        {
            ++GetContentInstallDirTotal;
            return string.Empty;
        }
    }

    public class VariablesConfigMock : VariablesConfig
    {
        public VariablesConfigMock(IFileSystem fileSystem, IWriter writer, string filePath, IConfigurationSerializer<JSONVariablesConfig> configSerializer) : base(fileSystem, writer, filePath, configSerializer)
        {
        }

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
        static IWriter NullWriter = new NullWriter();
        static ISteamPathFinder NullSteamPathFinder = new NullSteamPathFinder();
        static IConfigurationSerializer<JSONSteamAppsConfig> NullSteamAppsConfigSerializer = new NullConfigurationSerializer<JSONSteamAppsConfig>();
        static IConfigurationSerializer<JSONInstallSettings> NullInstallSettingsSerializer = new NullConfigurationSerializer<JSONInstallSettings>();
        static IConfigurationSerializer<JSONVariablesConfig> NullVariablesConfigSerializer = new NullConfigurationSerializer<JSONVariablesConfig>();

        [TestMethod]
        public void Call_SteamAppsConfig_GetSteamAppName()
        {
            var fileSystem = new MockFileSystem();
            var steamAppsConfig = new SteamAppsConfigMock(fileSystem, NullWriter, string.Empty, NullSteamAppsConfigSerializer, NullSteamPathFinder);
            var installSettingsConfig = new InstallSettingsMock(fileSystem, NullWriter, string.Empty, NullInstallSettingsSerializer);
            var variablesConfig = new VariablesConfigMock(fileSystem, NullWriter, string.Empty, NullVariablesConfigSerializer);

            var configuration = new Configuration(steamAppsConfig, installSettingsConfig, variablesConfig);
            configuration.GetSteamAppName(0);
            Assert.AreEqual(1, steamAppsConfig.GetSteamAppNameTotal);
        }

        [TestMethod]
        public void Call_SteamAppsConfig_GetSteamAppInstallDir()
        {
            var fileSystem = new MockFileSystem();
            var steamAppsConfig = new SteamAppsConfigMock(fileSystem, NullWriter, string.Empty, NullSteamAppsConfigSerializer, NullSteamPathFinder);
            var installSettingsConfig = new InstallSettingsMock(fileSystem, NullWriter, string.Empty, NullInstallSettingsSerializer);
            var variablesConfig = new VariablesConfigMock(fileSystem, NullWriter, string.Empty, NullVariablesConfigSerializer);

            var configuration = new Configuration(steamAppsConfig, installSettingsConfig, variablesConfig);
            configuration.GetSteamAppInstallDir(0);
            Assert.AreEqual(1, steamAppsConfig.GetSteamAppInstallDirTotal);
        }

        [TestMethod]
        public void Call_VariablesConfig_SaveVariable()
        {
            var fileSystem = new MockFileSystem();
            var steamAppsConfig = new SteamAppsConfigMock(fileSystem, NullWriter, string.Empty, NullSteamAppsConfigSerializer, NullSteamPathFinder);
            var installSettingsConfig = new InstallSettingsMock(fileSystem, NullWriter, string.Empty, NullInstallSettingsSerializer);
            var variablesConfig = new VariablesConfigMock(fileSystem, NullWriter, string.Empty, NullVariablesConfigSerializer);

            var configuration = new Configuration(steamAppsConfig, installSettingsConfig, variablesConfig);
            configuration.SaveVariable("hl2_content_path", "C:/Half-Life 2");
            Assert.AreEqual(1, variablesConfig.SaveVariableTotal);
        }

        [TestMethod]
        public void Call_InstallSettings_GetContentInstallDir()
        {
            var fileSystem = new MockFileSystem();
            var steamAppsConfig = new SteamAppsConfigMock(fileSystem, NullWriter, string.Empty, NullSteamAppsConfigSerializer, NullSteamPathFinder);
            var installSettingsConfig = new InstallSettingsMock(fileSystem, NullWriter, string.Empty, NullInstallSettingsSerializer);
            var variablesConfig = new VariablesConfigMock(fileSystem, NullWriter, string.Empty, NullVariablesConfigSerializer);

            var configuration = new Configuration(steamAppsConfig, installSettingsConfig, variablesConfig);
            configuration.GetContentInstallDir(0);
            Assert.AreEqual(1, installSettingsConfig.GetContentInstallDirTotal);
        }

        [TestMethod]
        public void Call_VariablesConfig_GetFileName()
        {
            var fileSystem = new MockFileSystem();
            var steamAppsConfig = new SteamAppsConfigMock(fileSystem, NullWriter, string.Empty, NullSteamAppsConfigSerializer, NullSteamPathFinder);
            var installSettingsConfig = new InstallSettingsMock(fileSystem, NullWriter, string.Empty, NullInstallSettingsSerializer);
            var variablesConfig = new VariablesConfigMock(fileSystem, NullWriter, string.Empty, NullVariablesConfigSerializer);

            var configuration = new Configuration(steamAppsConfig, installSettingsConfig, variablesConfig);
            configuration.GetVariablesFileName();
            Assert.AreEqual(1, variablesConfig.FileNameTotal);
        }
    }
}