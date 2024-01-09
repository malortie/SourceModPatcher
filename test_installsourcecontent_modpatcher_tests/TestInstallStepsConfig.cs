using test_installsourcecontent;
using JSONInstallStepsConfig = test_installsourcecontent_modpatcher.JSONInstallStepsConfig;

namespace test_installsourcecontent_modpatcher_tests
{
    [TestClass]
    public class TestInstallStepsConfig
    {
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
    }
}