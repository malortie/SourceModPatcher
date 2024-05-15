using System.Collections.ObjectModel;

namespace Pipelines.Tests
{
    public class ClassWithStrings
    {
        [PipelineStepReplaceToken]
        public string String1 { get; set; } = string.Empty;
        public string String2 { get; set; } = string.Empty;
        [PipelineStepReplaceToken]
        public string String3 { get; set; } = string.Empty;
    }

    public class ClassWithStringCollections
    {
        [PipelineStepReplaceToken]
        public List<string> StringList1 { get; set; } = [];
        public List<string> StringList2 { get; set; } = [];
        [PipelineStepReplaceToken]
        public List<string> StringList3 { get; set; } = [];
    }

    public class ClassWithCompositeMembers
    {
        [PipelineStepReplaceToken]
        public ClassWithStrings? ClassWithStrings1 { get; set; }
        public ClassWithStrings? ClassWithStrings2 { get; set; }
        [PipelineStepReplaceToken]
        public ClassWithStringCollections? ClassWithStringCollections1 { get; set; }
        public ClassWithStringCollections? ClassWithStringCollections2 { get; set; }
    }

    public class ClassCompositeLeaf
    {
        [PipelineStepReplaceToken]
        public string String1 { get; set; } = string.Empty;
        [PipelineStepReplaceToken]
        public List<string> StringList1 { get; set; } = [];
        [PipelineStepReplaceToken]
        public List<ClassCompositeLeaf> Children { get; set; } = [];
    }

    [TestClass]
    public class TestPipelineStepTokenReplacer
    {
        [TestMethod]
        public void Replace_WithStrings()
        {
            var tokenReplacer = new TokenReplacer
            {
                Prefix = "$(",
                Suffix = ")",
                Variables = new ReadOnlyDictionary<string, string>(new Dictionary<string, string> {
                    { "token_1", "value1" },
                    { "token_2", "value2" },
                    { "token_3", "value3" },
                })
            };

            var classInstance = new ClassWithStrings()
            {
                String1 = "$(token_1)",
                String2 = "$(token_2)",
                String3 = "$(token_3)",
            };
            var stepTokenReplacer = new PipelineStepTokenReplacer();
            stepTokenReplacer.Replace(classInstance, tokenReplacer);

            Assert.AreEqual("value1", classInstance.String1);
            Assert.AreEqual("$(token_2)", classInstance.String2);
            Assert.AreEqual("value3", classInstance.String3);
        }

        [TestMethod]
        public void Replace_WithStringCollections()
        {
            var tokenReplacer = new TokenReplacer
            {
                Prefix = "$(",
                Suffix = ")",
                Variables = new ReadOnlyDictionary<string, string>(new Dictionary<string, string> {
                    { "token_1", "value1" },
                    { "token_2", "value2" },
                    { "token_3", "value3" },
                    { "token_4", "value4" },
                    { "token_5", "value5" },
                    { "token_6", "value6" },
                })
            };

            var classInstance = new ClassWithStringCollections()
            {
                StringList1 = ["$(token_1)", "$(token_2)"],
                StringList2 = ["$(token_3)", "$(token_4)"],
                StringList3 = ["$(token_5)", "$(token_6)"],
            };
            var stepTokenReplacer = new PipelineStepTokenReplacer();
            stepTokenReplacer.Replace(classInstance, tokenReplacer);

            CollectionAssert.AreEquivalent(new List<string> { "value1", "value2" }, classInstance.StringList1);
            CollectionAssert.AreEquivalent(new List<string> { "$(token_3)", "$(token_4)" }, classInstance.StringList2);
            CollectionAssert.AreEquivalent(new List<string> { "value5", "value6" }, classInstance.StringList3);
        }

