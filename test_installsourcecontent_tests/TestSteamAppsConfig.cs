using test_installsourcecontent;

namespace test_installsourcecontent_tests
{
    [TestClass]
    public class TestSteamAppsConfig
    {
        [TestMethod]
        public void Deserialize_JSONSteamAppsConfigEntry()
        {
            var serializer = new JSONConfigurationSerializer<JSONSteamAppsConfigEntry>();
            var deserialized = serializer.Deserialize("""
                {
                    "name": "Source SDK Base 2006",
                    "appmanifest_file": "appmanifest_215.acf",
                    "install_dir": "C:/Program Files (x86)/Steam/steamapps/common/Source SDK Base"
                }
                """);

            Assert.IsNotNull(deserialized);
            Assert.AreEqual("Source SDK Base 2006", deserialized.Name);
            Assert.AreEqual("appmanifest_215.acf", deserialized.AppManifestFile);
            Assert.AreEqual("C:/Program Files (x86)/Steam/steamapps/common/Source SDK Base", deserialized.InstallDir);
        }

        [TestMethod]
        public void Deserialize_JSONSteamAppsConfig()
        {
            var serializer = new JSONConfigurationSerializer<JSONSteamAppsConfig>();
            var deserialized = serializer.Deserialize("""
                {
                    "215": 
                    {
                        "name": "Source SDK Base 2006",
                        "appmanifest_file": "appmanifest_215.acf",
                        "install_dir": "C:/Program Files (x86)/Steam/steamapps/common/Source SDK Base"
                    },
                    "218": 
                    {
                        "name": "Source SDK Base 2007",
                        "appmanifest_file": "appmanifest_218.acf",
                        "install_dir": "C:/Program Files (x86)/Steam/steamapps/common/Source SDK Base 2007"
                    },
                    "220": 
                    {
                        "name": "Half-Life 2",
                        "appmanifest_file": "appmanifest_220.acf",
                        "install_dir": "C:/Program Files (x86)/Steam/steamapps/common/Half-Life 2"
                    }
                }
                """);

            Assert.IsNotNull(deserialized);
            Assert.IsTrue(deserialized.ContainsKey(215));
            Assert.IsTrue(deserialized.ContainsKey(218));
            Assert.IsTrue(deserialized.ContainsKey(220));

            var steamApp = deserialized[215];
            Assert.AreEqual("Source SDK Base 2006", steamApp.Name);
            Assert.AreEqual("appmanifest_215.acf", steamApp.AppManifestFile);
            Assert.AreEqual("C:/Program Files (x86)/Steam/steamapps/common/Source SDK Base", steamApp.InstallDir);

            steamApp = deserialized[218];
            Assert.AreEqual("Source SDK Base 2007", steamApp.Name);
            Assert.AreEqual("appmanifest_218.acf", steamApp.AppManifestFile);
            Assert.AreEqual("C:/Program Files (x86)/Steam/steamapps/common/Source SDK Base 2007", steamApp.InstallDir);

            steamApp = deserialized[220];
            Assert.AreEqual("Half-Life 2", steamApp.Name);
            Assert.AreEqual("appmanifest_220.acf", steamApp.AppManifestFile);
            Assert.AreEqual("C:/Program Files (x86)/Steam/steamapps/common/Half-Life 2", steamApp.InstallDir);
        }
    }
}