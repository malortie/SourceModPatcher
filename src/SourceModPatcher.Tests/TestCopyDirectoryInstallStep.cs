using Pipelines;
using SourceContentInstaller;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;

namespace SourceModPatcher.Tests
{
    public class TestCopyDirectoryInstallStepEventHandler : ICopyDirectoryInstallStepEventHandler
    {
        public int BlankDestinationEntryTotal { get; set; }
        public int BlankSourceEntryTotal { get; set; }
        public int DirectoryCopyFailedTotal { get; set; }
        public int DirectoryCopySuccessTotal { get; set; }
        public int NoDestinationSpecifiedTotal { get; set; }
        public int NoSourceSpecifiedTotal { get; set; }

        public void BlankDestinationEntry()
        {
            ++BlankDestinationEntryTotal;
        }

        public void BlankSourceEntry()
        {
            ++BlankSourceEntryTotal;
        }

        public void DirectoryCopyFailed()
        {
            ++DirectoryCopyFailedTotal;
        }

        public void DirectoryCopySuccess()
        {
            ++DirectoryCopySuccessTotal;
        }

        public void NoDestinationSpecified()
        {
            ++NoDestinationSpecifiedTotal;
        }

        public void NoSourceSpecified()
        {
            ++NoSourceSpecifiedTotal;
        }
    }

    public class AlwaysSuccessDirectoryCopier : IDirectoryCopier
    {
        public DirectoryCopierResult CopyDirectory(IFileSystem fileSystem, IWriter writer, string sourceDirectoryPath, string destDirectoryPath)
        {
            return new DirectoryCopierResult { Success = true };
        }
    }

    public class AlwaysFalseDirectoryCopier : IDirectoryCopier
    {
        public DirectoryCopierResult CopyDirectory(IFileSystem fileSystem, IWriter writer, string sourceDirectoryPath, string destDirectoryPath)
        {
            return new DirectoryCopierResult { Success = false };
        }
    }

    [TestClass]
    public class TestCopyDirectoryInstallStep
    {
        static readonly IWriter NullWriter = new NullWriter();
        static readonly IConfiguration NullConfiguration = new NullConfiguration();
        static readonly IDirectoryCopier AlwaysSuccessDirectoryCopier = new AlwaysSuccessDirectoryCopier();
        static readonly IDirectoryCopier AlwaysFailureDirectoryCopier = new AlwaysFalseDirectoryCopier();

