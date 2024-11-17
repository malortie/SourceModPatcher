using Pipelines;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;

namespace SourceContentInstaller.Tests
{
    public class NullSteamPathFinder : ISteamPathFinder
    {
        public string? GetSteamPath()
        {
            return null;
        }
    }

    public class EmptySteamPathFinder : ISteamPathFinder
    {
        public string? GetSteamPath()
        {
            return string.Empty;
        }
    }

    public class SteamPathFinderStub : ISteamPathFinder
    {
        public string SteamPath { get; set; } = string.Empty;

        public string? GetSteamPath()
        {
            return SteamPath;
        }
    }

    public class SteamAppsConfigTrySetupFromRegistryMock(IFileSystem fileSystem, IWriter writer, string filePath, IConfigurationSerializer<JSONSteamAppsConfig> configSerializer, ISteamPathFinder steamPathFinder) : SteamAppsConfig(fileSystem, writer, filePath, configSerializer, steamPathFinder)
    {
        public int TrySetupFromRegistryTotal { get; set; } = 0;

        protected override void TrySetupFromRegistry()
        {
            ++TrySetupFromRegistryTotal;
        }
    }

    [TestClass]
    public class TestSteamAppsConfig
    {
        static readonly IWriter NullWriter = new NullWriter();
        static readonly ISteamPathFinder EmptySteamPathFinder = new EmptySteamPathFinder();

        [TestMethod]
        public void Deserialize_JSONSteamAppsConfigEntry()
        {
            var serializer = new JSONConfigurationSerializer<JSONSteamAppsConfigEntry>();
            var deserialized = serializer.Deserialize("""
                {
                    "name": "Source SDK Base 2006",
                    "appmanifest_file": "appmanifest_215.acf",
                    "install_dir": "C:/Program Files (x86)/Steam/steamapps/common/Source SDK Base"
                }
                """);

            Assert.IsNotNull(deserialized);
            Assert.AreEqual("Source SDK Base 2006", deserialized.Name);
            Assert.AreEqual("appmanifest_215.acf", deserialized.AppManifestFile);
            Assert.AreEqual("C:/Program Files (x86)/Steam/steamapps/common/Source SDK Base", deserialized.InstallDir);
        }

        [TestMethod]
        public void Deserialize_JSONSteamAppsConfig()
        {
            var serializer = new JSONConfigurationSerializer<JSONSteamAppsConfig>();
            var deserialized = serializer.Deserialize("""
                {
                    "215": 
                    {
                        "name": "Source SDK Base 2006",
                        "appmanifest_file": "appmanifest_215.acf",
                        "install_dir": "C:/Program Files (x86)/Steam/steamapps/common/Source SDK Base"
                    },
                    "218": 
                    {
                        "name": "Source SDK Base 2007",
                        "appmanifest_file": "appmanifest_218.acf",
                        "install_dir": "C:/Program Files (x86)/Steam/steamapps/common/Source SDK Base 2007"
                    },
                    "220": 
                    {
                        "name": "Half-Life 2",
                        "appmanifest_file": "appmanifest_220.acf",
                        "install_dir": "C:/Program Files (x86)/Steam/steamapps/common/Half-Life 2"
                    }
                }
                """);

            Assert.IsNotNull(deserialized);
            Assert.IsTrue(deserialized.ContainsKey(215));
            Assert.IsTrue(deserialized.ContainsKey(218));
            Assert.IsTrue(deserialized.ContainsKey(220));

            var steamApp = deserialized[215];
            Assert.AreEqual("Source SDK Base 2006", steamApp.Name);
            Assert.AreEqual("appmanifest_215.acf", steamApp.AppManifestFile);
            Assert.AreEqual("C:/Program Files (x86)/Steam/steamapps/common/Source SDK Base", steamApp.InstallDir);

            steamApp = deserialized[218];
            Assert.AreEqual("Source SDK Base 2007", steamApp.Name);
            Assert.AreEqual("appmanifest_218.acf", steamApp.AppManifestFile);
            Assert.AreEqual("C:/Program Files (x86)/Steam/steamapps/common/Source SDK Base 2007", steamApp.InstallDir);

            steamApp = deserialized[220];
            Assert.AreEqual("Half-Life 2", steamApp.Name);
            Assert.AreEqual("appmanifest_220.acf", steamApp.AppManifestFile);
            Assert.AreEqual("C:/Program Files (x86)/Steam/steamapps/common/Half-Life 2", steamApp.InstallDir);
        }