        [TestMethod]
        public void Replace_WithCompositeMembers()
        {
            var tokenReplacer = new TokenReplacer
            {
                Prefix = "$(",
                Suffix = ")",
                Variables = new ReadOnlyDictionary<string, string>(new Dictionary<string, string> {
                    { "token_1a", "value1a" },
                    { "token_1b", "value1b" },
                    { "token_1c", "value1c" },

                    { "token_2a", "value2a" },
                    { "token_2b", "value2b" },
                    { "token_2c", "value2c" },

                    { "token_3a", "value3a" },
                    { "token_3b", "value3b" },
                    { "token_3c", "value3c" },
                    { "token_3d", "value3d" },
                    { "token_3e", "value3e" },
                    { "token_3f", "value3f" },

                    { "token_4a", "value4a" },
                    { "token_4b", "value4b" },
                    { "token_4c", "value4c" },
                    { "token_4d", "value4d" },
                    { "token_4e", "value4e" },
                    { "token_4f", "value4f" },
                })
            };

            var classInstance = new ClassWithCompositeMembers()
            {
                ClassWithStrings1 = new ClassWithStrings
                {
                    String1 = "$(token_1a)",
                    String2 = "$(token_1b)",
                    String3 = "$(token_1c)"
                },
                ClassWithStrings2 = new ClassWithStrings
                {
                    String1 = "$(token_2a)",
                    String2 = "$(token_2b)",
                    String3 = "$(token_2c)"
                },
                ClassWithStringCollections1 = new ClassWithStringCollections
                {
                    StringList1 = ["$(token_3a)", "$(token_3b)"],
                    StringList2 = ["$(token_3c)", "$(token_3d)"],
                    StringList3 = ["$(token_3e)", "$(token_3f)"]
                },
                ClassWithStringCollections2 = new ClassWithStringCollections
                {
                    StringList1 = ["$(token_4a)", "$(token_4b)"],
                    StringList2 = ["$(token_4c)", "$(token_4d)"],
                    StringList3 = ["$(token_4e)", "$(token_4f)"]
                }
            };
            var stepTokenReplacer = new PipelineStepTokenReplacer();
            stepTokenReplacer.Replace(classInstance, tokenReplacer);

            Assert.AreEqual("value1a", classInstance.ClassWithStrings1.String1);
            Assert.AreEqual("$(token_1b)", classInstance.ClassWithStrings1.String2);
            Assert.AreEqual("value1c", classInstance.ClassWithStrings1.String3);

            Assert.AreEqual("$(token_2a)", classInstance.ClassWithStrings2.String1);
            Assert.AreEqual("$(token_2b)", classInstance.ClassWithStrings2.String2);
            Assert.AreEqual("$(token_2c)", classInstance.ClassWithStrings2.String3);

            CollectionAssert.AreEquivalent(new List<string> { "value3a", "value3b" }, classInstance.ClassWithStringCollections1.StringList1);
            CollectionAssert.AreEquivalent(new List<string> { "$(token_3c)", "$(token_3d)" }, classInstance.ClassWithStringCollections1.StringList2);
            CollectionAssert.AreEquivalent(new List<string> { "value3e", "value3f" }, classInstance.ClassWithStringCollections1.StringList3);

            CollectionAssert.AreEquivalent(new List<string> { "$(token_4a)", "$(token_4b)" }, classInstance.ClassWithStringCollections2.StringList1);
            CollectionAssert.AreEquivalent(new List<string> { "$(token_4c)", "$(token_4d)" }, classInstance.ClassWithStringCollections2.StringList2);
            CollectionAssert.AreEquivalent(new List<string> { "$(token_4e)", "$(token_4f)" }, classInstance.ClassWithStringCollections2.StringList3);
        }


