using Pipelines;
using System.IO.Abstractions.TestingHelpers;

namespace SourceModPatcher.Tests
{
    [TestClass]
    public class TestDirectoryCopier
    {
        const string SOURCE_PATH = "C:/source";
        const string DEST_PATH = "C:/destination";

        readonly IWriter NullWriter = new NullWriter();

        [TestMethod]
        public void CopyDirectory_WhenDestinationDirectoryDoesNotExist()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $"{SOURCE_PATH}/folder1/file1.txt", new MockFileData("folder1_file1") },
                { $"{SOURCE_PATH}/folder1/file2.txt", new MockFileData("folder1_file2") },
                { $"{SOURCE_PATH}/folder1/folder2/file1.txt", new MockFileData("folder2_file1") },
                { $"{SOURCE_PATH}/folder1/folder2/file2.txt", new MockFileData("folder2_file2") },
                { $"{SOURCE_PATH}/folder1/folder2/folder3/file1.txt", new MockFileData("folder3_file1") },
                { $"{SOURCE_PATH}/folder1/folder2/folder3/file2.txt", new MockFileData("folder3_file2") },
            });

            var directoryCopier = new DirectoryCopier();
            var result = directoryCopier.CopyDirectory(fileSystem, NullWriter, SOURCE_PATH, DEST_PATH);
            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.FileCopyResults.All(a => a.Status == DirectoryCopierFileCopyStatus.Ok));
            Assert.AreEqual("folder1_file1", fileSystem.File.ReadAllText($"{DEST_PATH}/folder1/file1.txt"));
            Assert.AreEqual("folder1_file2", fileSystem.File.ReadAllText($"{DEST_PATH}/folder1/file2.txt"));
            Assert.AreEqual("folder2_file1", fileSystem.File.ReadAllText($"{DEST_PATH}/folder1/folder2/file1.txt"));
            Assert.AreEqual("folder2_file2", fileSystem.File.ReadAllText($"{DEST_PATH}/folder1/folder2/file2.txt"));
            Assert.AreEqual("folder3_file1", fileSystem.File.ReadAllText($"{DEST_PATH}/folder1/folder2/folder3/file1.txt"));
            Assert.AreEqual("folder3_file2", fileSystem.File.ReadAllText($"{DEST_PATH}/folder1/folder2/folder3/file2.txt"));
        }

        [TestMethod]
        public void CopyDirectory_WhenDestinationDirectoryExists()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $"{SOURCE_PATH}/folder1/file1.txt", new MockFileData("folder1_file1") },
                { $"{SOURCE_PATH}/folder1/folder2/file1.txt", new MockFileData("folder2_file1") },
                { $"{SOURCE_PATH}/folder1/folder2/folder3/file1.txt", new MockFileData("folder3_file1") },

                { $"{DEST_PATH}/folder1", new MockDirectoryData() },
                { $"{DEST_PATH}/folder1/folder2", new MockDirectoryData() },
                { $"{DEST_PATH}/folder1/folder2/folder3", new MockDirectoryData() },
            });

            var directoryCopier = new DirectoryCopier();
            var result = directoryCopier.CopyDirectory(fileSystem, NullWriter, SOURCE_PATH, DEST_PATH);
            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.FileCopyResults.All(a => a.Status == DirectoryCopierFileCopyStatus.Ok));
            Assert.AreEqual("folder1_file1", fileSystem.File.ReadAllText($"{DEST_PATH}/folder1/file1.txt"));
            Assert.AreEqual("folder2_file1", fileSystem.File.ReadAllText($"{DEST_PATH}/folder1/folder2/file1.txt"));
            Assert.AreEqual("folder3_file1", fileSystem.File.ReadAllText($"{DEST_PATH}/folder1/folder2/folder3/file1.txt"));
        }

        [TestMethod]
        public void CopyDirectory_WhenDestinationFileExists()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $"{SOURCE_PATH}/folder1/file1.txt", new MockFileData("folder1_file1") },
                { $"{SOURCE_PATH}/folder1/folder2/file1.txt", new MockFileData("folder2_file1") },
                { $"{SOURCE_PATH}/folder1/folder2/folder3/file1.txt", new MockFileData("folder3_file1") },

                { $"{DEST_PATH}/folder1/file1.txt", new MockFileData(string.Empty) },
                { $"{DEST_PATH}/folder1/folder2/file1.txt", new MockFileData(string.Empty) },
                { $"{DEST_PATH}/folder1/folder2/folder3/file1.txt", new MockFileData(string.Empty) },
            });

            var directoryCopier = new DirectoryCopier();
            var result = directoryCopier.CopyDirectory(fileSystem, NullWriter, SOURCE_PATH, DEST_PATH);
            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.FileCopyResults.All(a => a.Status == DirectoryCopierFileCopyStatus.Ok));
            Assert.AreEqual("folder1_file1", fileSystem.File.ReadAllText($"{DEST_PATH}/folder1/file1.txt"));
            Assert.AreEqual("folder2_file1", fileSystem.File.ReadAllText($"{DEST_PATH}/folder1/folder2/file1.txt"));
            Assert.AreEqual("folder3_file1", fileSystem.File.ReadAllText($"{DEST_PATH}/folder1/folder2/folder3/file1.txt"));
        }
    }
}