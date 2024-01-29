using Pipelines;
using SourceContentInstaller;
using System.IO.Abstractions.TestingHelpers;

namespace SourceModPatcher.Tests
{
    [TestClass]
    public class TestCommonConfig
    {
        static readonly IWriter NullWriter = new NullWriter();

        [TestMethod]
        public void Deserialize_JSONCommonConfig()
        {
            var serializer = new JSONConfigurationSerializer<JSONCommonConfig>();
            var deserialized = serializer.Deserialize("""
                {
                    "sourcemods_path": "C:/Program Files (x86)/Steam/steamapps/sourcemods",
                    "other_variable": "other value"
                }
                """);

            Assert.IsNotNull(deserialized);
            Assert.IsTrue(deserialized.ContainsKey("sourcemods_path"));
            Assert.IsTrue(deserialized.ContainsKey("other_variable"));
            Assert.AreEqual("C:/Program Files (x86)/Steam/steamapps/sourcemods", deserialized["sourcemods_path"]);
            Assert.AreEqual("other value", deserialized["other_variable"]);
        }

        [TestMethod]
        public void LoadConfig_Simple()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                { "C:/sourcemods.common.json", new MockFileData(File.ReadAllBytes("../../../data/config/sourcemods.common.json")) }
            });

            var commonConfig = new CommonConfig(fileSystem, NullWriter, "C:/sourcemods.common.json", new JSONConfigurationSerializer<JSONCommonConfig>());
            commonConfig.LoadConfig();

            var config = commonConfig.Config;
            Assert.IsNotNull(config);
            Assert.IsTrue(config.ContainsKey("sourcemods_path"));
            Assert.IsTrue(config.ContainsKey("other_variable"));
            Assert.AreEqual("C:/sourcemods", config["sourcemods_path"]);
            Assert.AreEqual("other value", config["other_variable"]);
        }

        [TestMethod]
        public void GetSourceModsPath()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                { "C:/sourcemods.common.json", new MockFileData(File.ReadAllBytes("../../../data/config/sourcemods.common.json")) }
            });

            var commonConfig = new CommonConfig(fileSystem, NullWriter, "C:/sourcemods.common.json", new JSONConfigurationSerializer<JSONCommonConfig>());
            commonConfig.LoadConfig();

            Assert.AreEqual("C:/sourcemods", commonConfig.GetSourceModsPath());
        }
    }
}