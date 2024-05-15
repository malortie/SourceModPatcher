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
                    "215": {
                        "install": false,
                        "install_dir": "C:/Documents/SourceContent/sdkbase2006"
                    },
                    "218": {
                        "install": true,
                        "install_dir": "C:/Documents/SourceContent/sdkbase2007"
                    },
                    "220": {
                        "install": false,
                        "install_dir": "C:/Documents/SourceContent/hl2"
                    }
                }
                """);

            Assert.IsNotNull(deserialized);
            Assert.IsTrue(deserialized.ContainsKey(215));
            Assert.IsTrue(deserialized.ContainsKey(218));
            Assert.IsTrue(deserialized.ContainsKey(220));

            Assert.IsFalse(deserialized[215].Install);
            Assert.AreEqual("C:/Documents/SourceContent/sdkbase2006", deserialized[215].InstallDir);

            Assert.IsTrue(deserialized[218].Install);
            Assert.AreEqual("C:/Documents/SourceContent/sdkbase2007", deserialized[218].InstallDir);

            Assert.IsFalse(deserialized[220].Install);
            Assert.AreEqual("C:/Documents/SourceContent/hl2", deserialized[220].InstallDir);
        }

        [TestMethod]
        public void LoadConfig_Simple()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                { "C:/steamapps.install.settings.json", new MockFileData(File.ReadAllBytes("../../../data/config/steamapps.install.settings.json")) }
            });

            var installSettings = new InstallSettings(fileSystem, NullWriter, "C:/steamapps.install.settings.json", new JSONConfigurationSerializer<JSONInstallSettings>());
            installSettings.LoadConfig();

            var config = installSettings.Config;
            Assert.IsNotNull(config);
            Assert.IsTrue(config.ContainsKey(215));
            Assert.IsTrue(config.ContainsKey(218));
            Assert.IsTrue(config.ContainsKey(220));
            Assert.IsTrue(config.ContainsKey(240));

            var steamApp = config[215];
            Assert.IsFalse(steamApp.Install);
            Assert.AreEqual("C:/sdkbase2006", steamApp.InstallDir);

            steamApp = config[218];
            Assert.IsTrue(steamApp.Install);
            Assert.AreEqual("C:/sdkbase2007", steamApp.InstallDir);

            steamApp = config[220];
            Assert.IsFalse(steamApp.Install);
            Assert.AreEqual("C:/hl2", steamApp.InstallDir);

            steamApp = config[240];
            Assert.IsTrue(steamApp.Install);
            Assert.AreEqual("C:/cstrike", steamApp.InstallDir);
        }

        [TestMethod]
        public void GetSteamAppsToInstall()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                { "C:/steamapps.install.settings.json", new MockFileData(File.ReadAllBytes("../../../data/config/steamapps.install.settings.json")) }
            });

            var installSettings = new InstallSettings(fileSystem, NullWriter, "C:/steamapps.install.settings.json", new JSONConfigurationSerializer<JSONInstallSettings>());
            installSettings.LoadConfig();

            CollectionAssert.AreEquivalent(new List<int> { 218, 240 }, installSettings.GetSteamAppsToInstall());
        }

        [TestMethod]
        public void IsSteamAppMarkedForInstall()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                { "C:/steamapps.install.settings.json", new MockFileData(File.ReadAllBytes("../../../data/config/steamapps.install.settings.json")) }
            });

            var installSettings = new InstallSettings(fileSystem, NullWriter, "C:/steamapps.install.settings.json", new JSONConfigurationSerializer<JSONInstallSettings>());
            installSettings.LoadConfig();

            Assert.IsFalse(installSettings.IsSteamAppMarkedForInstall(215));
            Assert.IsTrue(installSettings.IsSteamAppMarkedForInstall(218));
            Assert.IsFalse(installSettings.IsSteamAppMarkedForInstall(220));
            Assert.IsTrue(installSettings.IsSteamAppMarkedForInstall(240));
        }

        [TestMethod]
        public void GetContentInstallDir()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                { "C:/steamapps.install.settings.json", new MockFileData(File.ReadAllBytes("../../../data/config/steamapps.install.settings.json")) }
            });

            var installSettings = new InstallSettings(fileSystem, NullWriter, "C:/steamapps.install.settings.json", new JSONConfigurationSerializer<JSONInstallSettings>());
            installSettings.LoadConfig();

            Assert.AreEqual("C:/sdkbase2006", installSettings.GetContentInstallDir(215));
            Assert.AreEqual("C:/sdkbase2007", installSettings.GetContentInstallDir(218));
            Assert.AreEqual("C:/hl2", installSettings.GetContentInstallDir(220));
            Assert.AreEqual("C:/cstrike", installSettings.GetContentInstallDir(240));
        }
    }
}