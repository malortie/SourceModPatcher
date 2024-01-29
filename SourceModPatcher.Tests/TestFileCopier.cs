using Pipelines;
using System.IO.Abstractions.TestingHelpers;
using test_installsourcecontent_modpatcher;

namespace test_installsourcecontent_modpatcher_tests
{
    [TestClass]
    public class TestFileCopier
    {
        [TestMethod]
        public void CopyFile_WhenDestinationDirectoryDoesNotExist()
        {
            const string SOURCE_PATH = "C:/source/test1.txt";
            const string DEST_PATH = "C:/destination/test1.txt";

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { SOURCE_PATH, new MockFileData("test1") },
            });

            var fileCopier = new FileCopier();
            fileCopier.CopyFile(fileSystem, new NullWriter(), SOURCE_PATH, DEST_PATH);

            Assert.IsTrue(fileSystem.FileExists(DEST_PATH));
            Assert.AreEqual("test1", fileSystem.File.ReadAllText(DEST_PATH));
        }

        [TestMethod]
        public void CopyFile_WhenDestinationDirectoryExists()
        {
            const string SOURCE_PATH = "C:/source/test1.txt";
            const string DEST_PATH = "C:/destination/test1.txt";

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { SOURCE_PATH, new MockFileData("test1") },
                { "C:/destination", new MockDirectoryData() },
            });

            var fileCopier = new FileCopier();
            fileCopier.CopyFile(fileSystem, new NullWriter(), SOURCE_PATH, DEST_PATH);

            Assert.IsTrue(fileSystem.FileExists(DEST_PATH));
            Assert.AreEqual("test1", fileSystem.File.ReadAllText(DEST_PATH));
        }

        [TestMethod]
        public void CopyFile_WhenDestinationFileExists()
        {
            const string SOURCE_PATH = "C:/source/test1.txt";
            const string DEST_PATH = "C:/destination/test1.txt";

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { SOURCE_PATH, new MockFileData("test1") },
                { DEST_PATH, new MockFileData("random") },
            });

            var fileCopier = new FileCopier();
            fileCopier.CopyFile(fileSystem, new NullWriter(), SOURCE_PATH, DEST_PATH);

            Assert.IsTrue(fileSystem.FileExists(DEST_PATH));
            Assert.AreEqual("test1", fileSystem.File.ReadAllText(DEST_PATH));
        }
    }
}