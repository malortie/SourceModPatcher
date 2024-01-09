using test_installsourcecontent;
using JSONInstallSettingsEntry = test_installsourcecontent_modpatcher.JSONInstallSettingsEntry;
using JSONInstallSettings = test_installsourcecontent_modpatcher.JSONInstallSettings;

namespace test_installsourcecontent_modpatcher_tests
{
    [TestClass]
    public class TestInstallSettings
    {
        [TestMethod]
        public void Deserialize_JSONInstallSettingsEntry()
        {
            var serializer = new JSONConfigurationSerializer<JSONInstallSettingsEntry>();
            var deserialized = serializer.Deserialize("""
                {
                    "install": false
                }
                """);

            Assert.IsNotNull(deserialized);
            Assert.IsFalse(deserialized.Install);
        }

        [TestMethod]
        public void Deserialize_JSONInstallSettings()
        {
            var serializer = new JSONConfigurationSerializer<JSONInstallSettings>();
            var deserialized = serializer.Deserialize("""
                {
                    "sourcemod_1": {
                        "install": false
                    },
                    "sourcemod_2": {
                        "install": true
                    },
                    "sourcemod_3": {
                        "install": false
                    }
                }
                """);

            Assert.IsNotNull(deserialized);
            Assert.IsTrue(deserialized.ContainsKey("sourcemod_1"));
            Assert.IsTrue(deserialized.ContainsKey("sourcemod_2"));
            Assert.IsTrue(deserialized.ContainsKey("sourcemod_3"));

            Assert.IsFalse(deserialized["sourcemod_1"].Install);
            Assert.IsTrue(deserialized["sourcemod_2"].Install);
            Assert.IsFalse(deserialized["sourcemod_3"].Install);
        }
    }
}