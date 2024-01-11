using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Pipelines;
using test_installsourcecontent_modpatcher;

using IConfiguration = test_installsourcecontent_modpatcher.IConfiguration;
using Context = test_installsourcecontent_modpatcher.Context;
using JSONInstallStep = test_installsourcecontent_modpatcher.JSONInstallStep;
using System.Collections.ObjectModel;
using test_installsourcecontent;

namespace test_installsourcecontent_modpatcher_tests
{
    public class TestValidateVariablesDependenciesInstallStepEventHandler : IValidateVariablesDependenciesInstallStepEventHandler
    {
        public int NoDependenciesSpecifiedTotal { get; private set; } = 0;
        public int MissingSingleVariableDependencyTotal { get; private set; } = 0;
        public int MissingMultiVariableDependencyTotal { get; private set; } = 0;
        public int MissingDependenciesTotal { get; private set; } = 0;

        public void NoDependenciesSpecified()
        {
            ++NoDependenciesSpecifiedTotal;
        }
        public void MissingSingleVariableDependency()
        {
            ++MissingSingleVariableDependencyTotal;
        }
        public void MissingMultiVariableDependency()
        {
            ++MissingMultiVariableDependencyTotal;
        }
        public void MissingDependencies()
        {
            ++MissingDependenciesTotal;
        }
    }

    public class SourceContentVariablesContextMock : Context
    {
        public SourceContentVariablesContextMock(IFileSystem fileSystem, IConfiguration configuration) : base(fileSystem, configuration)
        {
        }

        public Dictionary<string, string> Dependencies { get; set; } = new Dictionary<string, string>();

        public override ReadOnlyDictionary<string, string> GetSourceContentVariables()
        {
            return new ReadOnlyDictionary<string, string>(Dependencies);
        }
    }


    [TestClass]
    public class TestValidateVariablesDependenciesInstallStep
    {
        static IWriter NullWriter = new NullWriter();
        static IConfiguration NullConfiguration = new NullConfiguration();

