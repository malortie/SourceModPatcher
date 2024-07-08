using Pipelines;
using SourceContentInstaller;
using System;
using System.Collections.ObjectModel;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Text.Json.Serialization;

namespace SourceModPatcher.Tests
{
    public class MockContent
    {
        public List<string> OutputVariables { get; set; } = [];
    }

    public class MockSourceModConfigDependencies
    {
        public List<List<string>> Required { get; set; } = [];
        public List<List<string>>? Optional { get; set; } = [];
    }

    public class MockSourceModConfigEntry
    {
        public MockSourceModConfigDependencies Dependencies { get; set; } = new();
    }

    public class MockDependencyValidationConfiguration : IConfiguration
    {
        public Dictionary<string, string> Variables { get; set; } = [];
        public Dictionary<string, MockContent> Contents { get; set; } = [];
        public Dictionary<string, MockSourceModConfigEntry> SourceMods { get; set; } = [];

        public ReadOnlyDictionary<string, string> GetInstallVariables()
        {
            return new ReadOnlyDictionary<string, string>(new Dictionary<string, string>());
        }

        public string GetSourceModDataDir(string key)
        {
            return string.Empty;
        }

        public string GetSourceModDir(string key)
        {
            return string.Empty;
        }

        public string GetSourceModFolder(string key)
        {
            return string.Empty;
        }

        public string GetSourceModName(string key)
        {
            return string.Empty;
        }

        public ReadOnlyDictionary<string, string> GetVariables()
        {
            return new(Variables);
        }

        public string GetVariablesFileName()
        {
            return string.Empty;
        }

        public List<List<string>> GetRequiredContentDependencies(string sourcemodID)
        {
            return SourceMods[sourcemodID].Dependencies.Required;
        }

        public List<List<string>> GetOptionalContentDependencies(string sourcemodID)
        {
            return SourceMods[sourcemodID].Dependencies.Optional ?? [];
        }

        public string GetContentName(string contentID)
        {
            return string.Empty;
        }

        public List<string> GetContentOutputVariables(string contentID)
        {
            return Contents[contentID].OutputVariables;
        }
    }

    [TestClass]
    public class TestDependencyValidation
    {
        [TestMethod]
        public void Validate_Single_NoMissingVariable_ShouldBeFulfilled()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var configuration = new MockDependencyValidationConfiguration
            {
                Variables = new Dictionary<string, string> {
                    { "content_1_output_1", "value1" },
                    { "content_1_output_2", "value2" }
                },
                Contents = new Dictionary<string, MockContent> {
                    { "content_1", new MockContent { OutputVariables = ["content_1_output_1", "content_1_output_2"]  } },
                }
            };

            var validator = new DependencyValidation();
            var result = validator.Validate(new Context(fileSystem, configuration), ["content_1"]);

            Assert.IsTrue(result.FulFilled);
            Assert.AreEqual(0, result.EquivalentContent.First().MissingVariables.Count());
        }

        public void Validate_Single_OneMissingVariable_ShouldNotBeFulfilled()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var configuration = new MockDependencyValidationConfiguration
            {
                Variables = new Dictionary<string, string> {
                    { "content_1_output_1", "value1" }
                },
                Contents = new Dictionary<string, MockContent> {
                    { "content_1", new MockContent { OutputVariables = ["content_1_output_1", "content_1_output_2"]  } },
                }
            };

            var validator = new DependencyValidation();
            var result = validator.Validate(new Context(fileSystem, configuration), ["content_1"]);

