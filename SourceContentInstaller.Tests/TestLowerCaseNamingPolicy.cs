using System.Text;

namespace SourceContentInstaller.Tests
{
    [TestClass]
    public class TestLowerCaseNamingPolicy
    {
        [TestMethod]
        public void ConvertName_ShouldBeLowerCase_WhenInputIsUpperCase()
        {
            const string input = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var namingPolicy = new LowerCaseNamingPolicy();
            Assert.AreEqual(input.ToLower(), namingPolicy.ConvertName(input));
        }

        [TestMethod]
        public void ConvertName_ShouldBeLowerCase_WhenInputIsLowerCase()
        {
            const string input = "abcdefghijklmnopqrstuvwxyz";
            var namingPolicy = new LowerCaseNamingPolicy();
            Assert.AreEqual(input, namingPolicy.ConvertName(input));
        }
    }
}