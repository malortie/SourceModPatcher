using test_installsourcecontent;
using JSONInstallStepsConfig = test_installsourcecontent_modpatcher.JSONInstallStepsConfig;
using InstallStepsConfig = test_installsourcecontent_modpatcher.InstallStepsConfig;
using Pipelines;
using System.IO.Abstractions.TestingHelpers;

namespace test_installsourcecontent_modpatcher_tests
{
    [TestClass]
    public class TestInstallStepsConfig
    {
        static IWriter NullWriter = new NullWriter();

        [TestMethod]
        public void Deserialize_JSONInstallStepsConfig()
        {
            var serializer = new JSONConfigurationSerializer<JSONInstallStepsConfig>();
            var deserialized = serializer.Deserialize("""
            {
                "sourcemod_1": "./sourcemod_1.install.steps.json",
                "sourcemod_2": "./sourcemod_2.install.steps.json",
                "sourcemod_3": "./sourcemod_3.install.steps.json"
            }
            """);

            Assert.IsNotNull(deserialized);
            Assert.IsTrue(deserialized.ContainsKey("sourcemod_1"));
            Assert.IsTrue(deserialized.ContainsKey("sourcemod_2"));
            Assert.IsTrue(deserialized.ContainsKey("sourcemod_3"));

            Assert.AreEqual("./sourcemod_1.install.steps.json", deserialized["sourcemod_1"]);
            Assert.AreEqual("./sourcemod_2.install.steps.json", deserialized["sourcemod_2"]);
            Assert.AreEqual("./sourcemod_3.install.steps.json", deserialized["sourcemod_3"]);
        }

        [TestMethod]
        public void LoadConfig_Simple()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                { "C:/sourcemods.install.steps.json", new MockFileData(File.ReadAllBytes("../../../data/config/sourcemods.install.steps.json")) }
            });

            var installStepsConfig = new InstallStepsConfig(fileSystem, NullWriter, "C:/sourcemods.install.steps.json", new JSONConfigurationSerializer<JSONInstallStepsConfig>());
            installStepsConfig.LoadConfig();

            var config = installStepsConfig.Config;
            Assert.IsNotNull(config);
            Assert.IsTrue(config.ContainsKey("sourcemod_1"));
            Assert.IsTrue(config.ContainsKey("sourcemod_2"));
            Assert.IsTrue(config.ContainsKey("sourcemod_3"));

            Assert.AreEqual("C:/sourcemod_1.install.steps.json", config["sourcemod_1"]);
            Assert.AreEqual("C:/sourcemod_2.install.steps.json", config["sourcemod_2"]);
            Assert.AreEqual("C:/sourcemod_3.install.steps.json", config["sourcemod_3"]);
        }
    }
}