        [TestMethod]
        public void Replace_WithNestedCompositeMembers()
        {
            var tokenReplacer = new TokenReplacer
            {
                Prefix = "$(",
                Suffix = ")",
                Variables = new ReadOnlyDictionary<string, string>(new Dictionary<string, string> {
                    { "token_1a", "value1a" },
                    { "token_1b", "value1b" },
                    { "token_1c", "value1c" },

                    { "token_2a", "value2a" },
                    { "token_2b", "value2b" },
                    { "token_2c", "value2c" },

                    { "token_3a", "value3a" },
                    { "token_3b", "value3b" },
                    { "token_3c", "value3c" },

                    { "token_4a", "value4a" },
                    { "token_4b", "value4b" },
                    { "token_4c", "value4c" },

                    { "token_5a", "value5a" },
                    { "token_5b", "value5b" },
                    { "token_5c", "value5c" },

                    { "token_6a", "value6a" },
                    { "token_6b", "value6b" },
                    { "token_6c", "value6c" },

                    { "token_7a", "value7a" },
                    { "token_7b", "value7b" },
                    { "token_7c", "value7c" },
                })
            };

            var classInstance = new ClassCompositeLeaf
            {
                String1 = "$(token_1a)",
                StringList1 = ["$(token_1b)", "$(token_1c)"],
                Children = [
                    new ClassCompositeLeaf
                    {
                        String1 = "$(token_2a)",
                        StringList1 = ["$(token_2b)", "$(token_2c)"],
                        Children = [
                            new ClassCompositeLeaf
                            {
                                String1 = "$(token_3a)",
                                StringList1 = ["$(token_3b)", "$(token_3c)"],
                                Children = [
                                    new ClassCompositeLeaf
                                    {
                                        String1 = "$(token_5a)",
                                        StringList1 = ["$(token_5b)", "$(token_5c)"],
                                    }
                                ]
                            },
                            new ClassCompositeLeaf
                            {
                                String1 = "$(token_4a)",
                                StringList1 = ["$(token_4b)", "$(token_4c)"],
                                Children = [
                                    new ClassCompositeLeaf
                                    {
                                        String1 = "$(token_6a)",
                                        StringList1 = ["$(token_6b)", "$(token_6c)"],
                                    },
                                    new ClassCompositeLeaf
                                    {
                                        String1 = "$(token_7a)",
                                        StringList1 = ["$(token_7b)", "$(token_7c)"],
                                    }
                                ]
                            }
                        ]
                    }
                ]
            };
            var stepTokenReplacer = new PipelineStepTokenReplacer();
            stepTokenReplacer.Replace(classInstance, tokenReplacer);

            Assert.AreEqual("value1a", classInstance.String1);
            Assert.AreEqual("value1b", classInstance.StringList1[0]);
            Assert.AreEqual("value1c", classInstance.StringList1[1]);

            var children = classInstance.Children;
            var child_1 = children[0];

            Assert.AreEqual("value2a", child_1.String1);
            Assert.AreEqual("value2b", child_1.StringList1[0]);
            Assert.AreEqual("value2c", child_1.StringList1[1]);

            children = child_1.Children;
            child_1 = children[0];
            var child_2 = children[1];

            Assert.AreEqual("value3a", child_1.String1);
            Assert.AreEqual("value3b", child_1.StringList1[0]);
            Assert.AreEqual("value3c", child_1.StringList1[1]);

            Assert.AreEqual("value4a", child_2.String1);
            Assert.AreEqual("value4b", child_2.StringList1[0]);
            Assert.AreEqual("value4c", child_2.StringList1[1]);

            children = child_1.Children;
            child_1 = children[0];
            Assert.AreEqual("value5a", child_1.String1);
            Assert.AreEqual("value5b", child_1.StringList1[0]);
            Assert.AreEqual("value5c", child_1.StringList1[1]);

            children = child_2.Children;
            child_1 = children[0];
            child_2 = children[1];

            Assert.AreEqual("value6a", child_1.String1);
            Assert.AreEqual("value6b", child_1.StringList1[0]);
            Assert.AreEqual("value6c", child_1.StringList1[1]);

            Assert.AreEqual("value7a", child_2.String1);
            Assert.AreEqual("value7b", child_2.StringList1[0]);
            Assert.AreEqual("value7c", child_2.StringList1[1]);
        }
    }
}