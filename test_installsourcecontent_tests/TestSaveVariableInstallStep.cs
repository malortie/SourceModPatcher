using System.IO.Abstractions.TestingHelpers;
using Pipelines;
using test_installsourcecontent;

namespace test_installsourcecontent_tests
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
    }
}