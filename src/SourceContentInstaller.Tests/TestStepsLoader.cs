using Pipelines;
using System.IO.Abstractions.TestingHelpers;

namespace SourceContentInstaller.Tests
{
    public class NullStepsMapper<ConfigT> : IStepMapper<ConfigT>
    {
        public IPipelineStepData Map(ConfigT jsonInstallStep)
        {
            return null;
        }
    }

    [TestClass]
    public class TestStepsLoader
    {
        [TestMethod]
        public void Load_ThrowException_WhenDeserializedDataIsNull()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                 { "C:/steps.json", new MockFileData("") }
            });

            var steps = new StepsLoader<object>(fileSystem, new NullWriter(), new NullConfigurationSerializer<IList<object>>(), new NullStepsMapper<object>());

            Assert.ThrowsException<Exception>(() => steps.Load("C:/steps.json"));
        }
    }
}