            Assert.IsFalse(result.FulFilled);
            Assert.AreEqual(1, result.EquivalentContent.First().MissingVariables.Count());
            Assert.AreEqual("content_1_output_2", result.EquivalentContent.First().MissingVariables.First());
        }

        public void Validate_Single_AllMissingVariables_ShouldNotBeFulfilled()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var configuration = new MockDependencyValidationConfiguration
            {
                Variables = [],
                Contents = new Dictionary<string, MockContent> {
                    { "content_1", new MockContent { OutputVariables = ["content_1_output_1", "content_1_output_2"]  } },
                }
            };

            var validator = new DependencyValidation();
            var result = validator.Validate(new Context(fileSystem, configuration), ["content_1"]);

            Assert.IsFalse(result.FulFilled);
            Assert.AreEqual(2, result.EquivalentContent.First().MissingVariables.Count());
            CollectionAssert.AreEquivalent(new []{ "content_1_output_1", "content_1_output_2" }, result.EquivalentContent.First().MissingVariables);
        }

        [TestMethod]
        public void Validate_Multi_1_NoMissingVariable_2_NoMissingVariable_ShouldBeFulfilled()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var configuration = new MockDependencyValidationConfiguration
            {
                Variables = new Dictionary<string, string> {
                    { "content_1_output_1", "value1" },
                    { "content_1_output_2", "value2" },
                    { "content_2_output_1", "value3" },
                    { "content_2_output_2", "value4" },
                },
                Contents = new Dictionary<string, MockContent> {
                    { "content_1", new MockContent { OutputVariables = ["content_1_output_1", "content_1_output_2"]  } },
                    { "content_2", new MockContent { OutputVariables = ["content_2_output_1", "content_2_output_2"]  } },
                }
            };

            var validator = new DependencyValidation();
            var result = validator.Validate(new Context(fileSystem, configuration), ["content_1", "content_2"]);

            Assert.IsTrue(result.FulFilled);
            Assert.AreEqual(0, result.EquivalentContent[0].MissingVariables.Count());
            Assert.AreEqual(0, result.EquivalentContent[1].MissingVariables.Count());
        }

        [TestMethod]
        public void Validate_Multi_1_NoMissingVariable_2_OneMissingVariable_ShouldBeFulfilled()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var configuration = new MockDependencyValidationConfiguration
            {
                Variables = new Dictionary<string, string> {
                    { "content_1_output_1", "value1" },
                    { "content_1_output_2", "value2" },
                    { "content_2_output_1", "value3" },
                },
                Contents = new Dictionary<string, MockContent> {
                    { "content_1", new MockContent { OutputVariables = ["content_1_output_1", "content_1_output_2"]  } },
                    { "content_2", new MockContent { OutputVariables = ["content_2_output_1", "content_2_output_2"]  } },
                }
            };

            var validator = new DependencyValidation();
            var result = validator.Validate(new Context(fileSystem, configuration), ["content_1", "content_2"]);

            Assert.IsTrue(result.FulFilled);
            Assert.AreEqual(0, result.EquivalentContent[0].MissingVariables.Count());
            Assert.AreEqual(1, result.EquivalentContent[1].MissingVariables.Count());
            Assert.AreEqual("content_2_output_2", result.EquivalentContent[1].MissingVariables.First());
        }

        [TestMethod]
        public void Validate_Multi_1_NoMissingVariable_2_AllMissingVariables_ShouldBeFulfilled()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var configuration = new MockDependencyValidationConfiguration
            {
                Variables = new Dictionary<string, string> {
                    { "content_1_output_1", "value1" },
                    { "content_1_output_2", "value2" }
                },
                Contents = new Dictionary<string, MockContent> {
                    { "content_1", new MockContent { OutputVariables = ["content_1_output_1", "content_1_output_2"]  } },
                    { "content_2", new MockContent { OutputVariables = ["content_2_output_1", "content_2_output_2"]  } },
                }
            };

            var validator = new DependencyValidation();
            var result = validator.Validate(new Context(fileSystem, configuration), ["content_1", "content_2"]);

            Assert.IsTrue(result.FulFilled);
            Assert.AreEqual(0, result.EquivalentContent[0].MissingVariables.Count());
            Assert.AreEqual(2, result.EquivalentContent[1].MissingVariables.Count());
            CollectionAssert.AreEquivalent(new[] { "content_2_output_1", "content_2_output_2" }, result.EquivalentContent[1].MissingVariables);
        }

        [TestMethod]
        public void Validate_Multi_1_OneMissingVariable_2_NoMissingVariable_ShouldBeFulfilled()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var configuration = new MockDependencyValidationConfiguration
            {
                Variables = new Dictionary<string, string> {
                    { "content_1_output_1", "value1" },
                    { "content_2_output_1", "value3" },
                    { "content_2_output_2", "value4" },
                },
                Contents = new Dictionary<string, MockContent> {
                    { "content_1", new MockContent { OutputVariables = ["content_1_output_1", "content_1_output_2"]  } },
                    { "content_2", new MockContent { OutputVariables = ["content_2_output_1", "content_2_output_2"]  } },
                }
            };

            var validator = new DependencyValidation();
            var result = validator.Validate(new Context(fileSystem, configuration), ["content_1", "content_2"]);

            Assert.IsTrue(result.FulFilled);
            Assert.AreEqual(1, result.EquivalentContent[0].MissingVariables.Count());
            Assert.AreEqual("content_1_output_2", result.EquivalentContent[0].MissingVariables.First());
            Assert.AreEqual(0, result.EquivalentContent[1].MissingVariables.Count());
        }

        [TestMethod]
        public void Validate_Multi_1_AllMissingVariable_2_NoMissingVariable_ShouldBeFulfilled()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var configuration = new MockDependencyValidationConfiguration
            {
                Variables = new Dictionary<string, string> {
                    { "content_2_output_1", "value3" },
                    { "content_2_output_2", "value4" },
                },
                Contents = new Dictionary<string, MockContent> {
                    { "content_1", new MockContent { OutputVariables = ["content_1_output_1", "content_1_output_2"]  } },
                    { "content_2", new MockContent { OutputVariables = ["content_2_output_1", "content_2_output_2"]  } },
                }
            };

            var validator = new DependencyValidation();
            var result = validator.Validate(new Context(fileSystem, configuration), ["content_1", "content_2"]);

            Assert.IsTrue(result.FulFilled);
            Assert.AreEqual(2, result.EquivalentContent[0].MissingVariables.Count());
            CollectionAssert.AreEquivalent(new[] { "content_1_output_1", "content_1_output_2" }, result.EquivalentContent[0].MissingVariables);
            Assert.AreEqual(0, result.EquivalentContent[1].MissingVariables.Count());
        }

        [TestMethod]
        public void Validate_Multi_1_OneMissingVariable_2_OneMissingVariable_ShouldNotBeFulfilled()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var configuration = new MockDependencyValidationConfiguration
            {
                Variables = new Dictionary<string, string> {
                    { "content_1_output_1", "value1" },
                    { "content_2_output_1", "value3" }
                },
                Contents = new Dictionary<string, MockContent> {
                    { "content_1", new MockContent { OutputVariables = ["content_1_output_1", "content_1_output_2"]  } },
                    { "content_2", new MockContent { OutputVariables = ["content_2_output_1", "content_2_output_2"]  } },
                }
            };

            var validator = new DependencyValidation();
            var result = validator.Validate(new Context(fileSystem, configuration), ["content_1", "content_2"]);

            Assert.IsFalse(result.FulFilled);
            Assert.AreEqual(1, result.EquivalentContent[0].MissingVariables.Count());
            Assert.AreEqual(1, result.EquivalentContent[1].MissingVariables.Count());
            Assert.AreEqual("content_1_output_2", result.EquivalentContent[0].MissingVariables.First());
            Assert.AreEqual("content_2_output_2", result.EquivalentContent[1].MissingVariables.First());
        }

        [TestMethod]
        public void Validate_Multi_1_AllMissingVariables_2_AllMissingVariables_ShouldNotBeFulfilled()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var configuration = new MockDependencyValidationConfiguration
            {
                Variables = new Dictionary<string, string> {},
                Contents = new Dictionary<string, MockContent> {
                    { "content_1", new MockContent { OutputVariables = ["content_1_output_1", "content_1_output_2"]  } },
                    { "content_2", new MockContent { OutputVariables = ["content_2_output_1", "content_2_output_2"]  } },
                }
            };

            var validator = new DependencyValidation();
            var result = validator.Validate(new Context(fileSystem, configuration), ["content_1", "content_2"]);

            Assert.IsFalse(result.FulFilled);
            Assert.AreEqual(2, result.EquivalentContent[0].MissingVariables.Count());
            Assert.AreEqual(2, result.EquivalentContent[1].MissingVariables.Count());
            CollectionAssert.AreEquivalent(new[] { "content_1_output_1", "content_1_output_2" }, result.EquivalentContent[0].MissingVariables);
            CollectionAssert.AreEquivalent(new[] { "content_2_output_1", "content_2_output_2" }, result.EquivalentContent[1].MissingVariables);
        }
    }
}