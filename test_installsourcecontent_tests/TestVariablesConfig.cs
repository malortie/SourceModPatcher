using test_installsourcecontent;

namespace test_installsourcecontent_tests
{
    [TestClass]
    public class TestVariablesConfig
    {
        [TestMethod]
        public void Serialize_JSONVariablesConfig()
        {
            const string expected = "{\r\n  \"hl2_content_path\": \"C:/Documents/SourceContent/hl2\",\r\n  \"sdkbase2006_content_path\": \"C:/Documents/SourceContent/sdkbase2006\",\r\n  \"sdkbase2007_content_path\": \"C:/Documents/SourceContent/sdkbase2007\"\r\n}";

            var serializer = new JSONConfigurationSerializer<JSONVariablesConfig>();
            var serialized = serializer.Serialize(new JSONVariablesConfig {
                { "hl2_content_path", "C:/Documents/SourceContent/hl2" },
                { "sdkbase2006_content_path", "C:/Documents/SourceContent/sdkbase2006" },
                { "sdkbase2007_content_path", "C:/Documents/SourceContent/sdkbase2007" },
            });

            Assert.AreEqual(expected, serialized);
        }

        [TestMethod]
        public void Deserialize_JSONVariablesConfig()
        {
            var serializer = new JSONConfigurationSerializer<JSONVariablesConfig>();
            var deserialized = serializer.Deserialize("""
                {
                    "hl2_content_path": "C:/Documents/SourceContent/hl2",
                    "sdkbase2006_content_path": "C:/Documents/SourceContent/sdkbase2006",
                    "sdkbase2007_content_path": "C:/Documents/SourceContent/sdkbase2007"
                }
                """);

            Assert.IsNotNull(deserialized);
            Assert.IsTrue(deserialized.ContainsKey("hl2_content_path"));
            Assert.IsTrue(deserialized.ContainsKey("sdkbase2006_content_path"));
            Assert.IsTrue(deserialized.ContainsKey("sdkbase2007_content_path"));
            Assert.AreEqual("C:/Documents/SourceContent/hl2", deserialized["hl2_content_path"]);
            Assert.AreEqual("C:/Documents/SourceContent/sdkbase2006", deserialized["sdkbase2006_content_path"]);
            Assert.AreEqual("C:/Documents/SourceContent/sdkbase2007", deserialized["sdkbase2007_content_path"]);
        }
    }
}