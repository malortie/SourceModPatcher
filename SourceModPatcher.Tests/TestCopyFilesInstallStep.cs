using Pipelines;
using SourceContentInstaller;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;

namespace SourceModPatcher.Tests
{
    public class TestCopyFilesInstallStepEventHandler : ICopyFilesInstallStepEventHandler
    {
        public int NoFilesSpecifiedTotal { get; private set; } = 0;
        public int BlankSourceEntryTotal { get; private set; } = 0;
        public int BlankDestinationEntryTotal { get; private set; } = 0;
        public int SourceFileDoesNotExistTotal { get; private set; } = 0;
        public int FileCopySuccessTotal { get; private set; } = 0;
        public int FileCopyFailedTotal { get; private set; } = 0;
        public int NoFilesCopiedTotal { get; private set; } = 0;

        public void NoFilesSpecified()
        {
            ++NoFilesSpecifiedTotal;
        }
        public void BlankSourceEntry()
        {
            ++BlankSourceEntryTotal;
        }
        public void BlankDestinationEntry()
        {
            ++BlankDestinationEntryTotal;
        }
        public void SourceFileDoesNotExist()
        {
            ++SourceFileDoesNotExistTotal;
        }
        public void FileCopySuccess()
        {
            ++FileCopySuccessTotal;
        }
        public void FileCopyFailed()
        {
            ++FileCopyFailedTotal;
        }
        public void NoFilesCopied()
        {
            ++NoFilesCopiedTotal;
        }
    }

    public class AlwaysTrueFileCopier : IFileCopier
    {
        public bool CopyFile(IFileSystem fileSystem, IWriter writer, string sourceFilePath, string destFilePath)
        {
            return true;
        }
    }

    public class AlwaysFalseFileCopier : IFileCopier
    {
        public bool CopyFile(IFileSystem fileSystem, IWriter writer, string sourceFilePath, string destFilePath)
        {
            return false;
        }
    }

    public class MockFileCopier : IFileCopier
    {
        public List<string> TrueOnFiles { get; set; } = [];
        public List<string> FalseOnFiles { get; set; } = [];

        public bool CopyFile(IFileSystem fileSystem, IWriter writer, string sourceFilePath, string destFilePath)
        {
            if (TrueOnFiles.Contains(sourceFilePath))
                return true;
            if (FalseOnFiles.Contains(sourceFilePath))
                return false;

            throw new Exception($"{sourceFilePath} does not appear in any list.");
        }
    }

    [TestClass]
    public class TestCopyFilesInstallStep
    {
        static readonly IWriter NullWriter = new NullWriter();
        static readonly IConfiguration NullConfiguration = new NullConfiguration();
        static readonly IFileCopier AlwaysTrueFileCopier = new AlwaysTrueFileCopier();
        static readonly IFileCopier AlwaysFalseFileCopier = new AlwaysFalseFileCopier();

