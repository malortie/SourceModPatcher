using Pipelines;
using SourceContentInstaller;
using System.IO.Abstractions.TestingHelpers;

namespace SourceModPatcher.Tests
{
    [TestClass]
    public class TestInstallVariablesConfig
    {
        static IWriter NullWriter = new NullWriter();

        [TestMethod]
        public void Deserialize_JSONInstallVariablesConfig()
        {
            var serializer = new JSONConfigurationSerializer<JSONInstallVariablesConfig>();
            var deserialized = serializer.Deserialize("""
                {
                    "data_dir": "./data",
                    "data_mods_dir": "./data/mods",
                    "data_sdks_dir": "./data/sdks"
                }
                """);

            Assert.IsNotNull(deserialized);
            Assert.IsTrue(deserialized.ContainsKey("data_dir"));
            Assert.IsTrue(deserialized.ContainsKey("data_mods_dir"));
            Assert.IsTrue(deserialized.ContainsKey("data_mods_dir"));
            Assert.AreEqual("./data", deserialized["data_dir"]);
            Assert.AreEqual("./data/mods", deserialized["data_mods_dir"]);
            Assert.AreEqual("./data/sdks", deserialized["data_sdks_dir"]);
        }

        [TestMethod]
        public void LoadConfig_Simple()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                { "C:/sourcemods.install.variables.json", new MockFileData(File.ReadAllBytes("../../../data/config/sourcemods.install.variables.json")) }
            });

            var installStepsConfig = new InstallVariablesConfig(fileSystem, NullWriter, "C:/sourcemods.install.variables.json", new JSONConfigurationSerializer<JSONInstallVariablesConfig>());
            installStepsConfig.LoadConfig();

            var config = installStepsConfig.Config;
            Assert.IsNotNull(config);
            Assert.IsTrue(config.ContainsKey("data_dir"));
            Assert.IsTrue(config.ContainsKey("data_mods_dir"));
            Assert.IsTrue(config.ContainsKey("data_sdks_dir"));

            Assert.AreEqual("C:/data", config["data_dir"]);
            Assert.AreEqual("C:/data/mods", config["data_mods_dir"]);
            Assert.AreEqual("C:/data/sdks", config["data_sdks_dir"]);
        }
    }
}