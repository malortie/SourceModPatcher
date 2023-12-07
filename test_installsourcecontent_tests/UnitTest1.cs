using System.IO.Abstractions.TestingHelpers;
using test_installsourcecontent;

namespace test_installsourcecontent_tests
{
    [TestClass]
    public class TestVPKExtractor
    {

        [TestMethod]
        public void TestVPKExtractorTest()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { "C:/vpks/simple_vpk.vpk", new MockFileData(File.ReadAllBytes("../../../data/vpks/simple_vpk.vpk")) }
            });

            var extractor = new VPKExtractor();
            extractor.Extract(fileSystem, new NullWriter(), "C:/vpks/simple_vpk.vpk", "C:/output");
            var expected = new string[] {
                "C:/output/file1.txt",
                "C:/output/model.mdl",
                "C:/output/sound.wav",
            };
            var actual = fileSystem.Directory.GetFiles("C:/output").Select(a => PathExtensions.ConvertToUnixDirectorySeparator(fileSystem, a)).ToArray();
            CollectionAssert.AreEquivalent(expected, actual);
        }
    }
}