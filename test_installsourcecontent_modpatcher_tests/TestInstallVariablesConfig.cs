using test_installsourcecontent;
using test_installsourcecontent_modpatcher;

namespace test_installsourcecontent_modpatcher_tests
{
    [TestClass]
    public class TestInstallVariablesConfig
    {
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
    }
}