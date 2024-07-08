using Pipelines;
using SourceContentInstaller;
using System.Collections.ObjectModel;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;

namespace SourceModPatcher.Tests
{
    public class TestValidateVariablesDependenciesInstallStepEventHandler : IValidateVariablesDependenciesInstallStepEventHandler
    {
        public int MissingSingleVariableRequiredDependencyTotal { get; private set; } = 0;
        public int MissingMultiVariableRequiredDependencyTotal { get; private set; } = 0;

        public int MissingSingleVariableOptionalDependencyTotal { get; private set; } = 0;
        public int MissingMultiVariableOptionalDependencyTotal { get; private set; } = 0;

        public void MissingSingleVariableRequiredDependency()
        {
            ++MissingSingleVariableRequiredDependencyTotal;
        }
        public void MissingMultiVariableRequiredDependency()
        {
            ++MissingMultiVariableRequiredDependencyTotal;
        }

        public void MissingSingleVariableOptionalDependency()
        {
            ++MissingSingleVariableOptionalDependencyTotal;
        }
        public void MissingMultiVariableOptionalDependency()
        {
            ++MissingMultiVariableOptionalDependencyTotal;
        }
    }

    [TestClass]
    public class TestValidateVariablesDependenciesInstallStep
    {
        static readonly IWriter NullWriter = new NullWriter();
        static readonly IConfiguration NullConfiguration = new NullConfiguration();
        static readonly IDependencyValidation DefaultDependencyValidator = new DependencyValidation();

        [TestMethod]
        public void ValidateDependenciesStep_RequiredSingleAllFulfilled_ReturnsCompleted()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestValidateVariablesDependenciesInstallStepEventHandler();
            var step = new ValidateVariablesDependenciesInstallStep(DefaultDependencyValidator, eventHandler);
            var stepData = new ValidateVariablesDependenciesInstallStepData();
            var configuration = new MockDependencyValidationConfiguration
            {
                Variables = new Dictionary<string, string> {
                    { "content_1_output_1", "value1" },
                    { "content_2_output_1", "value2" }
                },
                Contents = new Dictionary<string, MockContent> {
                    { "content_1", new MockContent { OutputVariables = ["content_1_output_1"] } },
                    { "content_2", new MockContent { OutputVariables = ["content_2_output_1"] } },
                },
                SourceMods = new Dictionary<string, MockSourceModConfigEntry> {
                    {  "sourcemod_1", new MockSourceModConfigEntry{ 
                        Dependencies = new MockSourceModConfigDependencies { 
                            Required = [["content_1"],["content_2"]]
                        }
                    } }
                }
            };

            var result = step.DoStep(new Context(fileSystem, configuration) { SourceModID = "sourcemod_1" }, stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Complete, result);
            Assert.AreEqual(0, eventHandler.MissingSingleVariableRequiredDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingMultiVariableRequiredDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingSingleVariableOptionalDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingMultiVariableOptionalDependencyTotal);
        }


        [TestMethod]
        public void ValidateDependenciesStep_RequiredSingleOneNotFulfilled_ReturnsFailed()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestValidateVariablesDependenciesInstallStepEventHandler();
            var step = new ValidateVariablesDependenciesInstallStep(DefaultDependencyValidator, eventHandler);
            var stepData = new ValidateVariablesDependenciesInstallStepData();
            var configuration = new MockDependencyValidationConfiguration
            {
                Variables = new Dictionary<string, string> {
                    { "content_1_output_1", "value1" },
                },
                Contents = new Dictionary<string, MockContent> {
                    { "content_1", new MockContent { OutputVariables = ["content_1_output_1"] } },
                    { "content_2", new MockContent { OutputVariables = ["content_2_output_1"] } },
                },
                SourceMods = new Dictionary<string, MockSourceModConfigEntry> {
                    {  "sourcemod_1", new MockSourceModConfigEntry{
                        Dependencies = new MockSourceModConfigDependencies {
                            Required = [["content_1"],["content_2"]]
                        }
                    } }
                }
            };

