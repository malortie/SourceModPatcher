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
                    "215": "./215.install.steps.json",
                    "218": "./218.install.steps.json",
                    "220": "./220.install.steps.json"
                }
                """);

            Assert.IsNotNull(deserialized);
            Assert.IsTrue(deserialized.ContainsKey(215));
            Assert.IsTrue(deserialized.ContainsKey(218));
            Assert.IsTrue(deserialized.ContainsKey(220));

            Assert.AreEqual("./215.install.steps.json", deserialized[215]);
            Assert.AreEqual("./218.install.steps.json", deserialized[218]);
            Assert.AreEqual("./220.install.steps.json", deserialized[220]);
        }

        [TestMethod]
        public void LoadConfig_Simple()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                { "C:/steamapps.install.steps.json", new MockFileData(File.ReadAllBytes("../../../data/config/steamapps.install.steps.json")) }
            });

            var installStepsConfig = new InstallStepsConfig(fileSystem, NullWriter, "C:/steamapps.install.steps.json", new JSONConfigurationSerializer<JSONInstallStepsConfig>());
            installStepsConfig.LoadConfig();

            var config = installStepsConfig.Config;
            Assert.IsNotNull(config);
            Assert.IsTrue(config.ContainsKey(215));
            Assert.IsTrue(config.ContainsKey(218));
            Assert.IsTrue(config.ContainsKey(220));
            Assert.IsTrue(config.ContainsKey(240));

            Assert.AreEqual("C:/215.install.steps.json", config[215]);
            Assert.AreEqual("C:/218.install.steps.json", config[218]);
            Assert.AreEqual("C:/220.install.steps.json", config[220]);
            Assert.AreEqual("C:/240.install.steps.json", config[240]);
        }
    }
}