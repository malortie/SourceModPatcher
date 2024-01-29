using System.Collections.ObjectModel;
using Pipelines;
using System.IO.Abstractions.TestingHelpers;

namespace SourceModPatcher.Tests
{
    [TestClass]
    public class TestFileTokenReplacer
    {
        [TestMethod]
        public void ReplaceInFile_Simple()
        {
            const string FILE_PATH = "C:/source/test1.txt";
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { FILE_PATH, new MockFileData("${{token1}} ${{token2}}") },
            });

            var fileTokenCopier = new FileTokenReplacer(new TokenReplacer {
                Prefix = "${{",
                Suffix = "}}"
            });
            fileTokenCopier.Variables = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(){
                {"token1", "hello"},
                {"token2", "world"}
            });

            Assert.IsTrue(fileTokenCopier.ReplaceInFile(fileSystem, new NullWriter(), FILE_PATH));
            Assert.AreEqual("hello world", fileSystem.File.ReadAllText(FILE_PATH));
        }
    }
}