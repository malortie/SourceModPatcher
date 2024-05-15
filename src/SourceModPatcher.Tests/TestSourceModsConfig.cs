using Pipelines;
using SourceContentInstaller;
using System.IO.Abstractions.TestingHelpers;

namespace SourceModPatcher.Tests
{
    [TestClass]
    public class TestSourceModsConfig
    {
        static readonly IWriter NullWriter = new NullWriter();

        [TestMethod]
        public void Deserialize_JSONSourceModsConfigEntry()
        {
            var serializer = new JSONConfigurationSerializer<JSONSourceModsConfigEntry>();
            var deserialized = serializer.Deserialize("""
                {
                    "name": "Source mod 1",
                    "sourcemod_folder": "SourceMod_1",
                    "data_dir": "./data/mods/SourceMod_1"
                }
                """);

            Assert.IsNotNull(deserialized);
            Assert.AreEqual("Source mod 1", deserialized.Name);
            Assert.AreEqual("SourceMod_1", deserialized.SourceModFolder);
            Assert.AreEqual("./data/mods/SourceMod_1", deserialized.DataDir);
        }

        [TestMethod]
        public void Deserialize_JSONSourceModsConfig()
        {
            var serializer = new JSONConfigurationSerializer<JSONSourceModsConfig>();
            var deserialized = serializer.Deserialize("""
                {
                    "sourcemod_1": 
                    {
                        "name": "Source mod 1",
                        "sourcemod_folder": "SourceMod_1",
                        "data_dir": "./data/mods/SourceMod_1"
                    },
                    "sourcemod_2": 
                    {
                        "name": "Source mod 2",
                        "sourcemod_folder": "SourceMod_2",
                        "data_dir": "./data/mods/SourceMod_2"
                    },
                    "sourcemod_3": 
                    {
                        "name": "Source mod 3",
                        "sourcemod_folder": "SourceMod_3",
                        "data_dir": "./data/mods/SourceMod_3"
                    }
                }
                """);

            Assert.IsNotNull(deserialized);
            Assert.IsTrue(deserialized.ContainsKey("sourcemod_1"));
            Assert.IsTrue(deserialized.ContainsKey("sourcemod_2"));
            Assert.IsTrue(deserialized.ContainsKey("sourcemod_3"));

            var sourceMod = deserialized["sourcemod_1"];
            Assert.AreEqual("Source mod 1", sourceMod.Name);
            Assert.AreEqual("SourceMod_1", sourceMod.SourceModFolder);
            Assert.AreEqual("./data/mods/SourceMod_1", sourceMod.DataDir);

            sourceMod = deserialized["sourcemod_2"];
            Assert.AreEqual("Source mod 2", sourceMod.Name);
            Assert.AreEqual("SourceMod_2", sourceMod.SourceModFolder);
            Assert.AreEqual("./data/mods/SourceMod_2", sourceMod.DataDir);

            sourceMod = deserialized["sourcemod_3"];
            Assert.AreEqual("Source mod 3", sourceMod.Name);
            Assert.AreEqual("SourceMod_3", sourceMod.SourceModFolder);
            Assert.AreEqual("./data/mods/SourceMod_3", sourceMod.DataDir);
        }