        [TestMethod]
        public void LoadConfig_Simple()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                { "C:/steamapps.json", new MockFileData(File.ReadAllBytes("../../../data/config/steamapps.json")) },
                { "C:/Steam/steamapps/libraryfolders.vdf", new MockFileData(File.ReadAllBytes("../../../data/vdf/libraryfolders.vdf")) },
                { "C:/Steam/steamapps/appmanifest_215.acf", new MockFileData(File.ReadAllBytes("../../../data/vdf/appmanifest_215.acf")) },
                { "D:/SteamLibrary/steamapps/appmanifest_218.acf", new MockFileData(File.ReadAllBytes("../../../data/vdf/appmanifest_218.acf")) },
                { "E:/SteamLibrary/steamapps/appmanifest_220.acf", new MockFileData(File.ReadAllBytes("../../../data/vdf/appmanifest_220.acf")) },
            });

            var steamPathFinder = new SteamPathFinderStub { SteamPath = "C:/Steam" };
            var steamAppsConfig = new SteamAppsConfig(fileSystem, NullWriter, "C:/steamapps.json", new JSONConfigurationSerializer<JSONSteamAppsConfig>(), steamPathFinder);

            steamAppsConfig.LoadConfig();

            var config = steamAppsConfig.Config;
            Assert.IsNotNull(config);
            Assert.IsTrue(config.ContainsKey(215));
            Assert.IsTrue(config.ContainsKey(218));
            Assert.IsTrue(config.ContainsKey(220));

            var steamApp = config[215];
            Assert.AreEqual("Source SDK Base 2006", steamApp.Name);
            Assert.AreEqual("appmanifest_215.acf", steamApp.AppManifestFile);
            Assert.AreEqual("C:/Steam/steamapps/common/Source SDK Base", steamApp.InstallDir);

            steamApp = config[218];
            Assert.AreEqual("Source SDK Base 2007", steamApp.Name);
            Assert.AreEqual("appmanifest_218.acf", steamApp.AppManifestFile);
            Assert.AreEqual("D:/SteamLibrary/steamapps/common/Source SDK Base 2007", steamApp.InstallDir);

            steamApp = config[220];
            Assert.AreEqual("Half-Life 2", steamApp.Name);
            Assert.AreEqual("appmanifest_220.acf", steamApp.AppManifestFile);
            Assert.AreEqual("E:/SteamLibrary/steamapps/common/Half-Life 2", steamApp.InstallDir);
        }

        [TestMethod]
        public void LoadConfig_UseConfigFile()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                { "C:/steamapps.json", new MockFileData(File.ReadAllBytes("../../../data/config/steamapps.json")) }
            });

            var steamAppsConfig = new SteamAppsConfig(fileSystem, NullWriter, "C:/steamapps.json", new JSONConfigurationSerializer<JSONSteamAppsConfig>(), EmptySteamPathFinder)
            {
                UseConfigFile = true
            };
            steamAppsConfig.LoadConfig();

            var config = steamAppsConfig.Config;
            Assert.IsNotNull(config);
            Assert.IsTrue(config.ContainsKey(215));
            Assert.IsTrue(config.ContainsKey(218));
            Assert.IsTrue(config.ContainsKey(220));

            var steamApp = config[215];
            Assert.AreEqual("Source SDK Base 2006", steamApp.Name);
            Assert.AreEqual("appmanifest_215.acf", steamApp.AppManifestFile);
            Assert.AreEqual("C:/SteamLibrary/Source SDK Base", steamApp.InstallDir);

            steamApp = config[218];
            Assert.AreEqual("Source SDK Base 2007", steamApp.Name);
            Assert.AreEqual("appmanifest_218.acf", steamApp.AppManifestFile);
            Assert.AreEqual("D:/SteamLibrary/Source SDK Base 2007", steamApp.InstallDir);

            steamApp = config[220];
            Assert.AreEqual("Half-Life 2", steamApp.Name);
            Assert.AreEqual("appmanifest_220.acf", steamApp.AppManifestFile);
            Assert.AreEqual("E:/SteamLibrary/Half-Life 2", steamApp.InstallDir);
        }

        [TestMethod]
        public void LoadConfig_TrySetupFromRegistry_Called_WhenUseConfigFileIsFalse()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                { "C:/steamapps.json", new MockFileData(File.ReadAllBytes("../../../data/config/steamapps.json")) }
            });

            var steamAppsConfig = new SteamAppsConfigTrySetupFromRegistryMock(fileSystem, NullWriter, "C:/steamapps.json", new JSONConfigurationSerializer<JSONSteamAppsConfig>(), EmptySteamPathFinder)
            {
                UseConfigFile = false
            };
            steamAppsConfig.LoadConfig();

            Assert.AreEqual(1, steamAppsConfig.TrySetupFromRegistryTotal);
        }

        [TestMethod]
        public void LoadConfig_TrySetupFromRegistry_NotCalled_WhenUseConfigFileIsTrue()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                { "C:/steamapps.json", new MockFileData(File.ReadAllBytes("../../../data/config/steamapps.json")) }
            });

            var steamAppsConfig = new SteamAppsConfigTrySetupFromRegistryMock(fileSystem, NullWriter, "C:/steamapps.json", new JSONConfigurationSerializer<JSONSteamAppsConfig>(), EmptySteamPathFinder)
            {
                UseConfigFile = true
            };
            steamAppsConfig.LoadConfig();

            Assert.AreEqual(0, steamAppsConfig.TrySetupFromRegistryTotal);
        }

        [TestMethod]
        public void GetSteamAppName()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                { "C:/steamapps.json", new MockFileData(File.ReadAllBytes("../../../data/config/steamapps.json")) },
            });

            var steamAppsConfig = new SteamAppsConfig(fileSystem, NullWriter, "C:/steamapps.json", new JSONConfigurationSerializer<JSONSteamAppsConfig>(), EmptySteamPathFinder)
            {
                UseConfigFile = true
            };
            steamAppsConfig.LoadConfig();

            Assert.AreEqual("Source SDK Base 2006", steamAppsConfig.GetSteamAppName(215));
            Assert.AreEqual("Source SDK Base 2007", steamAppsConfig.GetSteamAppName(218));
            Assert.AreEqual("Half-Life 2", steamAppsConfig.GetSteamAppName(220));
        }

        [TestMethod]
        public void GetSteamAppInstallDir()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                { "C:/steamapps.json", new MockFileData(File.ReadAllBytes("../../../data/config/steamapps.json")) }
            });

            var steamAppsConfig = new SteamAppsConfig(fileSystem, NullWriter, "C:/steamapps.json", new JSONConfigurationSerializer<JSONSteamAppsConfig>(), EmptySteamPathFinder)
            {
                UseConfigFile = true
            };
            steamAppsConfig.LoadConfig();

            Assert.AreEqual("C:/SteamLibrary/Source SDK Base", steamAppsConfig.GetSteamAppInstallDir(215));
            Assert.AreEqual("D:/SteamLibrary/Source SDK Base 2007", steamAppsConfig.GetSteamAppInstallDir(218));
            Assert.AreEqual("E:/SteamLibrary/Half-Life 2", steamAppsConfig.GetSteamAppInstallDir(220));
        }

        [TestMethod]
        public void GetSteamAppInstallDirVariable()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                { "C:/steamapps.json", new MockFileData(File.ReadAllBytes("../../../data/config/steamapps.json")) }
            });

            var steamAppsConfig = new SteamAppsConfig(fileSystem, NullWriter, "C:/steamapps.json", new JSONConfigurationSerializer<JSONSteamAppsConfig>(), EmptySteamPathFinder)
            {
                UseConfigFile = true
            };
            steamAppsConfig.LoadConfig();

            Assert.AreEqual("steamapps_sdkbase2006_install_dir", steamAppsConfig.GetSteamAppInstallDirVariable(215));
            Assert.AreEqual("steamapps_sdkbase2007_install_dir", steamAppsConfig.GetSteamAppInstallDirVariable(218));
            Assert.AreEqual("steamapps_hl2_install_dir", steamAppsConfig.GetSteamAppInstallDirVariable(220));
        }

        [TestMethod]
        public void IsSteamAppInstalled()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                { "C:/steamapps_not_installed.json", new MockFileData(File.ReadAllBytes("../../../data/config/steamapps_not_installed.json")) },
                { "C:/SteamLibrary/Source SDK Base", new MockDirectoryData() },
                { "E:/SteamLibrary/Half-Life 2", new MockDirectoryData() }
            });

            var steamAppsConfig = new SteamAppsConfig(fileSystem, NullWriter, "C:/steamapps_not_installed.json", new JSONConfigurationSerializer<JSONSteamAppsConfig>(), EmptySteamPathFinder)
            {
                UseConfigFile = true
            };
            steamAppsConfig.LoadConfig();

            Assert.IsTrue(steamAppsConfig.IsSteamAppInstalled(215));
            Assert.IsFalse(steamAppsConfig.IsSteamAppInstalled(218));
            Assert.IsTrue(steamAppsConfig.IsSteamAppInstalled(220));
            Assert.IsFalse(steamAppsConfig.IsSteamAppInstalled(320));
        }

        [TestMethod]
        public void GetInstalledSteamApps()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                { "C:/steamapps_not_installed.json", new MockFileData(File.ReadAllBytes("../../../data/config/steamapps_not_installed.json")) },
                { "C:/SteamLibrary/Source SDK Base", new MockDirectoryData() },
                { "E:/SteamLibrary/Half-Life 2", new MockDirectoryData() }
            });

            var steamAppsConfig = new SteamAppsConfig(fileSystem, NullWriter, "C:/steamapps_not_installed.json", new JSONConfigurationSerializer<JSONSteamAppsConfig>(), EmptySteamPathFinder)
            {
                UseConfigFile = true
            };
            steamAppsConfig.LoadConfig();

            CollectionAssert.AreEquivalent(new List<int> { 215, 220 }, steamAppsConfig.GetInstalledSteamApps());
        }

        [TestMethod]
        public void SupportedSourceGamesAppIDs_Sorted()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                { "C:/steamapps_unsorted.json", new MockFileData(File.ReadAllBytes("../../../data/config/steamapps_unsorted.json")) }
            });

            var steamAppsConfig = new SteamAppsConfig(fileSystem, NullWriter, "C:/steamapps_unsorted.json", new JSONConfigurationSerializer<JSONSteamAppsConfig>(), EmptySteamPathFinder)
            {
                UseConfigFile = true
            };
            steamAppsConfig.LoadConfig();

            CollectionAssert.AreEqual(new List<int> { 215, 218, 220 }, steamAppsConfig.SupportedSourceGamesAppIDs);
        }

        [TestMethod]
        public void LoadConfig_ThrowException_WhenSteamPathIsNull()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                { "C:/steamapps.json", new MockFileData(File.ReadAllBytes("../../../data/config/steamapps.json")) }
            });

            var steamAppsConfig = new SteamAppsConfig(fileSystem, NullWriter, "C:/steamapps.json", new JSONConfigurationSerializer<JSONSteamAppsConfig>(), new NullSteamPathFinder());

            Assert.ThrowsException<Exception>(() => steamAppsConfig.LoadConfig());
        }

        [TestMethod]
        public void LoadConfig_ThrowFileNotFoundException_WhenLibraryFoldersDoesNotExist()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                { "C:/steamapps.json", new MockFileData(File.ReadAllBytes("../../../data/config/steamapps.json")) }
            });

            var steamPathFinder = new SteamPathFinderStub { SteamPath = "C:/Steam" };
            var steamAppsConfig = new SteamAppsConfig(fileSystem, NullWriter, "C:/steamapps.json", new JSONConfigurationSerializer<JSONSteamAppsConfig>(), steamPathFinder);

            Assert.ThrowsException<FileNotFoundException>(() => steamAppsConfig.LoadConfig());
        }

        [TestMethod]
        public void LoadConfig_ThrowFileNotFoundException_WhenAppManifestFileDoesNotExist()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                { "C:/steamapps.json", new MockFileData(File.ReadAllBytes("../../../data/config/steamapps.json")) },
                { "C:/Steam/steamapps/libraryfolders.vdf", new MockFileData(File.ReadAllBytes("../../../data/vdf/libraryfolders.vdf")) },
            });

            var steamPathFinder = new SteamPathFinderStub { SteamPath = "C:/Steam" };
            var steamAppsConfig = new SteamAppsConfig(fileSystem, NullWriter, "C:/steamapps.json", new JSONConfigurationSerializer<JSONSteamAppsConfig>(), steamPathFinder);

            Assert.ThrowsException<FileNotFoundException>(() => steamAppsConfig.LoadConfig());
        }
    }
}