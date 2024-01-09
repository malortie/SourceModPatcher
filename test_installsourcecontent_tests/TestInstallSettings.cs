using test_installsourcecontent;

namespace test_installsourcecontent_tests
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
                    "install": false,
                    "install_dir": "C:/Documents/SourceContent/sdkbase2006"
                }
                """);

            Assert.IsNotNull(deserialized);
            Assert.IsFalse(deserialized.Install);
            Assert.AreEqual("C:/Documents/SourceContent/sdkbase2006", deserialized.InstallDir);
        }

        [TestMethod]
        public void Deserialize_JSONInstallSettings()
        {
            var serializer = new JSONConfigurationSerializer<JSONInstallSettings>();
            var deserialized = serializer.Deserialize("""
                {
                    "215": {
                        "install": false,
                        "install_dir": "C:/Documents/SourceContent/sdkbase2006"
                    },
                    "218": {
                        "install": true,
                        "install_dir": "C:/Documents/SourceContent/sdkbase2007"
                    },
                    "220": {
                        "install": false,
                        "install_dir": "C:/Documents/SourceContent/hl2"
                    }
                }
                """);

            Assert.IsNotNull(deserialized);
            Assert.IsTrue(deserialized.ContainsKey(215));
            Assert.IsTrue(deserialized.ContainsKey(218));
            Assert.IsTrue(deserialized.ContainsKey(220));

            Assert.IsFalse(deserialized[215].Install);
            Assert.AreEqual("C:/Documents/SourceContent/sdkbase2006", deserialized[215].InstallDir);

            Assert.IsTrue(deserialized[218].Install);
            Assert.AreEqual("C:/Documents/SourceContent/sdkbase2007", deserialized[218].InstallDir);

            Assert.IsFalse(deserialized[220].Install);
            Assert.AreEqual("C:/Documents/SourceContent/hl2", deserialized[220].InstallDir);
        }
    }
}