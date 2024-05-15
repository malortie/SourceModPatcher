using Pipelines;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Text.Json.Serialization;

namespace SourceContentInstaller.Tests
{
    public class JSONSimpleTestConfig
    {
        [JsonPropertyName("propBool")]
        public bool PropBool { get; set; } = false;
        [JsonPropertyName("propInt")]
        public int PropInt { get; set; } = 0;
        [JsonPropertyName("propString")]
        public string PropString { get; set; } = string.Empty;
        [JsonPropertyName("propIntList")]
        public List<int> PropIntList { get; set; } = [];
        [JsonPropertyName("propObject")]
        public Dictionary<string, string> PropObject { get; set; } = [];
    }

    public class NullConfigurationSerializer<ConfigT> : IConfigurationSerializer<ConfigT>
    {
        public ConfigT? Deserialize(string value)
        {
            return default;
        }

        public string Serialize(ConfigT value)
        {
            return string.Empty;
        }
    }

    public class ConfigurationSerializerMock<ConfigT> : IConfigurationSerializer<ConfigT> where ConfigT : new()
    {
        public ConfigT? Deserialize(string value)
        {
            return new ConfigT();
        }

        public string Serialize(ConfigT value)
        {
            return string.Empty;
        }
    }

    public class ConfigurationManagerMock<ConfigT>(IFileSystem fileSystem, IWriter writer, string filePath, IConfigurationSerializer<ConfigT> configSerializer) : ConfigurationManager<ConfigT>(fileSystem, writer, filePath, configSerializer) where ConfigT : new()
    {
        public int PostLoadConfigTotal { get; set; } = 0;

        public new ConfigT Config { get { return base.Config; } set { base.Config = value; } }

        protected override void PostLoadConfig()
        {
            ++PostLoadConfigTotal;
        }
    }

    [TestClass]
    public class TestConfigurationManager
    {
        static readonly IWriter NullWriter = new NullWriter();

        [TestMethod]
        public void FileName_ReturnsFileNameWithoutPath()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });

            var configManager = new ConfigurationManager<object>(fileSystem, NullWriter, "C:/Documents/simple_test.json", new NullConfigurationSerializer<object>());

            Assert.AreEqual("simple_test.json", configManager.GetFileName());
        }

        [TestMethod]
        public void LoadConfig_Simple()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                 { "C:/simple_test.json", new MockFileData(File.ReadAllBytes("../../../data/config/simple_test.json")) }
            });

            var configManager = new ConfigurationManager<JSONSimpleTestConfig>(fileSystem, NullWriter, "C:/simple_test.json", new JSONConfigurationSerializer<JSONSimpleTestConfig>());

            configManager.LoadConfig();
            var config = configManager.Config;

            Assert.IsNotNull(config);
            Assert.IsTrue(config.PropBool);
            Assert.AreEqual(42, config.PropInt);
            Assert.AreEqual("value1", config.PropString);
            CollectionAssert.AreEquivalent(new List<int> { 1, 2, 3, 4 }, config.PropIntList);
            Assert.IsNotNull(config.PropObject);
            Assert.IsTrue(config.PropObject.ContainsKey("hello"));
            Assert.AreEqual("world", config.PropObject["hello"]);
        }

        [TestMethod]
        public void LoadConfig_ThrowException_WhenDeserializedDataIsNull()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                 { "C:/simple_test.json", new MockFileData(File.ReadAllBytes("../../../data/config/simple_test.json")) }
            });

            var configManager = new ConfigurationManager<object>(fileSystem, NullWriter, "C:/simple_test.json", new NullConfigurationSerializer<object>());

            Assert.ThrowsException<Exception>(() => configManager.LoadConfig());
        }

        [TestMethod]
        public void LoadConfig_Calls_PostLoadConfig_WhenSuccessful()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                 { "C:/simple_test.json", new MockFileData("") }
            });

            var configManager = new ConfigurationManagerMock<object>(fileSystem, NullWriter, "C:/simple_test.json", new ConfigurationSerializerMock<object>());

            configManager.LoadConfig();

            Assert.AreEqual(1, configManager.PostLoadConfigTotal);
        }

        [TestMethod]
        public void SaveConfig_Simple()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });

            var configManager = new ConfigurationManagerMock<JSONSimpleTestConfig>(fileSystem, NullWriter, "C:/simple_test.json", new JSONConfigurationSerializer<JSONSimpleTestConfig>())
            {
                Config = new JSONSimpleTestConfig
                {
                    PropBool = false,
                    PropInt = 100,
                    PropString = "test",
                    PropIntList = [5, 6, 7, 8],
                    PropObject = {
                    { "key", "value" }
                }
                }
            };

            configManager.SaveConfig();

            const string expected = """
                {
                  "propBool": false,
                  "propInt": 100,
                  "propString": "test",
                  "propIntList": [
                    5,
                    6,
                    7,
                    8
                  ],
                  "propObject": {
                    "key": "value"
                  }
                }
                """;

            Assert.AreEqual(expected, fileSystem.File.ReadAllText("C:/simple_test.json"));
        }
    }
}