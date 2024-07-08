using Pipelines;
using System.IO.Abstractions.TestingHelpers;

namespace SourceContentInstaller.Tests
{
    [TestClass]
    public class TestInstallSettings
    {
        static readonly IWriter NullWriter = new NullWriter();

        [TestMethod]
        public void Deserialize_JSONInstallSettingsEntry()
        {
            var serializer = new JSONConfigurationSerializer<JSONInstallSettingsEntry>();
            var deserialized = serializer.Deserialize("""
                {
                    "install": false,
                    "install_dir": "C:/Documents/SourceContent/sdkbase2006"
                }
                """);

            Assert.IsNotNull(deserialized);
            Assert.IsFalse(deserialized.Install);
            Assert.AreEqual("C:/Documents/SourceContent/sdkbase2006", deserialized.InstallDir);
        }

        [TestMethod]
        public void Deserialize_JSONInstallSettings()
        {
            var serializer = new JSONConfigurationSerializer<JSONInstallSettings>();
            var deserialized = serializer.Deserialize("""
                {
                    "sdkbase2006": {
                        "install": false,
                        "install_dir": "C:/Documents/SourceContent/sdkbase2006"
                    },
                    "sdkbase2007": {
                        "install": true,
                        "install_dir": "C:/Documents/SourceContent/sdkbase2007"
                    },
                    "hl2": {
                        "install": false,
                        "install_dir": "C:/Documents/SourceContent/hl2"
                    }
                }
                """);

            Assert.IsNotNull(deserialized);
            Assert.IsTrue(deserialized.ContainsKey("sdkbase2006"));
            Assert.IsTrue(deserialized.ContainsKey("sdkbase2007"));
            Assert.IsTrue(deserialized.ContainsKey("hl2"));

            Assert.IsFalse(deserialized["sdkbase2006"].Install);
            Assert.AreEqual("C:/Documents/SourceContent/sdkbase2006", deserialized["sdkbase2006"].InstallDir);

            Assert.IsTrue(deserialized["sdkbase2007"].Install);
            Assert.AreEqual("C:/Documents/SourceContent/sdkbase2007", deserialized["sdkbase2007"].InstallDir);

            Assert.IsFalse(deserialized["hl2"].Install);
            Assert.AreEqual("C:/Documents/SourceContent/hl2", deserialized["hl2"].InstallDir);
        }

        [TestMethod]
        public void LoadConfig_Simple()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                { "C:/contents.install.settings.json", new MockFileData(File.ReadAllBytes("../../../data/config/contents.install.settings.json")) }
            });

            var installSettings = new InstallSettings(fileSystem, NullWriter, "C:/contents.install.settings.json", new JSONConfigurationSerializer<JSONInstallSettings>());
            installSettings.LoadConfig();

            var config = installSettings.Config;
            Assert.IsNotNull(config);
            Assert.IsTrue(config.ContainsKey("sdkbase2006"));
            Assert.IsTrue(config.ContainsKey("sdkbase2007"));
            Assert.IsTrue(config.ContainsKey("hl2"));
            Assert.IsTrue(config.ContainsKey("cstrike"));

            var contentID = config["sdkbase2006"];
            Assert.IsFalse(contentID.Install);
            Assert.AreEqual("C:/sdkbase2006", contentID.InstallDir);

            contentID = config["sdkbase2007"];
            Assert.IsTrue(contentID.Install);
            Assert.AreEqual("C:/sdkbase2007", contentID.InstallDir);

            contentID = config["hl2"];
            Assert.IsFalse(contentID.Install);
            Assert.AreEqual("C:/hl2", contentID.InstallDir);

            contentID = config["cstrike"];
            Assert.IsTrue(contentID.Install);
            Assert.AreEqual("C:/cstrike", contentID.InstallDir);
        }

        [TestMethod]
        public void GetContentsToInstall()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                { "C:/contents.install.settings.json", new MockFileData(File.ReadAllBytes("../../../data/config/contents.install.settings.json")) }
            });

            var installSettings = new InstallSettings(fileSystem, NullWriter, "C:/contents.install.settings.json", new JSONConfigurationSerializer<JSONInstallSettings>());
            installSettings.LoadConfig();

            CollectionAssert.AreEquivalent(new List<string> { "sdkbase2007", "cstrike" }, installSettings.GetContentsToInstall());
        }

        [TestMethod]
        public void IsSteamAppMarkedForInstall()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                { "C:/contents.install.settings.json", new MockFileData(File.ReadAllBytes("../../../data/config/contents.install.settings.json")) }
            });

            var installSettings = new InstallSettings(fileSystem, NullWriter, "C:/contents.install.settings.json", new JSONConfigurationSerializer<JSONInstallSettings>());
            installSettings.LoadConfig();

            Assert.IsFalse(installSettings.IsContentMarkedForInstall("sdkbase2006"));
            Assert.IsTrue(installSettings.IsContentMarkedForInstall("sdkbase2007"));
            Assert.IsFalse(installSettings.IsContentMarkedForInstall("hl2"));
            Assert.IsTrue(installSettings.IsContentMarkedForInstall("cstrike"));
        }

        [TestMethod]
        public void GetContentInstallDir()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                { "C:/contents.install.settings.json", new MockFileData(File.ReadAllBytes("../../../data/config/contents.install.settings.json")) }
            });

            var installSettings = new InstallSettings(fileSystem, NullWriter, "C:/contents.install.settings.json", new JSONConfigurationSerializer<JSONInstallSettings>());
            installSettings.LoadConfig();

            Assert.AreEqual("C:/sdkbase2006", installSettings.GetContentInstallDir("sdkbase2006"));
            Assert.AreEqual("C:/sdkbase2007", installSettings.GetContentInstallDir("sdkbase2007"));
            Assert.AreEqual("C:/hl2", installSettings.GetContentInstallDir("hl2"));
            Assert.AreEqual("C:/cstrike", installSettings.GetContentInstallDir("cstrike"));
        }
    }
}