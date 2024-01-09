using test_installsourcecontent;
using test_installsourcecontent_modpatcher;

namespace test_installsourcecontent_modpatcher_tests
{
    [TestClass]
    public class TestSourceModsConfig
    {
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
    }
}