        [TestMethod]
        public void NoSourceSpecifiedReturnsFailed()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestCopyDirectoryInstallStepEventHandler();
            var step = new CopyDirectoryInstallStep(AlwaysSuccessDirectoryCopier, eventHandler);
            var stepData = new CopyDirectoryInstallStepData()
            {
                Source = null,
                Destination = "C:/destination"
            };

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Failed, result);
            Assert.AreEqual(0, eventHandler.BlankDestinationEntryTotal);
            Assert.AreEqual(0, eventHandler.BlankSourceEntryTotal);
            Assert.AreEqual(0, eventHandler.DirectoryCopyFailedTotal);
            Assert.AreEqual(0, eventHandler.DirectoryCopySuccessTotal);
            Assert.AreEqual(1, eventHandler.NoSourceSpecifiedTotal);
            Assert.AreEqual(0, eventHandler.NoDestinationSpecifiedTotal);
        }


        [TestMethod]
        public void BlankSourceEntryReturnsFailed()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestCopyDirectoryInstallStepEventHandler();
            var step = new CopyDirectoryInstallStep(AlwaysSuccessDirectoryCopier, eventHandler);
            var stepData = new CopyDirectoryInstallStepData()
            {
                Source = string.Empty,
                Destination = "C:/destination"
            };

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Failed, result);
            Assert.AreEqual(0, eventHandler.BlankDestinationEntryTotal);
            Assert.AreEqual(1, eventHandler.BlankSourceEntryTotal);
            Assert.AreEqual(0, eventHandler.DirectoryCopyFailedTotal);
            Assert.AreEqual(0, eventHandler.DirectoryCopySuccessTotal);
            Assert.AreEqual(0, eventHandler.NoSourceSpecifiedTotal);
            Assert.AreEqual(0, eventHandler.NoDestinationSpecifiedTotal);
        }

        [TestMethod]
        public void NoDestinationSpecifiedReturnsFailed()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestCopyDirectoryInstallStepEventHandler();
            var step = new CopyDirectoryInstallStep(AlwaysSuccessDirectoryCopier, eventHandler);
            var stepData = new CopyDirectoryInstallStepData()
            {
                Source = "C:/source",
                Destination = null
            };

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Failed, result);
            Assert.AreEqual(0, eventHandler.BlankDestinationEntryTotal);
            Assert.AreEqual(0, eventHandler.BlankSourceEntryTotal);
            Assert.AreEqual(0, eventHandler.DirectoryCopyFailedTotal);
            Assert.AreEqual(0, eventHandler.DirectoryCopySuccessTotal);
            Assert.AreEqual(0, eventHandler.NoSourceSpecifiedTotal);
            Assert.AreEqual(1, eventHandler.NoDestinationSpecifiedTotal);
        }

        [TestMethod]
        public void BlankDestinationEntryReturnsFailed()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestCopyDirectoryInstallStepEventHandler();
            var step = new CopyDirectoryInstallStep(AlwaysSuccessDirectoryCopier, eventHandler);
            var stepData = new CopyDirectoryInstallStepData()
            {
                Source = "C:/source",
                Destination = string.Empty
            };

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Failed, result);
            Assert.AreEqual(1, eventHandler.BlankDestinationEntryTotal);
            Assert.AreEqual(0, eventHandler.BlankSourceEntryTotal);
            Assert.AreEqual(0, eventHandler.DirectoryCopyFailedTotal);
            Assert.AreEqual(0, eventHandler.DirectoryCopySuccessTotal);
            Assert.AreEqual(0, eventHandler.NoSourceSpecifiedTotal);
            Assert.AreEqual(0, eventHandler.NoDestinationSpecifiedTotal);
        }

        [TestMethod]
        public void DirectoryCopySuccess()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestCopyDirectoryInstallStepEventHandler();
            var step = new CopyDirectoryInstallStep(AlwaysSuccessDirectoryCopier, eventHandler);
            var stepData = new CopyDirectoryInstallStepData()
            {
                Source = "C:/source",
                Destination = "C:/destination"
            };

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Complete, result);
            Assert.AreEqual(0, eventHandler.BlankDestinationEntryTotal);
            Assert.AreEqual(0, eventHandler.BlankSourceEntryTotal);
            Assert.AreEqual(0, eventHandler.DirectoryCopyFailedTotal);
            Assert.AreEqual(1, eventHandler.DirectoryCopySuccessTotal);
            Assert.AreEqual(0, eventHandler.NoSourceSpecifiedTotal);
            Assert.AreEqual(0, eventHandler.NoDestinationSpecifiedTotal);
        }

        [TestMethod]
        public void DirectoryCopyFailed()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestCopyDirectoryInstallStepEventHandler();
            var step = new CopyDirectoryInstallStep(AlwaysFailureDirectoryCopier, eventHandler);
            var stepData = new CopyDirectoryInstallStepData()
            {
                Source = "C:/source",
                Destination = "C:/destination"
            };

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Failed, result);
            Assert.AreEqual(0, eventHandler.BlankDestinationEntryTotal);
            Assert.AreEqual(0, eventHandler.BlankSourceEntryTotal);
            Assert.AreEqual(1, eventHandler.DirectoryCopyFailedTotal);
            Assert.AreEqual(0, eventHandler.DirectoryCopySuccessTotal);
            Assert.AreEqual(0, eventHandler.NoSourceSpecifiedTotal);
            Assert.AreEqual(0, eventHandler.NoDestinationSpecifiedTotal);
        }

        [TestMethod]
        public void StepsLoader_Load_Simple()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>{
                { "C:/step_copy_directory.json", new MockFileData(File.ReadAllBytes("../../../data/config/step_copy_directory.json")) },
            });

            var stepsLoader = new StepsLoader<JSONInstallStep>(fileSystem, NullWriter, new JSONConfigurationSerializer<IList<JSONInstallStep>>(), new SourceModPatcher.InstallStepMapper<JSONInstallStep>());

            var stepsList = stepsLoader.Load("C:/step_copy_directory.json");

            Assert.IsNotNull(stepsList);
            var stepData = (CopyDirectoryInstallStepData)stepsList[0];
            Assert.AreEqual("step_copy_directory", stepData.Name);
            Assert.AreEqual("Copy sourcemod directory", stepData.Description);
            CollectionAssert.AreEquivalent(new List<string> { "previous_step1" }, stepData.DependsOn);

            Assert.AreEqual("C:/source", stepData.Source);
            Assert.AreEqual("C:/destination", stepData.Destination);
        }
    }
}