        [TestMethod]
        public void EmptyDependenciesListReturnsFailed()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestValidateVariablesDependenciesInstallStepEventHandler();
            var step = new ValidateVariablesDependenciesInstallStep(eventHandler);
            var stepData = new ValidateVariablesDependenciesInstallStepData();

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Failed, result);
            Assert.AreEqual(1, eventHandler.NoDependenciesSpecifiedTotal);
        }

        [TestMethod]
        public void AllDependenciesMissing_SingleOnly_ReturnsFailed()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestValidateVariablesDependenciesInstallStepEventHandler();
            var step = new ValidateVariablesDependenciesInstallStep(eventHandler);
            var stepData = new ValidateVariablesDependenciesInstallStepData
            {
                Dependencies = [
                    ["dependency_1"],
                    ["dependency_2"],
                    ["dependency_3"]
                ]
            };

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Failed, result);
            Assert.AreEqual(3, eventHandler.MissingSingleVariableDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingMultiVariableDependencyTotal);
            Assert.AreEqual(1, eventHandler.MissingDependenciesTotal);
        }

        [TestMethod]
        public void AllDependenciesMissing_MultiOnly_ReturnsFailed()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestValidateVariablesDependenciesInstallStepEventHandler();
            var step = new ValidateVariablesDependenciesInstallStep(eventHandler);
            var stepData = new ValidateVariablesDependenciesInstallStepData
            {
                Dependencies = [
                    ["dependency_1a", "dependency_1b"],
                    ["dependency_2a", "dependency_2b"],
                    ["dependency_3a", "dependency_3b"]
                ]
            };

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Failed, result);
            Assert.AreEqual(0, eventHandler.MissingSingleVariableDependencyTotal);
            Assert.AreEqual(3, eventHandler.MissingMultiVariableDependencyTotal);
            Assert.AreEqual(1, eventHandler.MissingDependenciesTotal);
        }

        [TestMethod]
        public void AllDependenciesMissing_Single_And_Multi_ReturnsFailed()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestValidateVariablesDependenciesInstallStepEventHandler();
            var step = new ValidateVariablesDependenciesInstallStep(eventHandler);
            var stepData = new ValidateVariablesDependenciesInstallStepData
            {
                Dependencies = [
                    ["dependency_1"],
                    ["dependency_2a", "dependency_2b"],
                    ["dependency_3"]
                ]
            };

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Failed, result);
            Assert.AreEqual(2, eventHandler.MissingSingleVariableDependencyTotal);
            Assert.AreEqual(1, eventHandler.MissingMultiVariableDependencyTotal);
            Assert.AreEqual(1, eventHandler.MissingDependenciesTotal);
        }


        [TestMethod]
        public void AtLeastOneDependencyMissing_SingleOnly_ReturnsFailed()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestValidateVariablesDependenciesInstallStepEventHandler();
            var step = new ValidateVariablesDependenciesInstallStep(eventHandler);
            var stepData = new ValidateVariablesDependenciesInstallStepData
            {
                Dependencies = [
                    ["dependency_1"],
                    ["dependency_2"],
                    ["dependency_3"]
                ]
            };

            var context = new SourceContentVariablesContextMock(fileSystem, NullConfiguration)
            {
                Dependencies = new Dictionary<string, string>() {
                    { "dependency_1", "" },
                    { "dependency_2", "" },
                }
            };
            var result = step.DoStep(context, stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Failed, result);
            Assert.AreEqual(1, eventHandler.MissingSingleVariableDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingMultiVariableDependencyTotal);
            Assert.AreEqual(1, eventHandler.MissingDependenciesTotal);
        }

        [TestMethod]
        public void AtLeastOneDependencyMissing_MultiOnly_ReturnsFailed()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestValidateVariablesDependenciesInstallStepEventHandler();
            var step = new ValidateVariablesDependenciesInstallStep(eventHandler);
            var stepData = new ValidateVariablesDependenciesInstallStepData
            {
                Dependencies = [
                    ["dependency_1a", "dependency_1b"],
                    ["dependency_2a", "dependency_2b"],
                    ["dependency_3a", "dependency_3b"]
                ]
            };

            var context = new SourceContentVariablesContextMock(fileSystem, NullConfiguration)
            {
                Dependencies = new Dictionary<string, string>() {
                    { "dependency_1a", "" },
                    { "dependency_2a", "" }
                }
            };
            var result = step.DoStep(context, stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Failed, result);
            Assert.AreEqual(0, eventHandler.MissingSingleVariableDependencyTotal);
            Assert.AreEqual(1, eventHandler.MissingMultiVariableDependencyTotal);
            Assert.AreEqual(1, eventHandler.MissingDependenciesTotal);
        }

        [TestMethod]
        public void AtLeastOneDependencyMissing_Single_And_Multi_ReturnsFailed()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestValidateVariablesDependenciesInstallStepEventHandler();
            var step = new ValidateVariablesDependenciesInstallStep(eventHandler);
            var stepData = new ValidateVariablesDependenciesInstallStepData
            {
                Dependencies = [
                    ["dependency_1"],
                    ["dependency_2a", "dependency_2b"],
                    ["dependency_3"],
                    ["dependency_4a", "dependency_4b"],
                    ["dependency_5"],
                    ["dependency_6a", "dependency_6b"],
                ]
            };

            var context = new SourceContentVariablesContextMock(fileSystem, NullConfiguration)
            {
                Dependencies = new Dictionary<string, string>() {
                    { "dependency_1", "" },
                    { "dependency_2a", "" },
                    { "dependency_3", "" },
                    { "dependency_4a", "" }
                }
            };
            var result = step.DoStep(context, stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Failed, result);
            Assert.AreEqual(1, eventHandler.MissingSingleVariableDependencyTotal);
            Assert.AreEqual(1, eventHandler.MissingMultiVariableDependencyTotal);
            Assert.AreEqual(1, eventHandler.MissingDependenciesTotal);
        }

        [TestMethod]
        public void AllDependenciesFulfilled_Single_ReturnsCompleted()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestValidateVariablesDependenciesInstallStepEventHandler();
            var step = new ValidateVariablesDependenciesInstallStep(eventHandler);
            var stepData = new ValidateVariablesDependenciesInstallStepData
            {
                Dependencies = [
                    ["dependency_1"],
                    ["dependency_2"],
                    ["dependency_3"]
                ]
            };

            var context = new SourceContentVariablesContextMock(fileSystem, NullConfiguration)
            {
                Dependencies = new Dictionary<string, string>() {
                    { "dependency_1", "" },
                    { "dependency_2", "" },
                    { "dependency_3", "" }
                }
            };
            var result = step.DoStep(context, stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Complete, result);
            Assert.AreEqual(0, eventHandler.MissingSingleVariableDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingMultiVariableDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingDependenciesTotal);
        }

        [TestMethod]
        public void AllDependenciesFulfilled_Multi_ReturnsCompleted()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestValidateVariablesDependenciesInstallStepEventHandler();
            var step = new ValidateVariablesDependenciesInstallStep(eventHandler);
            var stepData = new ValidateVariablesDependenciesInstallStepData
            {
                Dependencies = [
                    ["dependency_1a", "dependency_1b"],
                    ["dependency_2a", "dependency_2b"],
                    ["dependency_3a", "dependency_3b"]
                ]
            };

            var context = new SourceContentVariablesContextMock(fileSystem, NullConfiguration)
            {
                Dependencies = new Dictionary<string, string>() {
                    { "dependency_1a", "" },
                    { "dependency_2a", "" },
                    { "dependency_3a", "" }
                }
            };
            var result = step.DoStep(context, stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Complete, result);
            Assert.AreEqual(0, eventHandler.MissingSingleVariableDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingMultiVariableDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingDependenciesTotal);
        }

        [TestMethod]
        public void AllDependenciesFulfilled_Single_And_Multi_ReturnsCompleted()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestValidateVariablesDependenciesInstallStepEventHandler();
            var step = new ValidateVariablesDependenciesInstallStep(eventHandler);
            var stepData = new ValidateVariablesDependenciesInstallStepData
            {
                Dependencies = [
                    ["dependency_1"],
                    ["dependency_2a", "dependency_2b"],
                    ["dependency_3"],
                    ["dependency_4a", "dependency_4b"]
                ]
            };

            var context = new SourceContentVariablesContextMock(fileSystem, NullConfiguration)
            {
                Dependencies = new Dictionary<string, string>() {
                    { "dependency_1", "" },
                    { "dependency_2a", "" },
                    { "dependency_3", "" },
                    { "dependency_4a", "" },
                }
            };
            var result = step.DoStep(context, stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Complete, result);
            Assert.AreEqual(0, eventHandler.MissingSingleVariableDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingMultiVariableDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingDependenciesTotal);
        }

        [TestMethod]
        public void TwoDependencies_Multi_SecondInFirstFulfilled_SecondInLastNotFulfilled_ReturnsFailed()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestValidateVariablesDependenciesInstallStepEventHandler();
            var step = new ValidateVariablesDependenciesInstallStep(eventHandler);
            var stepData = new ValidateVariablesDependenciesInstallStepData
            {
                Dependencies = [
                    ["dependency_1a", "dependency_1b"],
                    ["dependency_2a", "dependency_2b"],
                ]
            };

            var context = new SourceContentVariablesContextMock(fileSystem, NullConfiguration)
            {
                Dependencies = new Dictionary<string, string>() {
                    { "dependency_1b", "" },
                }
            };
            var result = step.DoStep(context, stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Failed, result);
            Assert.AreEqual(0, eventHandler.MissingSingleVariableDependencyTotal);
            Assert.AreEqual(1, eventHandler.MissingMultiVariableDependencyTotal);
            Assert.AreEqual(1, eventHandler.MissingDependenciesTotal);
        }

        [TestMethod]
        public void TwoDependencies_Multi_SecondInFirstNotFulfilled_SecondInLastFulfilled_ReturnsFailed()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestValidateVariablesDependenciesInstallStepEventHandler();
            var step = new ValidateVariablesDependenciesInstallStep(eventHandler);
            var stepData = new ValidateVariablesDependenciesInstallStepData
            {
                Dependencies = [
                    ["dependency_1a", "dependency_1b"],
                    ["dependency_2a", "dependency_2b"],
                ]
            };

            var context = new SourceContentVariablesContextMock(fileSystem, NullConfiguration)
            {
                Dependencies = new Dictionary<string, string>() {
                    { "dependency_2b", "" },
                }
            };
            var result = step.DoStep(context, stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Failed, result);
            Assert.AreEqual(0, eventHandler.MissingSingleVariableDependencyTotal);
            Assert.AreEqual(1, eventHandler.MissingMultiVariableDependencyTotal);
            Assert.AreEqual(1, eventHandler.MissingDependenciesTotal);
        }

        [TestMethod]
        public void TwoDependencies_Multi_AllSecondsFulfilled_ReturnsCompleted()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestValidateVariablesDependenciesInstallStepEventHandler();
            var step = new ValidateVariablesDependenciesInstallStep(eventHandler);
            var stepData = new ValidateVariablesDependenciesInstallStepData
            {
                Dependencies = [
                    ["dependency_1a", "dependency_1b"],
                    ["dependency_2a", "dependency_2b"],
                ]
            };

            var context = new SourceContentVariablesContextMock(fileSystem, NullConfiguration)
            {
                Dependencies = new Dictionary<string, string>() {
                    { "dependency_1b", "" },
                    { "dependency_2b", "" },
                }
            };
            var result = step.DoStep(context, stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Complete, result);
            Assert.AreEqual(0, eventHandler.MissingSingleVariableDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingMultiVariableDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingDependenciesTotal);
        }

        [TestMethod]
        public void OnlyOneDependency_Single_Fulfilled_ReturnsCompleted()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestValidateVariablesDependenciesInstallStepEventHandler();
            var step = new ValidateVariablesDependenciesInstallStep(eventHandler);
            var stepData = new ValidateVariablesDependenciesInstallStepData
            {
                Dependencies = [
                    ["dependency_1"]
                ]
            };

            var context = new SourceContentVariablesContextMock(fileSystem, NullConfiguration)
            {
                Dependencies = new Dictionary<string, string>() {
                    { "dependency_1", "" },
                }
            };
            var result = step.DoStep(context, stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Complete, result);
            Assert.AreEqual(0, eventHandler.MissingSingleVariableDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingMultiVariableDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingDependenciesTotal);
        }

        [TestMethod]
        public void OnlyOneDependency_Single_NotFulfilled_ReturnsFailed()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestValidateVariablesDependenciesInstallStepEventHandler();
            var step = new ValidateVariablesDependenciesInstallStep(eventHandler);
            var stepData = new ValidateVariablesDependenciesInstallStepData
            {
                Dependencies = [
                    ["dependency_1"]
                ]
            };

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Failed, result);
            Assert.AreEqual(1, eventHandler.MissingSingleVariableDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingMultiVariableDependencyTotal);
            Assert.AreEqual(1, eventHandler.MissingDependenciesTotal);
        }

        [TestMethod]
        public void OnlyOneDependency_Multi_Fulfilled_ReturnsCompleted()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestValidateVariablesDependenciesInstallStepEventHandler();
            var step = new ValidateVariablesDependenciesInstallStep(eventHandler);
            var stepData = new ValidateVariablesDependenciesInstallStepData
            {
                Dependencies = [
                    ["dependency_1", "dependency_2"]
                ]
            };

            var context = new SourceContentVariablesContextMock(fileSystem, NullConfiguration)
            {
                Dependencies = new Dictionary<string, string>() {
                    { "dependency_1", "" },
                }
            };
            var result = step.DoStep(context, stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Complete, result);
            Assert.AreEqual(0, eventHandler.MissingSingleVariableDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingMultiVariableDependencyTotal);
            Assert.AreEqual(0, eventHandler.MissingDependenciesTotal);
        }

        [TestMethod]
        public void OnlyOneDependency_Multi_NotFulfilled_ReturnsFailed()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestValidateVariablesDependenciesInstallStepEventHandler();
            var step = new ValidateVariablesDependenciesInstallStep(eventHandler);
            var stepData = new ValidateVariablesDependenciesInstallStepData
            {
                Dependencies = [
                    ["dependency_1", "dependency_2"]
                ]
            };

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Failed, result);
            Assert.AreEqual(0, eventHandler.MissingSingleVariableDependencyTotal);
            Assert.AreEqual(1, eventHandler.MissingMultiVariableDependencyTotal);
            Assert.AreEqual(1, eventHandler.MissingDependenciesTotal);
        }

        [TestMethod]
        public void StepsLoader_Load_Simple()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>{
                { "C:/step_validate_variables_dependencies.json", new MockFileData(File.ReadAllBytes("../../../data/config/step_validate_variables_dependencies.json")) },
            });

            var stepsLoader = new StepsLoader<JSONInstallStep>(fileSystem, NullWriter, new JSONConfigurationSerializer<IList<JSONInstallStep>>(), new test_installsourcecontent_modpatcher.InstallStepMapper<JSONInstallStep>());

            var stepsList = stepsLoader.Load("C:/step_validate_variables_dependencies.json");

            Assert.IsNotNull(stepsList);
            var stepData = (ValidateVariablesDependenciesInstallStepData)stepsList[0];
            Assert.AreEqual("step_validate_variables_dependencies", stepData.Name);
            Assert.AreEqual("Validate sourcemod variables dependencies", stepData.Description);
            CollectionAssert.AreEquivalent(new List<string> { "previous_step1" }, stepData.DependsOn);

            Assert.IsNotNull(stepData.Dependencies);
            Assert.AreEqual(3, stepData.Dependencies.Count);
            CollectionAssert.AreEquivalent(new List<string> { "dependency_1" }, stepData.Dependencies[0]);
            CollectionAssert.AreEquivalent(new List<string> { "dependency_2a", "dependency_2b" }, stepData.Dependencies[1]);
            CollectionAssert.AreEquivalent(new List<string> { "dependency_3" }, stepData.Dependencies[2]);
        }
    }
}