        [TestMethod]
        public void EmptyFilesListReturnsFailed()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestCopyFilesInstallStepEventHandler();
            var step = new CopyFilesInstallStep(AlwaysTrueFileCopier, eventHandler);
            var stepData = new CopyFilesInstallStepData();

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Failed, result);
            Assert.AreEqual(1, eventHandler.NoFilesSpecifiedTotal);
        }

        [TestMethod]
        public void BlankSourceFileEntryReturnsFailed()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestCopyFilesInstallStepEventHandler();
            var step = new CopyFilesInstallStep(AlwaysTrueFileCopier, eventHandler);
            var stepData = new CopyFilesInstallStepData()
            {
                Files = [
                    new CopyFilesInstallStepDataFile { Source = string.Empty, Destination = "test" }
                ]
            };

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Failed, result);
            Assert.AreEqual(1, eventHandler.BlankSourceEntryTotal);
            Assert.AreEqual(0, eventHandler.BlankDestinationEntryTotal);
        }

        [TestMethod]
        public void BlankDestinationFileEntryReturnsFailed()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestCopyFilesInstallStepEventHandler();
            var step = new CopyFilesInstallStep(AlwaysTrueFileCopier, eventHandler);
            var stepData = new CopyFilesInstallStepData()
            {
                Files = [
                    new CopyFilesInstallStepDataFile { Source = "test", Destination = string.Empty }
                ]
            };

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Failed, result);
            Assert.AreEqual(0, eventHandler.BlankSourceEntryTotal);
            Assert.AreEqual(1, eventHandler.BlankDestinationEntryTotal);
        }

        [TestMethod]
        public void AllFilesNotFoundReturnsFailed()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });

            var eventHandler = new TestCopyFilesInstallStepEventHandler();
            var step = new CopyFilesInstallStep(AlwaysTrueFileCopier, eventHandler);
            var stepData = new CopyFilesInstallStepData()
            {
                Files = [
                    new CopyFilesInstallStepDataFile { Source = "src_file_1", Destination = "dst_file_1" },
                    new CopyFilesInstallStepDataFile { Source = "src_file_2", Destination = "dst_file_2" },
                    new CopyFilesInstallStepDataFile { Source = "src_file_3", Destination = "dst_file_3" },
                ]
            };

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Failed, result);
            Assert.AreEqual(3, eventHandler.SourceFileDoesNotExistTotal);
            Assert.AreEqual(0, eventHandler.FileCopySuccessTotal);
            Assert.AreEqual(1, eventHandler.NoFilesCopiedTotal);
        }

        [TestMethod]
        public void AtLeastOneFileFoundAndCopiedReturnsPartiallyCompleted()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>{
                { "C:/source/test1.txt", new MockFileData("") },
                { "C:/destination", new MockDirectoryData() }
            });

            var eventHandler = new TestCopyFilesInstallStepEventHandler();
            var step = new CopyFilesInstallStep(AlwaysTrueFileCopier, eventHandler);
            var stepData = new CopyFilesInstallStepData()
            {
                Files = [
                    new CopyFilesInstallStepDataFile { Source = "C:/source/test1.txt", Destination = "C:/destination/test1.txt" },
                    new CopyFilesInstallStepDataFile { Source = "test2", Destination = "C:/destination/test2.txt" },
                    new CopyFilesInstallStepDataFile { Source = "test3", Destination = "C:/destination/test3.txt" },
                ]
            };

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.PartiallyComplete, result);
            Assert.AreEqual(2, eventHandler.SourceFileDoesNotExistTotal);
            Assert.AreEqual(1, eventHandler.FileCopySuccessTotal);
            Assert.AreEqual(0, eventHandler.NoFilesCopiedTotal);
        }

        [TestMethod]
        public void AllFilesFoundAndCopiedReturnsComplete()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>{
                { "C:/source/test1.txt", new MockFileData("") },
                { "C:/source/test2.txt", new MockFileData("") },
                { "C:/source/test3.txt", new MockFileData("") },
                { "C:/destination", new MockDirectoryData() }
            });

            var eventHandler = new TestCopyFilesInstallStepEventHandler();
            var step = new CopyFilesInstallStep(AlwaysTrueFileCopier, eventHandler);
            var stepData = new CopyFilesInstallStepData()
            {
                Files = [
                    new CopyFilesInstallStepDataFile { Source = "C:/source/test1.txt", Destination = "C:/destination/test1.txt" },
                    new CopyFilesInstallStepDataFile { Source = "C:/source/test2.txt", Destination = "C:/destination/test2.txt" },
                    new CopyFilesInstallStepDataFile { Source = "C:/source/test3.txt", Destination = "C:/destination/test3.txt" },
                ]
            };

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Complete, result);
            Assert.AreEqual(0, eventHandler.SourceFileDoesNotExistTotal);
            Assert.AreEqual(3, eventHandler.FileCopySuccessTotal);
            Assert.AreEqual(0, eventHandler.NoFilesCopiedTotal);
        }

        [TestMethod]
        public void OnlyOneFile_CopySuccessfulReturnsCompleted()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>{
                { "C:/source/test1.txt", new MockFileData("") },
            });

            var eventHandler = new TestCopyFilesInstallStepEventHandler();
            var step = new CopyFilesInstallStep(AlwaysTrueFileCopier, eventHandler);
            var stepData = new CopyFilesInstallStepData()
            {
                Files = [
                new CopyFilesInstallStepDataFile { Source = "C:/source/test1.txt", Destination = "C:/destination/test1.txt" }
              ]
            };

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Complete, result);
            Assert.AreEqual(1, eventHandler.FileCopySuccessTotal);
            Assert.AreEqual(0, eventHandler.FileCopyFailedTotal);
            Assert.AreEqual(0, eventHandler.NoFilesCopiedTotal);
        }

        [TestMethod]
        public void OnlyOneFile_CopyFailedReturnsFailed()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>{
                { "C:/source/test1.txt", new MockFileData("") },
            });

            var eventHandler = new TestCopyFilesInstallStepEventHandler();
            var step = new CopyFilesInstallStep(AlwaysFalseFileCopier, eventHandler);
            var stepData = new CopyFilesInstallStepData()
            {
                Files = [
                new CopyFilesInstallStepDataFile { Source = "C:/source/test1.txt", Destination = "C:/destination/test1.txt" }
              ]
            };

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Failed, result);
            Assert.AreEqual(0, eventHandler.FileCopySuccessTotal);
            Assert.AreEqual(1, eventHandler.FileCopyFailedTotal);
            Assert.AreEqual(1, eventHandler.NoFilesCopiedTotal);
        }

        [TestMethod]
        public void TwoFiles_Copy_Successful_Successful_ReturnsCompleted()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>{
                { "C:/source/test1.txt", new MockFileData("") },
                { "C:/source/test2.txt", new MockFileData("") },
            });

            var eventHandler = new TestCopyFilesInstallStepEventHandler();
            var step = new CopyFilesInstallStep(new MockFileCopier()
            {
                TrueOnFiles = ["C:/source/test1.txt", "C:/source/test2.txt"]
            }, eventHandler);
            var stepData = new CopyFilesInstallStepData()
            {
                Files = [
                    new CopyFilesInstallStepDataFile { Source = "C:/source/test1.txt", Destination = "C:/destination/test1.txt" },
                    new CopyFilesInstallStepDataFile { Source = "C:/source/test2.txt", Destination = "C:/destination/test2.txt" }
                ]
            };

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Complete, result);
            Assert.AreEqual(0, eventHandler.SourceFileDoesNotExistTotal);
            Assert.AreEqual(2, eventHandler.FileCopySuccessTotal);
            Assert.AreEqual(0, eventHandler.FileCopyFailedTotal);
            Assert.AreEqual(0, eventHandler.NoFilesCopiedTotal);
        }

        [TestMethod]
        public void TwoFiles_Copy_Successful_Failed_ReturnsPartiallyCompleted()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>{
                { "C:/source/test1.txt", new MockFileData("") },
                { "C:/source/test2.txt", new MockFileData("") },
            });

            var eventHandler = new TestCopyFilesInstallStepEventHandler();
            var step = new CopyFilesInstallStep(new MockFileCopier()
            {
                TrueOnFiles = ["C:/source/test1.txt"],
                FalseOnFiles = ["C:/source/test2.txt"]
            }, eventHandler);
            var stepData = new CopyFilesInstallStepData()
            {
                Files = [
                    new CopyFilesInstallStepDataFile { Source = "C:/source/test1.txt", Destination = "C:/destination/test1.txt" },
                    new CopyFilesInstallStepDataFile { Source = "C:/source/test2.txt", Destination = "C:/destination/test2.txt" }
                ]
            };

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.PartiallyComplete, result);
            Assert.AreEqual(0, eventHandler.SourceFileDoesNotExistTotal);
            Assert.AreEqual(1, eventHandler.FileCopySuccessTotal);
            Assert.AreEqual(1, eventHandler.FileCopyFailedTotal);
            Assert.AreEqual(0, eventHandler.NoFilesCopiedTotal);
        }

        [TestMethod]
        public void TwoFiles_Copy_Failed_Failed_ReturnsFailed()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>{
                { "C:/source/test1.txt", new MockFileData("") },
                { "C:/source/test2.txt", new MockFileData("") },
            });

            var eventHandler = new TestCopyFilesInstallStepEventHandler();
            var step = new CopyFilesInstallStep(new MockFileCopier()
            {
                FalseOnFiles = ["C:/source/test1.txt", "C:/source/test2.txt"]
            }, eventHandler);
            var stepData = new CopyFilesInstallStepData()
            {
                Files = [
                    new CopyFilesInstallStepDataFile { Source = "C:/source/test1.txt", Destination = "C:/destination/test1.txt" },
                    new CopyFilesInstallStepDataFile { Source = "C:/source/test2.txt", Destination = "C:/destination/test2.txt" }
                ]
            };

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Failed, result);
            Assert.AreEqual(0, eventHandler.SourceFileDoesNotExistTotal);
            Assert.AreEqual(0, eventHandler.FileCopySuccessTotal);
            Assert.AreEqual(2, eventHandler.FileCopyFailedTotal);
            Assert.AreEqual(1, eventHandler.NoFilesCopiedTotal);
        }

        [TestMethod]
        public void StepsLoader_Load_Simple()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>{
                { "C:/step_copy_files.json", new MockFileData(File.ReadAllBytes("../../../data/config/step_copy_files.json")) },
            });

            var stepsLoader = new StepsLoader<JSONInstallStep>(fileSystem, NullWriter, new JSONConfigurationSerializer<IList<JSONInstallStep>>(), new SourceModPatcher.InstallStepMapper<JSONInstallStep>());

            var stepsList = stepsLoader.Load("C:/step_copy_files.json");

            Assert.IsNotNull(stepsList);
            var stepData = (CopyFilesInstallStepData)stepsList[0];
            Assert.AreEqual("step_copy_files", stepData.Name);
            Assert.AreEqual("Copy sourcemod files", stepData.Description);
            CollectionAssert.AreEquivalent(new List<string> { "previous_step1" }, stepData.DependsOn);

            Assert.IsNotNull(stepData.Files);
            Assert.AreEqual(2, stepData.Files.Count);

            var file = stepData.Files[0];
            Assert.AreEqual("C:/source/bin/client.dll", file.Source);
            Assert.AreEqual("C:/destination/bin/client.dll", file.Destination);

            file = stepData.Files[1];
            Assert.AreEqual("C:/source/bin/server.dll", file.Source);
            Assert.AreEqual("C:/destination/bin/server.dll", file.Destination);
        }
    }
}