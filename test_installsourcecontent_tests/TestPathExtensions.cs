using System.IO.Abstractions.TestingHelpers;
using test_installsourcecontent;

namespace test_installsourcecontent_tests
{
    [TestClass]
    public class TestPathExtensions
    {
        [TestMethod]
        public void JoinWithSeparator_Two_Strings()
        {
            var fileSystem = new MockFileSystem();
            Assert.AreEqual("hello/world", PathExtensions.JoinWithSeparator(fileSystem, "hello", "world"));
        }

        [TestMethod]
        public void JoinWithSeparator_String_Array()
        {
            var fileSystem = new MockFileSystem();
            Assert.AreEqual("one/two/three/four/five", PathExtensions.JoinWithSeparator(fileSystem, ["one", "two", "three", "four", "five"]));
        }

        [TestMethod]
        public void ConvertToUnixDirectorySeparator_With_ForwardSlashes_Only()
        {
            var fileSystem = new MockFileSystem();
            const string expected = "C:/temp/temp2/temp3/file.txt";
            Assert.AreEqual(expected, PathExtensions.ConvertToUnixDirectorySeparator(fileSystem, expected));
        }

        [TestMethod]
        public void ConvertToUnixDirectorySeparator_With_BackSlashes_Only()
        {
            var fileSystem = new MockFileSystem();
            Assert.AreEqual("C:/temp/temp2/temp3/file.txt", PathExtensions.ConvertToUnixDirectorySeparator(fileSystem, @"C:\temp\temp2\temp3\file.txt"));
        }

        [TestMethod]
        public void ConvertToUnixDirectorySeparator_With_Forward_And_BackSlashes()
        {
            var fileSystem = new MockFileSystem();
            Assert.AreEqual("C:/temp/temp2/temp3/file.txt", PathExtensions.ConvertToUnixDirectorySeparator(fileSystem, @"C:\temp/temp2\temp3/file.txt"));
        }
    }
}