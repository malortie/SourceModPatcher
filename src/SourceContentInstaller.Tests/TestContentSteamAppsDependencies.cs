using Pipelines;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Serialization;

namespace SourceContentInstaller.Tests
{
    public class SteamAppsConfigMock2(IFileSystem fileSystem, IWriter writer, string filePath, IConfigurationSerializer<JSONSteamAppsConfig> configSerializer) : SteamAppsConfig(fileSystem, writer, filePath, configSerializer, null)
    {
        public List<int> InstalledSteamApps { get; set; } = [];

        public override List<int> GetInstalledSteamApps()
        {
            return InstalledSteamApps;
        }
    }

    [TestClass]
    public class TestContentSteamAppsDependencies
    {
        static readonly IWriter NullWriter = new NullWriter();
        static readonly IConfigurationSerializer<JSONSteamAppsConfig> NullSteamAppsConfigSerializer = new NullConfigurationSerializer<JSONSteamAppsConfig>();
        static readonly IConfigurationSerializer<JSONContentsConfig> NullContentsConfigSerializer = new NullConfigurationSerializer<JSONContentsConfig>();
        static readonly IConfigurationSerializer<JSONVariablesConfig> NullVariablesConfigSerializer = new NullConfigurationSerializer<JSONVariablesConfig>();

        [TestMethod]
        public void AreAllContentSteamAppsDependenciesInstalled_AllSteamAppsDependenciesInstalled_ReturnsTrue()
        {
            var fileSystem = new MockFileSystem();
            var contentSteamAppsDependencies = new ContentSteamAppsDependencies(
                new SteamAppsConfigMock2(fileSystem, NullWriter, string.Empty, NullSteamAppsConfigSerializer) {
                    InstalledSteamApps = [215, 218] },
                new ContentsConfig(fileSystem, NullWriter, string.Empty, NullContentsConfigSerializer) {
                    Config = {
                        { "content_1", new JSONContentsConfigEntry { SteamAppsDependencies = [215, 218] } }
                    }
                },
                new VariablesConfig(fileSystem, NullWriter, string.Empty, NullVariablesConfigSerializer));

            Assert.IsTrue(contentSteamAppsDependencies.AreAllContentSteamAppsDependenciesInstalled("content_1"));
        }

        [TestMethod]
        public void AreAllContentSteamAppsDependenciesInstalled_NotAllSteamAppsDependenciesInstalled_ReturnsFalse()
        {
            var fileSystem = new MockFileSystem();
            var contentSteamAppsDependencies = new ContentSteamAppsDependencies(
                new SteamAppsConfigMock2(fileSystem, NullWriter, string.Empty, NullSteamAppsConfigSerializer) {
                    InstalledSteamApps = [215] },
                new ContentsConfig(fileSystem, NullWriter, string.Empty, NullContentsConfigSerializer) {
                    Config = {
                        { "content_1", new JSONContentsConfigEntry { SteamAppsDependencies = [215, 218] } }
                    }
                },
                new VariablesConfig(fileSystem, NullWriter, string.Empty, NullVariablesConfigSerializer));

            Assert.IsFalse(contentSteamAppsDependencies.AreAllContentSteamAppsDependenciesInstalled("content_1"));
        }

        [TestMethod]
        public void GetInstallableContent()
        {
            var fileSystem = new MockFileSystem();
            var contentSteamAppsDependencies = new ContentSteamAppsDependencies(
                new SteamAppsConfigMock2(fileSystem, NullWriter, string.Empty, NullSteamAppsConfigSerializer)
                {
                    InstalledSteamApps = [215, 218, 220, 320]
                },
                new ContentsConfig(fileSystem, NullWriter, string.Empty, NullContentsConfigSerializer)
                {
                    Config = {
                        { "content_1", new JSONContentsConfigEntry { SteamAppsDependencies = [220] } },
                        { "content_2", new JSONContentsConfigEntry { SteamAppsDependencies = [320] } },
                        { "content_3", new JSONContentsConfigEntry { SteamAppsDependencies = [380] } },
                        { "content_4", new JSONContentsConfigEntry { SteamAppsDependencies = [420] } },
                    }
                },
                new VariablesConfig(fileSystem, NullWriter, string.Empty, NullVariablesConfigSerializer));

            CollectionAssert.AreEquivalent(new List<string>() { "content_1", "content_2" }, contentSteamAppsDependencies.GetInstallableContent());
        }

        [TestMethod]
        public void IsContentInstalled()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                { "C:/content_2_output_1", new MockDirectoryData() },
                { "C:/content_3_output_2", new MockDirectoryData() },
                { "C:/content_4_output_1", new MockDirectoryData() },
                { "C:/content_4_output_2", new MockDirectoryData() },
            });
            var contentSteamAppsDependencies = new ContentSteamAppsDependencies(
                new SteamAppsConfigMock2(fileSystem, NullWriter, string.Empty, NullSteamAppsConfigSerializer),
                new ContentsConfig(fileSystem, NullWriter, string.Empty, NullContentsConfigSerializer)
                {
                    Config = {
                        { "content_1", new JSONContentsConfigEntry { OutputVariables = ["content_1_output_1", "content_1_output_2"] } },
                        { "content_2", new JSONContentsConfigEntry { OutputVariables = ["content_2_output_1", "content_2_output_2"] } },
                        { "content_3", new JSONContentsConfigEntry { OutputVariables = ["content_3_output_1", "content_3_output_2"] } },
                        { "content_4", new JSONContentsConfigEntry { OutputVariables = ["content_4_output_1", "content_4_output_2"] } },
                    }
                },
                new VariablesConfig(fileSystem, NullWriter, string.Empty, NullVariablesConfigSerializer) {
                    Config = {
                        { "content_2_output_1", "C:/content_2_output_1" },
                        { "content_3_output_2", "C:/content_3_output_2" },
                        { "content_4_output_1", "C:/content_4_output_1" },
                        { "content_4_output_2", "C:/content_4_output_2" },
                    }
                });

            Assert.IsFalse(contentSteamAppsDependencies.IsContentInstalled("content_1"));
            Assert.IsFalse(contentSteamAppsDependencies.IsContentInstalled("content_2"));
            Assert.IsFalse(contentSteamAppsDependencies.IsContentInstalled("content_3"));
            Assert.IsTrue(contentSteamAppsDependencies.IsContentInstalled("content_4"));
        }
    }
}
