using Pipelines;
using System.IO.Abstractions.TestingHelpers;

namespace SourceContentInstaller.Tests
{
    public class TestSaveVariableInstallStepEventHandler : ISaveVariableInstallStepEventHandler
    {
        public int NoVariableNameSpecifiedTotal { get; private set; } = 0;
        public int NoVariableValueSpecifiedTotal { get; private set; } = 0;

        public void NoVariableNameSpecified() => ++NoVariableNameSpecifiedTotal;
        public void NoVariableValueSpecified() => ++NoVariableValueSpecifiedTotal;
    }

    [TestClass]
    public class TestSaveVariableInstallStep
    {
        static IWriter NullWriter = new NullWriter();
        static IConfiguration NullConfiguration = new NullConfiguration();

        [TestMethod]
        public void BlankVariableNameReturnsFailed()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestSaveVariableInstallStepEventHandler();
            var step = new SaveVariableInstallStep(eventHandler);
            var stepData = new SaveVariableInstallStepData();

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Failed, result);
            Assert.AreEqual(1, eventHandler.NoVariableNameSpecifiedTotal);
            Assert.AreEqual(0, eventHandler.NoVariableValueSpecifiedTotal);
        }

        [TestMethod]
        public void BlankVariableValueReturnsFailed()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestSaveVariableInstallStepEventHandler();
            var step = new SaveVariableInstallStep(eventHandler);
            var stepData = new SaveVariableInstallStepData()
            {
                VariableName = "test"
            };

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Failed, result);
            Assert.AreEqual(0, eventHandler.NoVariableNameSpecifiedTotal);
            Assert.AreEqual(1, eventHandler.NoVariableValueSpecifiedTotal);
        }

        [TestMethod]
        public void SaveVariableReturnsCompleted()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestSaveVariableInstallStepEventHandler();
            var step = new SaveVariableInstallStep(eventHandler);
            var stepData = new SaveVariableInstallStepData()
            {
                VariableName = "test1",
                VariableValue = "test2"
            };

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Complete, result);
            Assert.AreEqual(0, eventHandler.NoVariableNameSpecifiedTotal);
            Assert.AreEqual(0, eventHandler.NoVariableValueSpecifiedTotal);
        }

        [TestMethod]
        public void StepsLoader_Load_Simple()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>{
                { "C:/step_save_variable.json", new MockFileData(File.ReadAllBytes("../../../data/config/step_save_variable.json")) },
            });

            var stepsLoader = new StepsLoader<JSONInstallStep>(fileSystem, NullWriter, new JSONConfigurationSerializer<IList<JSONInstallStep>>(), new InstallStepMapper<JSONInstallStep>());

            var stepsList = stepsLoader.Load("C:/step_save_variable.json");

            Assert.IsNotNull(stepsList);
            var stepData = (SaveVariableInstallStepData)stepsList[0];
            Assert.AreEqual("step_save_variable", stepData.Name);
            Assert.AreEqual("Add game install path to variables.json", stepData.Description);
            CollectionAssert.AreEquivalent(new List<string> { "previous_step1" }, stepData.DependsOn);
            Assert.AreEqual("game_content_path", stepData.VariableName);
            Assert.AreEqual("value1", stepData.VariableValue);
        }
    }
}