using Pipelines;
using System.IO.Abstractions.TestingHelpers;

namespace SourceContentInstaller.Tests
{
    [TestClass]
    public class TestVariablesConfig
    {
        static readonly IWriter NullWriter = new NullWriter();

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

        [TestMethod]
        public void LoadConfig_Simple()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                { "C:/variables.json", new MockFileData(File.ReadAllBytes("../../../data/config/variables.json")) }
            });

            var variablesConfig = new VariablesConfig(fileSystem, NullWriter, "C:/variables.json", new JSONConfigurationSerializer<JSONVariablesConfig>()) { PurgeVariablesList = false };

            variablesConfig.LoadConfig();

            var config = variablesConfig.Config;
            Assert.IsNotNull(config);
            Assert.IsTrue(config.ContainsKey("hl2_content_path"));
            Assert.IsTrue(config.ContainsKey("sdkbase2006_content_path"));
            Assert.IsTrue(config.ContainsKey("sdkbase2007_content_path"));
            Assert.AreEqual("C:/Half-Life 2", config["hl2_content_path"]);
            Assert.AreEqual("C:/Source SDK Base", config["sdkbase2006_content_path"]);
            Assert.AreEqual("C:/Source SDK Base 2007", config["sdkbase2007_content_path"]);
        }

        [TestMethod]
        public void LoadConfig_CreateFileIfItDoesNotExist()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {});

            var variablesConfig = new VariablesConfig(fileSystem, NullWriter, "C:/variables.json", new JSONConfigurationSerializer<JSONVariablesConfig>()) { PurgeVariablesList = false };

            Assert.IsFalse(fileSystem.File.Exists("C:/variables.json"));

            variablesConfig.LoadConfig();

            Assert.IsTrue(fileSystem.File.Exists("C:/variables.json"));
            Assert.IsNotNull(variablesConfig.Config);
        }

        [TestMethod]
        public void PurgeVariablesList_When_PurgeVariablesList_Is_True()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                { "C:/Half-Life 2", new MockDirectoryData() },
                { "C:/variables.json", new MockFileData(File.ReadAllBytes("../../../data/config/variables.json")) }
            });

            var variablesConfig = new VariablesConfig(fileSystem, NullWriter, "C:/variables.json", new JSONConfigurationSerializer<JSONVariablesConfig>()) { PurgeVariablesList = true };

            variablesConfig.LoadConfig();

            var config = variablesConfig.Config;
            Assert.IsNotNull(config);
            Assert.IsTrue(config.ContainsKey("hl2_content_path"));
            Assert.IsFalse(config.ContainsKey("sdkbase2006_content_path"));
            Assert.IsFalse(config.ContainsKey("sdkbase2007_content_path"));
            Assert.AreEqual("C:/Half-Life 2", config["hl2_content_path"]);
        }

        [TestMethod]
        public void SaveConfig_EmptyWhenNoVariables()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });

            var variablesConfig = new VariablesConfig(fileSystem, NullWriter, "C:/variables.json", new JSONConfigurationSerializer<JSONVariablesConfig>()) { PurgeVariablesList = false };

            variablesConfig.SaveConfig();

            Assert.AreEqual("{}", fileSystem.File.ReadAllText("C:/variables.json"));
        }

        [TestMethod]
        public void SaveVariable_Simple()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });

            var variablesConfig = new VariablesConfig(fileSystem, NullWriter, "C:/variables.json", new JSONConfigurationSerializer<JSONVariablesConfig>()) { PurgeVariablesList = false };

            variablesConfig.SaveVariable("hl2_content_path", "C:/Half-Life 2");
            variablesConfig.SaveVariable("sdkbase2006_content_path", "C:/Source SDK Base");
            variablesConfig.SaveVariable("sdkbase2007_content_path", "C:/Source SDK Base 2007");

            const string expected = """
                {
                  "hl2_content_path": "C:/Half-Life 2",
                  "sdkbase2006_content_path": "C:/Source SDK Base",
                  "sdkbase2007_content_path": "C:/Source SDK Base 2007"
                }
                """;
            Assert.AreEqual(expected, fileSystem.File.ReadAllText("C:/variables.json"));
        }

        [TestMethod]
        public void SaveVariable_DoesNotEraseAlreadyExistingVariables()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                { "C:/variables.json", new MockFileData(File.ReadAllBytes("../../../data/config/variables.json")) }
            });

            var variablesConfig = new VariablesConfig(fileSystem, NullWriter, "C:/variables.json", new JSONConfigurationSerializer<JSONVariablesConfig>()) { PurgeVariablesList = false };

            variablesConfig.LoadConfig();

            var config = variablesConfig.Config;
            Assert.AreEqual("C:/Half-Life 2", config["hl2_content_path"]);
            Assert.AreEqual("C:/Source SDK Base", config["sdkbase2006_content_path"]);
            Assert.AreEqual("C:/Source SDK Base 2007", config["sdkbase2007_content_path"]);

            variablesConfig.SaveVariable("test", "1234");

            variablesConfig.LoadConfig();

            Assert.AreEqual("C:/Half-Life 2", config["hl2_content_path"]);
            Assert.AreEqual("C:/Source SDK Base", config["sdkbase2006_content_path"]);
            Assert.AreEqual("C:/Source SDK Base 2007", config["sdkbase2007_content_path"]);
            Assert.AreEqual("1234", config["test"]);
        }

        [TestMethod]
        public void SaveVariables_Simple()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });

            var variablesConfig = new VariablesConfig(fileSystem, NullWriter, "C:/variables.json", new JSONConfigurationSerializer<JSONVariablesConfig>()) { PurgeVariablesList = false };

            variablesConfig.SaveVariables(new() {
                { "hl2_content_path", "C:/Half-Life 2" },
                { "sdkbase2006_content_path", "C:/Source SDK Base" },
                { "sdkbase2007_content_path", "C:/Source SDK Base 2007" }
            });
            const string expected = """
                {
                  "hl2_content_path": "C:/Half-Life 2",
                  "sdkbase2006_content_path": "C:/Source SDK Base",
                  "sdkbase2007_content_path": "C:/Source SDK Base 2007"
                }
                """;
            Assert.AreEqual(expected, fileSystem.File.ReadAllText("C:/variables.json"));
        }

        [TestMethod]
        public void IsVariablePresentAndValid_WhenVariableIsUndefined_ReturnsFalse()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                { "C:/variables_invalid.json", new MockFileData(File.ReadAllBytes("../../../data/config/variables_invalid.json")) }
            });

            var variablesConfig = new VariablesConfig(fileSystem, NullWriter, "C:/variables_invalid.json", new JSONConfigurationSerializer<JSONVariablesConfig>()) { PurgeVariablesList = false };

            variablesConfig.LoadConfig();

            Assert.IsFalse(variablesConfig.IsVariablePresentAndValid("test"));
        }

        [TestMethod]
        public void IsVariablePresentAndValid_WhenVariableExistsButIsEmpty_ReturnsFalse()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                { "C:/variables_invalid.json", new MockFileData(File.ReadAllBytes("../../../data/config/variables_invalid.json")) }
            });

            var variablesConfig = new VariablesConfig(fileSystem, NullWriter, "C:/variables_invalid.json", new JSONConfigurationSerializer<JSONVariablesConfig>()) { PurgeVariablesList = false };

            variablesConfig.LoadConfig();

            Assert.IsFalse(variablesConfig.IsVariablePresentAndValid("empty_path"));
        }

        [TestMethod]
        public void IsVariablePresentAndValid_WhenVariableExistsButPathDoesNotExist_ReturnsFalse()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                { "C:/variables_invalid.json", new MockFileData(File.ReadAllBytes("../../../data/config/variables_invalid.json")) }
            });

            var variablesConfig = new VariablesConfig(fileSystem, NullWriter, "C:/variables_invalid.json", new JSONConfigurationSerializer<JSONVariablesConfig>()) { PurgeVariablesList = false };

            variablesConfig.LoadConfig();

            Assert.IsFalse(variablesConfig.IsVariablePresentAndValid("inexistant_path"));
        }

        [TestMethod]
        public void IsVariablePresentAndValid_WhenVariableExistsAndPathExists_ReturnsTrue()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                { "C:/Half-Life 2", new MockDirectoryData() },
                { "C:/variables.json", new MockFileData(File.ReadAllBytes("../../../data/config/variables.json")) }
            });

            var variablesConfig = new VariablesConfig(fileSystem, NullWriter, "C:/variables.json", new JSONConfigurationSerializer<JSONVariablesConfig>()) { PurgeVariablesList = false };

            variablesConfig.LoadConfig();

            Assert.IsTrue(variablesConfig.IsVariablePresentAndValid("hl2_content_path"));
        }
    }
}