            var result = step.DoStep(new Context(fileSystem, configuration) { SourceModID = "sourcemod_1" }, stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Failed, result);
            Assert.AreEqual(1, eventHandler.MissingSingleVariableRequiredDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingMultiVariableRequiredDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingSingleVariableOptionalDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingMultiVariableOptionalDependencyTotal);
        }


        [TestMethod]
        public void ValidateDependenciesStep_RequiredSingleAllNotFulfilled_ReturnsFailed()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestValidateVariablesDependenciesInstallStepEventHandler();
            var step = new ValidateVariablesDependenciesInstallStep(DefaultDependencyValidator, eventHandler);
            var stepData = new ValidateVariablesDependenciesInstallStepData();
            var configuration = new MockDependencyValidationConfiguration
            {
                Variables = new Dictionary<string, string> {},
                Contents = new Dictionary<string, MockContent> {
                    { "content_1", new MockContent { OutputVariables = ["content_1_output_1"] } },
                    { "content_2", new MockContent { OutputVariables = ["content_2_output_1"] } },
                },
                SourceMods = new Dictionary<string, MockSourceModConfigEntry> {
                    {  "sourcemod_1", new MockSourceModConfigEntry{
                        Dependencies = new MockSourceModConfigDependencies {
                            Required = [["content_1"],["content_2"]]
                        }
                    } }
                }
            };

            var result = step.DoStep(new Context(fileSystem, configuration) { SourceModID = "sourcemod_1" }, stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Failed, result);
            Assert.AreEqual(2, eventHandler.MissingSingleVariableRequiredDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingMultiVariableRequiredDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingSingleVariableOptionalDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingMultiVariableOptionalDependencyTotal);
        }

        [TestMethod]
        public void ValidateDependenciesStep_OptionalSingleAllFulfilled_ReturnsCompleted()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestValidateVariablesDependenciesInstallStepEventHandler();
            var step = new ValidateVariablesDependenciesInstallStep(DefaultDependencyValidator, eventHandler);
            var stepData = new ValidateVariablesDependenciesInstallStepData();
            var configuration = new MockDependencyValidationConfiguration
            {
                Variables = new Dictionary<string, string> {
                    { "content_1_output_1", "value1" },
                    { "content_2_output_1", "value2" }
                },
                Contents = new Dictionary<string, MockContent> {
                    { "content_1", new MockContent { OutputVariables = ["content_1_output_1"] } },
                    { "content_2", new MockContent { OutputVariables = ["content_2_output_1"] } },
                },
                SourceMods = new Dictionary<string, MockSourceModConfigEntry> {
                    {  "sourcemod_1", new MockSourceModConfigEntry{
                        Dependencies = new MockSourceModConfigDependencies {
                            Optional = [["content_1"],["content_2"]]
                        }
                    } }
                }
            };

            var result = step.DoStep(new Context(fileSystem, configuration) { SourceModID = "sourcemod_1" }, stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Complete, result);
            Assert.AreEqual(0, eventHandler.MissingSingleVariableRequiredDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingMultiVariableRequiredDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingSingleVariableOptionalDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingMultiVariableOptionalDependencyTotal);
        }


        [TestMethod]
        public void ValidateDependenciesStep_OptionalSingleOneNotFulfilled_ReturnsCompleted()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestValidateVariablesDependenciesInstallStepEventHandler();
            var step = new ValidateVariablesDependenciesInstallStep(DefaultDependencyValidator, eventHandler);
            var stepData = new ValidateVariablesDependenciesInstallStepData();
            var configuration = new MockDependencyValidationConfiguration
            {
                Variables = new Dictionary<string, string> {
                    { "content_1_output_1", "value1" },
                },
                Contents = new Dictionary<string, MockContent> {
                    { "content_1", new MockContent { OutputVariables = ["content_1_output_1"] } },
                    { "content_2", new MockContent { OutputVariables = ["content_2_output_1"] } },
                },
                SourceMods = new Dictionary<string, MockSourceModConfigEntry> {
                    {  "sourcemod_1", new MockSourceModConfigEntry{
                        Dependencies = new MockSourceModConfigDependencies {
                            Optional = [["content_1"],["content_2"]]
                        }
                    } }
                }
            };

            var result = step.DoStep(new Context(fileSystem, configuration) { SourceModID = "sourcemod_1" }, stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Complete, result);
            Assert.AreEqual(0, eventHandler.MissingSingleVariableRequiredDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingMultiVariableRequiredDependencyTotal);
            Assert.AreEqual(1, eventHandler.MissingSingleVariableOptionalDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingMultiVariableOptionalDependencyTotal);
        }

        [TestMethod]
        public void ValidateDependenciesStep_OptionalSingleAllNotFulfilled_ReturnsCompleted()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestValidateVariablesDependenciesInstallStepEventHandler();
            var step = new ValidateVariablesDependenciesInstallStep(DefaultDependencyValidator, eventHandler);
            var stepData = new ValidateVariablesDependenciesInstallStepData();
            var configuration = new MockDependencyValidationConfiguration
            {
                Variables = new Dictionary<string, string> { },
                Contents = new Dictionary<string, MockContent> {
                    { "content_1", new MockContent { OutputVariables = ["content_1_output_1"] } },
                    { "content_2", new MockContent { OutputVariables = ["content_2_output_1"] } },
                },
                SourceMods = new Dictionary<string, MockSourceModConfigEntry> {
                    {  "sourcemod_1", new MockSourceModConfigEntry{
                        Dependencies = new MockSourceModConfigDependencies {
                            Optional = [["content_1"],["content_2"]]
                        }
                    } }
                }
            };

            var result = step.DoStep(new Context(fileSystem, configuration) { SourceModID = "sourcemod_1" }, stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Complete, result);
            Assert.AreEqual(0, eventHandler.MissingSingleVariableRequiredDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingMultiVariableRequiredDependencyTotal);
            Assert.AreEqual(2, eventHandler.MissingSingleVariableOptionalDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingMultiVariableOptionalDependencyTotal);
        }

        [TestMethod]
        public void ValidateDependenciesStep_RequiredSingleFulfilled_OptionalSingleFulfilled_ReturnsCompleted()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestValidateVariablesDependenciesInstallStepEventHandler();
            var step = new ValidateVariablesDependenciesInstallStep(DefaultDependencyValidator, eventHandler);
            var stepData = new ValidateVariablesDependenciesInstallStepData();
            var configuration = new MockDependencyValidationConfiguration
            {
                Variables = new Dictionary<string, string> {
                    { "content_1_output_1", "value1" },
                    { "content_2_output_1", "value2" }
                },
                Contents = new Dictionary<string, MockContent> {
                    { "content_1", new MockContent { OutputVariables = ["content_1_output_1"] } },
                    { "content_2", new MockContent { OutputVariables = ["content_2_output_1"] } },
                },
                SourceMods = new Dictionary<string, MockSourceModConfigEntry> {
                    {  "sourcemod_1", new MockSourceModConfigEntry{
                        Dependencies = new MockSourceModConfigDependencies {
                            Required = [["content_1"]],
                            Optional = [["content_2"]]
                        }
                    } }
                }
            };

            var result = step.DoStep(new Context(fileSystem, configuration) { SourceModID = "sourcemod_1" }, stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Complete, result);
            Assert.AreEqual(0, eventHandler.MissingSingleVariableRequiredDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingMultiVariableRequiredDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingSingleVariableOptionalDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingMultiVariableOptionalDependencyTotal);
        }

        [TestMethod]
        public void ValidateDependenciesStep_RequiredSingleFulfilled_OptionalSingleNotFulfilled_ReturnsCompleted()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestValidateVariablesDependenciesInstallStepEventHandler();
            var step = new ValidateVariablesDependenciesInstallStep(DefaultDependencyValidator, eventHandler);
            var stepData = new ValidateVariablesDependenciesInstallStepData();
            var configuration = new MockDependencyValidationConfiguration
            {
                Variables = new Dictionary<string, string> {
                    { "content_1_output_1", "value1" },
                },
                Contents = new Dictionary<string, MockContent> {
                    { "content_1", new MockContent { OutputVariables = ["content_1_output_1"] } },
                    { "content_2", new MockContent { OutputVariables = ["content_2_output_1"] } },
                },
                SourceMods = new Dictionary<string, MockSourceModConfigEntry> {
                    {  "sourcemod_1", new MockSourceModConfigEntry{
                        Dependencies = new MockSourceModConfigDependencies {
                            Required = [["content_1"]],
                            Optional = [["content_2"]]
                        }
                    } }
                }
            };

            var result = step.DoStep(new Context(fileSystem, configuration) { SourceModID = "sourcemod_1" }, stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Complete, result);
            Assert.AreEqual(0, eventHandler.MissingSingleVariableRequiredDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingMultiVariableRequiredDependencyTotal);
            Assert.AreEqual(1, eventHandler.MissingSingleVariableOptionalDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingMultiVariableOptionalDependencyTotal);
        }

        [TestMethod]
        public void ValidateDependenciesStep_RequiredSingleNotFulfilled_OptionalSingleFulfilled_ReturnsFailed()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestValidateVariablesDependenciesInstallStepEventHandler();
            var step = new ValidateVariablesDependenciesInstallStep(DefaultDependencyValidator, eventHandler);
            var stepData = new ValidateVariablesDependenciesInstallStepData();
            var configuration = new MockDependencyValidationConfiguration
            {
                Variables = new Dictionary<string, string> {
                    { "content_2_output_1", "value2" },
                },
                Contents = new Dictionary<string, MockContent> {
                    { "content_1", new MockContent { OutputVariables = ["content_1_output_1"] } },
                    { "content_2", new MockContent { OutputVariables = ["content_2_output_1"] } },
                },
                SourceMods = new Dictionary<string, MockSourceModConfigEntry> {
                    {  "sourcemod_1", new MockSourceModConfigEntry{
                        Dependencies = new MockSourceModConfigDependencies {
                            Required = [["content_1"]],
                            Optional = [["content_2"]]
                        }
                    } }
                }
            };

            var result = step.DoStep(new Context(fileSystem, configuration) { SourceModID = "sourcemod_1" }, stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Failed, result);
            Assert.AreEqual(1, eventHandler.MissingSingleVariableRequiredDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingMultiVariableRequiredDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingSingleVariableOptionalDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingMultiVariableOptionalDependencyTotal);
        }


        [TestMethod]
        public void ValidateDependenciesStep_RequiredSingleNotFulfilled_OptionalSingleNotFulfilled_ReturnsFailed()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestValidateVariablesDependenciesInstallStepEventHandler();
            var step = new ValidateVariablesDependenciesInstallStep(DefaultDependencyValidator, eventHandler);
            var stepData = new ValidateVariablesDependenciesInstallStepData();
            var configuration = new MockDependencyValidationConfiguration
            {
                Variables = new Dictionary<string, string> {},
                Contents = new Dictionary<string, MockContent> {
                    { "content_1", new MockContent { OutputVariables = ["content_1_output_1"] } },
                    { "content_2", new MockContent { OutputVariables = ["content_2_output_1"] } },
                },
                SourceMods = new Dictionary<string, MockSourceModConfigEntry> {
                    {  "sourcemod_1", new MockSourceModConfigEntry{
                        Dependencies = new MockSourceModConfigDependencies {
                            Required = [["content_1"]],
                            Optional = [["content_2"]]
                        }
                    } }
                }
            };

            var result = step.DoStep(new Context(fileSystem, configuration) { SourceModID = "sourcemod_1" }, stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Failed, result);
            Assert.AreEqual(1, eventHandler.MissingSingleVariableRequiredDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingMultiVariableRequiredDependencyTotal);
            Assert.AreEqual(1, eventHandler.MissingSingleVariableOptionalDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingMultiVariableOptionalDependencyTotal);
        }

        [TestMethod]
        public void ValidateDependenciesStep_RequiredMultiFulfilled_OptionalMultiFulfilled_NoMissingVariable_ReturnsCompleted()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestValidateVariablesDependenciesInstallStepEventHandler();
            var step = new ValidateVariablesDependenciesInstallStep(DefaultDependencyValidator, eventHandler);
            var stepData = new ValidateVariablesDependenciesInstallStepData();
            var configuration = new MockDependencyValidationConfiguration
            {
                Variables = new Dictionary<string, string> {
                    { "content_1_output_1", "value1" },
                    { "content_2_output_1", "value2" },
                    { "content_3_output_1", "value3" },
                    { "content_4_output_1", "value4" },
                },
                Contents = new Dictionary<string, MockContent> {
                    { "content_1", new MockContent { OutputVariables = ["content_1_output_1"] } },
                    { "content_2", new MockContent { OutputVariables = ["content_2_output_1"] } },
                    { "content_3", new MockContent { OutputVariables = ["content_3_output_1"] } },
                    { "content_4", new MockContent { OutputVariables = ["content_4_output_1"] } },
                },
                SourceMods = new Dictionary<string, MockSourceModConfigEntry> {
                    {  "sourcemod_1", new MockSourceModConfigEntry{
                        Dependencies = new MockSourceModConfigDependencies {
                            Required = [["content_1", "content_2"]],
                            Optional = [["content_3", "content_4"]]
                        }
                    } }
                }
            };

            var result = step.DoStep(new Context(fileSystem, configuration) { SourceModID = "sourcemod_1" }, stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Complete, result);
            Assert.AreEqual(0, eventHandler.MissingSingleVariableRequiredDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingMultiVariableRequiredDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingSingleVariableOptionalDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingMultiVariableOptionalDependencyTotal);
        }

        [TestMethod]
        public void ValidateDependenciesStep_RequiredMultiFulfilled_OptionalMultiFulfilled_WithMissingVariables_ReturnsCompleted()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestValidateVariablesDependenciesInstallStepEventHandler();
            var step = new ValidateVariablesDependenciesInstallStep(DefaultDependencyValidator, eventHandler);
            var stepData = new ValidateVariablesDependenciesInstallStepData();
            var configuration = new MockDependencyValidationConfiguration
            {
                Variables = new Dictionary<string, string> {
                    { "content_1_output_1", "value1" },
                    { "content_3_output_1", "value3" },
                },
                Contents = new Dictionary<string, MockContent> {
                    { "content_1", new MockContent { OutputVariables = ["content_1_output_1"] } },
                    { "content_2", new MockContent { OutputVariables = ["content_2_output_1"] } },
                    { "content_3", new MockContent { OutputVariables = ["content_3_output_1"] } },
                    { "content_4", new MockContent { OutputVariables = ["content_4_output_1"] } },
                },
                SourceMods = new Dictionary<string, MockSourceModConfigEntry> {
                    {  "sourcemod_1", new MockSourceModConfigEntry{
                        Dependencies = new MockSourceModConfigDependencies {
                            Required = [["content_1", "content_2"]],
                            Optional = [["content_3", "content_4"]]
                        }
                    } }
                }
            };

            var result = step.DoStep(new Context(fileSystem, configuration) { SourceModID = "sourcemod_1" }, stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Complete, result);
            Assert.AreEqual(0, eventHandler.MissingSingleVariableRequiredDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingMultiVariableRequiredDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingSingleVariableOptionalDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingMultiVariableOptionalDependencyTotal);
        }

        [TestMethod]
        public void ValidateDependenciesStep_RequiredMultiFulfilled_OptionalMultiNotFulfilled_WithMissingVariables_ReturnsCompleted()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestValidateVariablesDependenciesInstallStepEventHandler();
            var step = new ValidateVariablesDependenciesInstallStep(DefaultDependencyValidator, eventHandler);
            var stepData = new ValidateVariablesDependenciesInstallStepData();
            var configuration = new MockDependencyValidationConfiguration
            {
                Variables = new Dictionary<string, string> {
                    { "content_1_output_1", "value1" },
                },
                Contents = new Dictionary<string, MockContent> {
                    { "content_1", new MockContent { OutputVariables = ["content_1_output_1"] } },
                    { "content_2", new MockContent { OutputVariables = ["content_2_output_1"] } },
                    { "content_3", new MockContent { OutputVariables = ["content_3_output_1"] } },
                    { "content_4", new MockContent { OutputVariables = ["content_4_output_1"] } },
                },
                SourceMods = new Dictionary<string, MockSourceModConfigEntry> {
                    {  "sourcemod_1", new MockSourceModConfigEntry{
                        Dependencies = new MockSourceModConfigDependencies {
                            Required = [["content_1", "content_2"]],
                            Optional = [["content_3", "content_4"]]
                        }
                    } }
                }
            };

            var result = step.DoStep(new Context(fileSystem, configuration) { SourceModID = "sourcemod_1" }, stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Complete, result);
            Assert.AreEqual(0, eventHandler.MissingSingleVariableRequiredDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingMultiVariableRequiredDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingSingleVariableOptionalDependencyTotal);
            Assert.AreEqual(1, eventHandler.MissingMultiVariableOptionalDependencyTotal);
        }


        [TestMethod]
        public void ValidateDependenciesStep_RequiredMultiNotFulfilled_OptionalMultiFulfilled_WithMissingVariables_ReturnsFailed()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestValidateVariablesDependenciesInstallStepEventHandler();
            var step = new ValidateVariablesDependenciesInstallStep(DefaultDependencyValidator, eventHandler);
            var stepData = new ValidateVariablesDependenciesInstallStepData();
            var configuration = new MockDependencyValidationConfiguration
            {
                Variables = new Dictionary<string, string> {
                    { "content_3_output_1", "value3" },
                },
                Contents = new Dictionary<string, MockContent> {
                    { "content_1", new MockContent { OutputVariables = ["content_1_output_1"] } },
                    { "content_2", new MockContent { OutputVariables = ["content_2_output_1"] } },
                    { "content_3", new MockContent { OutputVariables = ["content_3_output_1"] } },
                    { "content_4", new MockContent { OutputVariables = ["content_4_output_1"] } },
                },
                SourceMods = new Dictionary<string, MockSourceModConfigEntry> {
                    {  "sourcemod_1", new MockSourceModConfigEntry{
                        Dependencies = new MockSourceModConfigDependencies {
                            Required = [["content_1", "content_2"]],
                            Optional = [["content_3", "content_4"]]
                        }
                    } }
                }
            };

            var result = step.DoStep(new Context(fileSystem, configuration) { SourceModID = "sourcemod_1" }, stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Failed, result);
            Assert.AreEqual(0, eventHandler.MissingSingleVariableRequiredDependencyTotal);
            Assert.AreEqual(1, eventHandler.MissingMultiVariableRequiredDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingSingleVariableOptionalDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingMultiVariableOptionalDependencyTotal);
        }

        [TestMethod]
        public void ValidateDependenciesStep_RequiredMultiNotFulfilled_OptionalMultiNotFulfilled_ReturnsFailed()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestValidateVariablesDependenciesInstallStepEventHandler();
            var step = new ValidateVariablesDependenciesInstallStep(DefaultDependencyValidator, eventHandler);
            var stepData = new ValidateVariablesDependenciesInstallStepData();
            var configuration = new MockDependencyValidationConfiguration
            {
                Variables = new Dictionary<string, string> {},
                Contents = new Dictionary<string, MockContent> {
                    { "content_1", new MockContent { OutputVariables = ["content_1_output_1"] } },
                    { "content_2", new MockContent { OutputVariables = ["content_2_output_1"] } },
                    { "content_3", new MockContent { OutputVariables = ["content_3_output_1"] } },
                    { "content_4", new MockContent { OutputVariables = ["content_4_output_1"] } },
                },
                SourceMods = new Dictionary<string, MockSourceModConfigEntry> {
                    {  "sourcemod_1", new MockSourceModConfigEntry{
                        Dependencies = new MockSourceModConfigDependencies {
                            Required = [["content_1", "content_2"]],
                            Optional = [["content_3", "content_4"]]
                        }
                    } }
                }
            };

            var result = step.DoStep(new Context(fileSystem, configuration) { SourceModID = "sourcemod_1" }, stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Failed, result);
            Assert.AreEqual(0, eventHandler.MissingSingleVariableRequiredDependencyTotal);
            Assert.AreEqual(1, eventHandler.MissingMultiVariableRequiredDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingSingleVariableOptionalDependencyTotal);
            Assert.AreEqual(1, eventHandler.MissingMultiVariableOptionalDependencyTotal);
        }
    }
}