using Pipelines;
using SourceContentInstaller;
using System.Collections.ObjectModel;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;

namespace SourceModPatcher.Tests
{
    public class TestReplaceTokensInstallStepEventHandler : IReplaceTokensInstallStepEventHandler
    {
        public int NoFilesSpecifiedTotal { get; private set; } = 0;
        public int FileDoesNotExistTotal { get; private set; } = 0;
        public int FileTokenReplacementSucceededTotal { get; private set; } = 0;
        public int FileTokenReplacementFailedTotal { get; private set; } = 0;
        public int NoFilesProcessedTotal { get; private set; } = 0;

        public void NoFilesSpecified()
        {
            ++NoFilesSpecifiedTotal;
        }
        public void FileDoesNotExist()
        {
            ++FileDoesNotExistTotal;
        }
        public void FileTokenReplacementSucceeded()
        {
            ++FileTokenReplacementSucceededTotal;
        }
        public void FileTokenReplacementFailed()
        {
            ++FileTokenReplacementFailedTotal;
        }
        public void NoFilesProcessed()
        {
            ++NoFilesProcessedTotal;
        }
    }

    public class AlwaysTrueFileTokenReplacer : IFileTokenReplacer
    {
        public ReadOnlyDictionary<string, string> Variables { get; set; } = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>());

