using test_installsourcecontent;
using test_installsourcecontent_modpatcher;

namespace test_installsourcecontent_modpatcher_tests
{
    [TestClass]
    public class TestCommonConfig
    {
        [TestMethod]
        public void Deserialize_JSONCommonConfig()
        {
            var serializer = new JSONConfigurationSerializer<JSONCommonConfig>();
            var deserialized = serializer.Deserialize("""
                {
                    "sourcemods_path": "C:/Program Files (x86)/Steam/steamapps/sourcemods",
                    "other_variable": "other value"
                }
                """);

            Assert.IsNotNull(deserialized);
            Assert.IsTrue(deserialized.ContainsKey("sourcemods_path"));
            Assert.IsTrue(deserialized.ContainsKey("other_variable"));
            Assert.AreEqual("C:/Program Files (x86)/Steam/steamapps/sourcemods", deserialized["sourcemods_path"]);
            Assert.AreEqual("other value", deserialized["other_variable"]);
        }
    }
}