        [TestMethod]
        public void LoadConfig_Simple()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                { "C:/sourcemods.json", new MockFileData(File.ReadAllBytes("../../../data/config/sourcemods.json")) },
            });

            var sourceModsConfig = new SourceModsConfig(fileSystem, NullWriter, "C:/sourcemods.json", new JSONConfigurationSerializer<JSONSourceModsConfig>(), string.Empty);

            sourceModsConfig.LoadConfig();

            var config = sourceModsConfig.Config;
            Assert.IsNotNull(config);
            Assert.IsTrue(config.ContainsKey("sourcemod_1"));
            Assert.IsTrue(config.ContainsKey("sourcemod_2"));
            Assert.IsTrue(config.ContainsKey("sourcemod_3"));

            var sourceMod = config["sourcemod_1"];
            Assert.AreEqual("Source mod 1", sourceMod.Name);
            Assert.AreEqual("sourcemod_1_folder", sourceMod.SourceModFolder);
            Assert.AreEqual("./data/mods/sourcemod_1", sourceMod.DataDir);

            sourceMod = config["sourcemod_2"];
            Assert.AreEqual("Source mod 2", sourceMod.Name);
            Assert.AreEqual("sourcemod_2_folder", sourceMod.SourceModFolder);
            Assert.AreEqual("./data/mods/sourcemod_2", sourceMod.DataDir);

            sourceMod = config["sourcemod_3"];
            Assert.AreEqual("Source mod 3", sourceMod.Name);
            Assert.AreEqual("sourcemod_3_folder", sourceMod.SourceModFolder);
            Assert.AreEqual("./data/mods/sourcemod_3", sourceMod.DataDir);
        }

        [TestMethod]
        public void SourceModInstallPath_Property()
        {
            var fileSystem = new MockFileSystem();

            var sourceModsConfig = new SourceModsConfig(fileSystem, NullWriter, "C:/sourcemods.json", new JSONConfigurationSerializer<JSONSourceModsConfig>(), "123");

            Assert.AreEqual("123", sourceModsConfig.SourceModInstallPath);
        }

        [TestMethod]
        public void GetSourceModName()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                { "C:/sourcemods.json", new MockFileData(File.ReadAllBytes("../../../data/config/sourcemods.json")) },
            });

            var sourceModsConfig = new SourceModsConfig(fileSystem, NullWriter, "C:/sourcemods.json", new JSONConfigurationSerializer<JSONSourceModsConfig>(), string.Empty);

            sourceModsConfig.LoadConfig();

            Assert.AreEqual("Source mod 1", sourceModsConfig.GetSourceModName("sourcemod_1"));
            Assert.AreEqual("Source mod 2", sourceModsConfig.GetSourceModName("sourcemod_2"));
            Assert.AreEqual("Source mod 3", sourceModsConfig.GetSourceModName("sourcemod_3"));
        }

        [TestMethod]
        public void GetSourceModFolder()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                { "C:/sourcemods.json", new MockFileData(File.ReadAllBytes("../../../data/config/sourcemods.json")) },
            });

            var sourceModsConfig = new SourceModsConfig(fileSystem, NullWriter, "C:/sourcemods.json", new JSONConfigurationSerializer<JSONSourceModsConfig>(), string.Empty);

            sourceModsConfig.LoadConfig();

            Assert.AreEqual("sourcemod_1_folder", sourceModsConfig.GetSourceModFolder("sourcemod_1"));
            Assert.AreEqual("sourcemod_2_folder", sourceModsConfig.GetSourceModFolder("sourcemod_2"));
            Assert.AreEqual("sourcemod_3_folder", sourceModsConfig.GetSourceModFolder("sourcemod_3"));
        }

        [TestMethod]
        public void GetSourceModDir()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                { "C:/sourcemods.json", new MockFileData(File.ReadAllBytes("../../../data/config/sourcemods.json")) },
            });

            const string SourceModsPath = "C:/Steam/steamapps/sourcemods";

            var sourceModsConfig = new SourceModsConfig(fileSystem, NullWriter, "C:/sourcemods.json", new JSONConfigurationSerializer<JSONSourceModsConfig>(), SourceModsPath);

            sourceModsConfig.LoadConfig();

            Assert.AreEqual($"{SourceModsPath}/sourcemod_1_folder", sourceModsConfig.GetSourceModDir("sourcemod_1"));
            Assert.AreEqual($"{SourceModsPath}/sourcemod_2_folder", sourceModsConfig.GetSourceModDir("sourcemod_2"));
            Assert.AreEqual($"{SourceModsPath}/sourcemod_3_folder", sourceModsConfig.GetSourceModDir("sourcemod_3"));
        }

        [TestMethod]
        public void GetSourceModDataDir()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                { "C:/sourcemods.json", new MockFileData(File.ReadAllBytes("../../../data/config/sourcemods.json")) },
            });

            var sourceModsConfig = new SourceModsConfig(fileSystem, NullWriter, "C:/sourcemods.json", new JSONConfigurationSerializer<JSONSourceModsConfig>(), string.Empty);

            sourceModsConfig.LoadConfig();

            Assert.AreEqual("./data/mods/sourcemod_1", sourceModsConfig.GetSourceModDataDir("sourcemod_1"));
            Assert.AreEqual("./data/mods/sourcemod_2", sourceModsConfig.GetSourceModDataDir("sourcemod_2"));
            Assert.AreEqual("./data/mods/sourcemod_3", sourceModsConfig.GetSourceModDataDir("sourcemod_3"));
        }

        [TestMethod]
        public void IsSourceModInstalled()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                { "C:/sourcemods.json", new MockFileData(File.ReadAllBytes("../../../data/config/sourcemods.json")) },
                { "C:/Steam/steamapps/sourcemods/sourcemod_1_folder", new MockDirectoryData() },
                { "C:/Steam/steamapps/sourcemods/sourcemod_3_folder", new MockDirectoryData() },
            });

            const string SourceModsPath = "C:/Steam/steamapps/sourcemods";

            var sourceModsConfig = new SourceModsConfig(fileSystem, NullWriter, "C:/sourcemods.json", new JSONConfigurationSerializer<JSONSourceModsConfig>(), SourceModsPath);

            sourceModsConfig.LoadConfig();

            Assert.IsTrue(sourceModsConfig.IsSourceModInstalled("sourcemod_1"));
            Assert.IsFalse(sourceModsConfig.IsSourceModInstalled("sourcemod_2"));
            Assert.IsTrue(sourceModsConfig.IsSourceModInstalled("sourcemod_3"));
        }

        [TestMethod]
        public void GetInstalledSourceMods()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                { "C:/sourcemods.json", new MockFileData(File.ReadAllBytes("../../../data/config/sourcemods.json")) },
                { "C:/Steam/steamapps/sourcemods/sourcemod_1_folder", new MockDirectoryData() },
                { "C:/Steam/steamapps/sourcemods/sourcemod_3_folder", new MockDirectoryData() },
            });

            const string SourceModsPath = "C:/Steam/steamapps/sourcemods";

            var sourceModsConfig = new SourceModsConfig(fileSystem, NullWriter, "C:/sourcemods.json", new JSONConfigurationSerializer<JSONSourceModsConfig>(), SourceModsPath);

            sourceModsConfig.LoadConfig();

            CollectionAssert.AreEquivalent(new List<string> { "sourcemod_1", "sourcemod_3" }, sourceModsConfig.GetInstalledSourceMods());
        }

        [TestMethod]
        public void SupportedSourceModsKeys_Sorted()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                { "C:/sourcemods_unsorted.json", new MockFileData(File.ReadAllBytes("../../../data/config/sourcemods_unsorted.json")) },
            });

            var sourceModsConfig = new SourceModsConfig(fileSystem, NullWriter, "C:/sourcemods_unsorted.json", new JSONConfigurationSerializer<JSONSourceModsConfig>(), string.Empty);

            sourceModsConfig.LoadConfig();

            CollectionAssert.AreEqual(new List<string> { "sourcemod_1", "sourcemod_2", "sourcemod_3" }, sourceModsConfig.SupportedSourceModsKeys);
        }
    }
}