        public bool ReplaceInFile(IFileSystem fileSystem, IWriter writer, string filePath)
        {
            return true;
        }
    }

    public class AlwaysFalseFileTokenReplacer : IFileTokenReplacer
    {
        public ReadOnlyDictionary<string, string> Variables { get; set; } = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>());

        public bool ReplaceInFile(IFileSystem fileSystem, IWriter writer, string filePath)
        {
            return false;
        }
    }

    public class MockFileTokenReplacer : IFileTokenReplacer
    {
        public ReadOnlyDictionary<string, string> Variables { get; set; } = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>());

        public List<string> TrueOnFiles { get; set; } = [];
        public List<string> FalseOnFiles { get; set; } = [];

        public bool ReplaceInFile(IFileSystem fileSystem, IWriter writer, string filePath)
        {
            if (TrueOnFiles.Contains(filePath))
                return true;
            if (FalseOnFiles.Contains(filePath))
                return false;

            throw new Exception($"{filePath} does not appear in any list.");
        }
    }

    [TestClass]
    public class TestReplaceTokensInstallStep
    {
        static IWriter NullWriter = new NullWriter();
        static IConfiguration NullConfiguration = new NullConfiguration();
        static IFileTokenReplacer AlwaysTrueFileTokenReplacer = new AlwaysTrueFileTokenReplacer();
        static IFileTokenReplacer AlwaysFalseFileTokenReplacer = new AlwaysFalseFileTokenReplacer();

        [TestMethod]
        public void EmptyFilesListReturnsFailed()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestReplaceTokensInstallStepEventHandler();
            var step = new ReplaceTokensInstallStep(AlwaysTrueFileTokenReplacer, eventHandler);
            var stepData = new ReplaceTokensInstallStepData();

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Failed, result);
            Assert.AreEqual(1, eventHandler.NoFilesSpecifiedTotal);
        }

        [TestMethod]
        public void AllFilesNotFoundReturnsFailed()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });

            var eventHandler = new TestReplaceTokensInstallStepEventHandler();
            var step = new ReplaceTokensInstallStep(AlwaysTrueFileTokenReplacer, eventHandler);
            var stepData = new ReplaceTokensInstallStepData()
            {
                Files = [
                    "C:/test1.txt",
                    "C:/test2.txt",
                    "C:/test3.txt"
                ]
            };

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Failed, result);
            Assert.AreEqual(3, eventHandler.FileDoesNotExistTotal);
            Assert.AreEqual(0, eventHandler.FileTokenReplacementSucceededTotal);
            Assert.AreEqual(1, eventHandler.NoFilesProcessedTotal);
        }

        [TestMethod]
        public void AtLeastOneFileFoundAndProcessedReturnsPartiallyCompleted()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>{
                { "C:/test1.txt", new MockFileData("") }
            });

            var eventHandler = new TestReplaceTokensInstallStepEventHandler();
            var step = new ReplaceTokensInstallStep(AlwaysTrueFileTokenReplacer, eventHandler);
            var stepData = new ReplaceTokensInstallStepData()
            {
                Files = [
                    "C:/test1.txt",
                    "test2",
                    "test3"
                ]
            };

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.PartiallyComplete, result);
            Assert.AreEqual(2, eventHandler.FileDoesNotExistTotal);
            Assert.AreEqual(1, eventHandler.FileTokenReplacementSucceededTotal);
            Assert.AreEqual(0, eventHandler.NoFilesProcessedTotal);
        }

        [TestMethod]
        public void AllFilesFoundAndProcessedReturnsComplete()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>{
                { "C:/test1.txt", new MockFileData("") },
                { "C:/test2.txt", new MockFileData("") },
                { "C:/test3.txt", new MockFileData("") }
            });

            var eventHandler = new TestReplaceTokensInstallStepEventHandler();
            var step = new ReplaceTokensInstallStep(AlwaysTrueFileTokenReplacer, eventHandler);
            var stepData = new ReplaceTokensInstallStepData()
            {
                Files = [
                    "C:/test1.txt",
                    "C:/test2.txt",
                    "C:/test3.txt"
                ]
            };

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Complete, result);
            Assert.AreEqual(0, eventHandler.FileDoesNotExistTotal);
            Assert.AreEqual(3, eventHandler.FileTokenReplacementSucceededTotal);
            Assert.AreEqual(0, eventHandler.NoFilesProcessedTotal);
        }

        [TestMethod]
        public void OnlyOneFile_ReplaceTokensSuccessfulReturnsCompleted()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>{
                { "C:/test1.txt", new MockFileData("") }
            });

            var eventHandler = new TestReplaceTokensInstallStepEventHandler();
            var step = new ReplaceTokensInstallStep(AlwaysTrueFileTokenReplacer, eventHandler);
            var stepData = new ReplaceTokensInstallStepData()
            {
                Files = [
                    "C:/test1.txt"
                ]
            };

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Complete, result);
            Assert.AreEqual(1, eventHandler.FileTokenReplacementSucceededTotal);
            Assert.AreEqual(0, eventHandler.FileTokenReplacementFailedTotal);
            Assert.AreEqual(0, eventHandler.NoFilesProcessedTotal);
        }

        [TestMethod]
        public void OnlyOneFile_ReplaceTokensFailedReturnsFailed()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>{
                { "C:/test1.txt", new MockFileData("") }
            });

            var eventHandler = new TestReplaceTokensInstallStepEventHandler();
            var step = new ReplaceTokensInstallStep(AlwaysFalseFileTokenReplacer, eventHandler);
            var stepData = new ReplaceTokensInstallStepData()
            {
                Files = [
                  "C:/test1.txt"
                ]
            };

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Failed, result);
            Assert.AreEqual(0, eventHandler.FileTokenReplacementSucceededTotal);
            Assert.AreEqual(1, eventHandler.FileTokenReplacementFailedTotal);
            Assert.AreEqual(1, eventHandler.NoFilesProcessedTotal);
        }

        [TestMethod]
        public void TwoFiles_ReplaceTokens_Successful_Successful_ReturnsCompleted()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>{
                { "C:/test1.txt", new MockFileData("") },
                { "C:/test2.txt", new MockFileData("") }
            });

            var eventHandler = new TestReplaceTokensInstallStepEventHandler();
            var step = new ReplaceTokensInstallStep(new MockFileTokenReplacer()
            {
                TrueOnFiles = ["C:/test1.txt", "C:/test2.txt"]
            }, eventHandler);
            var stepData = new ReplaceTokensInstallStepData()
            {
                Files = [
                    "C:/test1.txt",
                    "C:/test2.txt"
                ]
            };

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Complete, result);
            Assert.AreEqual(0, eventHandler.FileDoesNotExistTotal);
            Assert.AreEqual(2, eventHandler.FileTokenReplacementSucceededTotal);
            Assert.AreEqual(0, eventHandler.FileTokenReplacementFailedTotal);
            Assert.AreEqual(0, eventHandler.NoFilesProcessedTotal);
        }

        [TestMethod]
        public void TwoFiles_ReplaceTokens_Successful_Failed_ReturnsPartiallyCompleted()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>{
                { "C:/test1.txt", new MockFileData("") },
                { "C:/test2.txt", new MockFileData("") }
            });

            var eventHandler = new TestReplaceTokensInstallStepEventHandler();
            var step = new ReplaceTokensInstallStep(new MockFileTokenReplacer()
            {
                TrueOnFiles = ["C:/test1.txt"],
                FalseOnFiles = ["C:/test2.txt"]
            }, eventHandler);
            var stepData = new ReplaceTokensInstallStepData()
            {
                Files = [
                    "C:/test1.txt",
                    "C:/test2.txt"
                ]
            };

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.PartiallyComplete, result);
            Assert.AreEqual(0, eventHandler.FileDoesNotExistTotal);
            Assert.AreEqual(1, eventHandler.FileTokenReplacementSucceededTotal);
            Assert.AreEqual(1, eventHandler.FileTokenReplacementFailedTotal);
            Assert.AreEqual(0, eventHandler.NoFilesProcessedTotal);
        }

        [TestMethod]
        public void TwoFiles_ReplaceTokens_Failed_Failed_ReturnsFailed()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>{
                { "C:/test1.txt", new MockFileData("") },
                { "C:/test2.txt", new MockFileData("") }
            });

            var eventHandler = new TestReplaceTokensInstallStepEventHandler();
            var step = new ReplaceTokensInstallStep(new MockFileTokenReplacer()
            {
                FalseOnFiles = ["C:/test1.txt", "C:/test2.txt"]
            }, eventHandler);
            var stepData = new ReplaceTokensInstallStepData()
            {
                Files = [
                    "C:/test1.txt",
                    "C:/test2.txt"
                ]
            };

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Failed, result);
            Assert.AreEqual(0, eventHandler.FileDoesNotExistTotal);
            Assert.AreEqual(0, eventHandler.FileTokenReplacementSucceededTotal);
            Assert.AreEqual(2, eventHandler.FileTokenReplacementFailedTotal);
            Assert.AreEqual(1, eventHandler.NoFilesProcessedTotal);
        }

        [TestMethod]
        public void StepsLoader_Load_Simple()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>{
                { "C:/step_replace_tokens.json", new MockFileData(File.ReadAllBytes("../../../data/config/step_replace_tokens.json")) },
            });

            var stepsLoader = new StepsLoader<JSONInstallStep>(fileSystem, NullWriter, new JSONConfigurationSerializer<IList<JSONInstallStep>>(), new SourceModPatcher.InstallStepMapper<JSONInstallStep>());

            var stepsList = stepsLoader.Load("C:/step_replace_tokens.json");

            Assert.IsNotNull(stepsList);
            var stepData = (ReplaceTokensInstallStepData)stepsList[0];
            Assert.AreEqual("step_replace_tokens", stepData.Name);
            Assert.AreEqual("Replace tokens in sourcemod files", stepData.Description);
            CollectionAssert.AreEquivalent(new List<string> { "previous_step1" }, stepData.DependsOn);

            Assert.IsNotNull(stepData.Files);
            Assert.AreEqual(2, stepData.Files.Count);
            CollectionAssert.AreEquivalent(new List<string> { "C:/file1.txt", "C:/file2.txt" }, stepData.Files);
        }
    }
}