using Pipelines;
using System.IO.Abstractions.TestingHelpers;

namespace SourceContentInstaller.Tests
{
    [TestClass]
    public class TestInstallStepsConfig
    {
        static readonly IWriter NullWriter = new NullWriter();

        [TestMethod]
        public void Deserialize_JSONInstallStepsConfig()
        {
            var serializer = new JSONConfigurationSerializer<JSONInstallStepsConfig>();
            var deserialized = serializer.Deserialize("""
                {
                    "sdkbase2006": "./sdkbase2006.install.steps.json",
                    "sdkbase2007": "./sdkbase2007.install.steps.json",
                    "hl2": "./hl2.install.steps.json"
                }
                """);

            Assert.IsNotNull(deserialized);
            Assert.IsTrue(deserialized.ContainsKey("sdkbase2006"));
            Assert.IsTrue(deserialized.ContainsKey("sdkbase2007"));
            Assert.IsTrue(deserialized.ContainsKey("hl2"));

            Assert.AreEqual("./sdkbase2006.install.steps.json", deserialized["sdkbase2006"]);
            Assert.AreEqual("./sdkbase2007.install.steps.json", deserialized["sdkbase2007"]);
            Assert.AreEqual("./hl2.install.steps.json", deserialized["hl2"]);
        }

        [TestMethod]
        public void LoadConfig_Simple()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                { "C:/contents.install.steps.json", new MockFileData(File.ReadAllBytes("../../../data/config/contents.install.steps.json")) }
            });

            var installStepsConfig = new InstallStepsConfig(fileSystem, NullWriter, "C:/contents.install.steps.json", new JSONConfigurationSerializer<JSONInstallStepsConfig>());
            installStepsConfig.LoadConfig();

            var config = installStepsConfig.Config;
            Assert.IsNotNull(config);
            Assert.IsTrue(config.ContainsKey("sdkbase2006"));
            Assert.IsTrue(config.ContainsKey("sdkbase2007"));
            Assert.IsTrue(config.ContainsKey("hl2"));
            Assert.IsTrue(config.ContainsKey("cstrike"));

            Assert.AreEqual("C:/sdkbase2006.install.steps.json", config["sdkbase2006"]);
            Assert.AreEqual("C:/sdkbase2007.install.steps.json", config["sdkbase2007"]);
            Assert.AreEqual("C:/hl2.install.steps.json", config["hl2"]);
            Assert.AreEqual("C:/cstrike.install.steps.json", config["cstrike"]);
        }
    }
}