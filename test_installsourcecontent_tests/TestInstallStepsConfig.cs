using test_installsourcecontent;

namespace test_installsourcecontent_tests
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
    }
}