using SourceContentInstaller;
using JSONInstallSettingsEntry = SourceModPatcher.JSONInstallSettingsEntry;
using JSONInstallSettings = SourceModPatcher.JSONInstallSettings;
using InstallSettings = SourceModPatcher.InstallSettings;
using System.IO.Abstractions.TestingHelpers;
using Pipelines;

namespace SourceModPatcher.Tests
{
    [TestClass]
    public class TestInstallSettings
    {
        static IWriter NullWriter = new NullWriter();

        [TestMethod]
        public void Deserialize_JSONInstallSettingsEntry()
        {
            var serializer = new JSONConfigurationSerializer<JSONInstallSettingsEntry>();
            var deserialized = serializer.Deserialize("""
                {
                    "install": false
                }
                """);

            Assert.IsNotNull(deserialized);
            Assert.IsFalse(deserialized.Install);
        }

        [TestMethod]
        public void Deserialize_JSONInstallSettings()
        {
            var serializer = new JSONConfigurationSerializer<JSONInstallSettings>();
            var deserialized = serializer.Deserialize("""
                {
                    "sourcemod_1": {
                        "install": false
                    },
                    "sourcemod_2": {
                        "install": true
                    },
                    "sourcemod_3": {
                        "install": false
                    }
                }
                """);

            Assert.IsNotNull(deserialized);
            Assert.IsTrue(deserialized.ContainsKey("sourcemod_1"));
            Assert.IsTrue(deserialized.ContainsKey("sourcemod_2"));
            Assert.IsTrue(deserialized.ContainsKey("sourcemod_3"));

            Assert.IsFalse(deserialized["sourcemod_1"].Install);
            Assert.IsTrue(deserialized["sourcemod_2"].Install);
            Assert.IsFalse(deserialized["sourcemod_3"].Install);
        }

        [TestMethod]
        public void LoadConfig_Simple()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                { "C:/sourcemods.install.settings.json", new MockFileData(File.ReadAllBytes("../../../data/config/sourcemods.install.settings.json")) }
            });

            var installSettings = new InstallSettings(fileSystem, NullWriter, "C:/sourcemods.install.settings.json", new JSONConfigurationSerializer<JSONInstallSettings>());
            installSettings.LoadConfig();

            var config = installSettings.Config;
            Assert.IsNotNull(config);
            Assert.IsTrue(config.ContainsKey("sourcemod_1"));
            Assert.IsTrue(config.ContainsKey("sourcemod_2"));
            Assert.IsTrue(config.ContainsKey("sourcemod_3"));
            Assert.IsTrue(config.ContainsKey("sourcemod_4"));

            Assert.IsFalse(config["sourcemod_1"].Install);
            Assert.IsTrue(config["sourcemod_2"].Install);
            Assert.IsFalse(config["sourcemod_3"].Install);
            Assert.IsTrue(config["sourcemod_4"].Install);
        }

        [TestMethod]
        public void GetSourceModsToInstall()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                { "C:/sourcemods.install.settings.json", new MockFileData(File.ReadAllBytes("../../../data/config/sourcemods.install.settings.json")) }
            });

            var installSettings = new InstallSettings(fileSystem, NullWriter, "C:/sourcemods.install.settings.json", new JSONConfigurationSerializer<JSONInstallSettings>());
            installSettings.LoadConfig();

            CollectionAssert.AreEquivalent(new List<string> { "sourcemod_2", "sourcemod_4" }, installSettings.GetSourceModsToInstall());
        }

        [TestMethod]
        public void IsSourceModMarkedForInstall()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                { "C:/sourcemods.install.settings.json", new MockFileData(File.ReadAllBytes("../../../data/config/sourcemods.install.settings.json")) }
            });

            var installSettings = new InstallSettings(fileSystem, NullWriter, "C:/sourcemods.install.settings.json", new JSONConfigurationSerializer<JSONInstallSettings>());
            installSettings.LoadConfig();

            Assert.IsFalse(installSettings.IsSourceModMarkedForInstall("sourcemod_1"));
            Assert.IsTrue(installSettings.IsSourceModMarkedForInstall("sourcemod_2"));
            Assert.IsFalse(installSettings.IsSourceModMarkedForInstall("sourcemod_3"));
            Assert.IsTrue(installSettings.IsSourceModMarkedForInstall("sourcemod_4"));
        }
    }
}