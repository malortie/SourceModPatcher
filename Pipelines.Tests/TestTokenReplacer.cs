using System.Collections.ObjectModel;

namespace Pipelines.Tests
{
    [TestClass]
    public class TestTokenReplacer
    {
        [TestMethod]
        public void Replace_SameStringWhenNoVariablesAreProvided()
        {
            var tokenReplacer = new TokenReplacer
            {
                Prefix = "$(",
                Suffix = ")",
            };

            Assert.AreEqual("$(token_1) $(token_2)", tokenReplacer.Replace("$(token_1) $(token_2)"));
        }

        [TestMethod]
        public void Replace_WithNoMissingToken()
        {
            var tokenReplacer = new TokenReplacer
            {
                Prefix = "$(",
                Suffix = ")",
                Variables = new ReadOnlyDictionary<string, string>(new Dictionary<string, string> {
                    { "token_1", "hello" },
                    { "token_2", "world" }
                })
            };

            Assert.AreEqual("hello world", tokenReplacer.Replace("$(token_1) $(token_2)"));
        }

        [TestMethod]
        public void Replace_WithOneMissingToken()
        {
            var tokenReplacer = new TokenReplacer
            {
                Prefix = "$(",
                Suffix = ")",
                Variables = new ReadOnlyDictionary<string, string>(new Dictionary<string, string> {
                    { "token_1", "hello" }
                })
            };

            Assert.AreEqual("hello $(token_2)", tokenReplacer.Replace("$(token_1) $(token_2)"));
        }

        [TestMethod]
        public void Replace_WithOneMissingToken_2()
        {
            var tokenReplacer = new TokenReplacer
            {
                Prefix = "$(",
                Suffix = ")",
                Variables = new ReadOnlyDictionary<string, string>(new Dictionary<string, string> {
                    { "token_2", "world" }
                })
            };

            Assert.AreEqual("$(token_1) world", tokenReplacer.Replace("$(token_1) $(token_2)"));
        }

        [TestMethod]
        public void Replace_WithTokenDefinedMultipleTimes()
        {
            var tokenReplacer = new TokenReplacer
            {
                Prefix = "$(",
                Suffix = ")",
                Variables = new ReadOnlyDictionary<string, string>(new Dictionary<string, string> {
                    { "token_1", "hello" }
                })
            };

            const string text = "$(token_1) $(token_1) $(token_1)";
            Assert.AreEqual("hello hello hello", tokenReplacer.Replace(text));
        }

        [TestMethod]
        public void Replace_WithMalformedToken_SpaceOnLeft()
        {
            var tokenReplacer = new TokenReplacer
            {
                Prefix = "$(",
                Suffix = ")",
                Variables = new ReadOnlyDictionary<string, string>(new Dictionary<string, string> {
                    { "token_1", "hello" }
                })
            };

            Assert.AreEqual("$( token_1)", tokenReplacer.Replace("$( token_1)"));
        }

        [TestMethod]
        public void Replace_WithMalformedToken_SpaceOnRight()
        {
            var tokenReplacer = new TokenReplacer
            {
                Prefix = "$(",
                Suffix = ")",
                Variables = new ReadOnlyDictionary<string, string>(new Dictionary<string, string> {
                    { "token_1", "hello" }
                })
            };

            Assert.AreEqual("$(token_1 )", tokenReplacer.Replace("$(token_1 )"));
        }

        [TestMethod]
        public void Replace_WithMalformedToken_SpaceAround()
        {
            var tokenReplacer = new TokenReplacer
            {
                Prefix = "$(",
                Suffix = ")",
                Variables = new ReadOnlyDictionary<string, string>(new Dictionary<string, string> {
                    { "token_1", "hello" }
                })
            };

            Assert.AreEqual("$( token_1 )", tokenReplacer.Replace("$( token_1 )"));
        }
    }
}