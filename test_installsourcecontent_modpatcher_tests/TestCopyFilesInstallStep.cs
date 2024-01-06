using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Pipelines;
using test_installsourcecontent_modpatcher;

using IConfiguration = test_installsourcecontent_modpatcher.IConfiguration;
using Context = test_installsourcecontent_modpatcher.Context;

namespace test_installsourcecontent_modpatcher_tests
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
        static IWriter NullWriter = new NullWriter();
        static IConfiguration NullConfiguration = new NullConfiguration();
        static IFileCopier DefaultFileCopier = new FileCopier();

        static IFileCopier AlwaysTrueFileCopier = new AlwaysTrueFileCopier();
        static IFileCopier AlwaysFalseFileCopier = new AlwaysFalseFileCopier();

        [TestMethod]
        public void EmptyFilesListReturnsFailed()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestCopyFilesInstallStepEventHandler();
            var step = new CopyFilesInstallStep(DefaultFileCopier, eventHandler);
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
    }
}