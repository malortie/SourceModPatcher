namespace Pipelines.Tests
{
    public class ClassWithReplaceTokenAttributeOnAllProperties
    {
        [PipelineStepReplaceToken]
        public char CharProperty { get; set; }
        [PipelineStepReplaceToken]
        public int IntProperty { get; set; }
        [PipelineStepReplaceToken]
        public float FloatProperty { get; set; }
        [PipelineStepReplaceToken]
        public string StringProperty { get; set; } = string.Empty;
        [PipelineStepReplaceToken]
        public object? ObjectProperty { get; set; }
        [PipelineStepReplaceToken]
        public List<object> ObjectListProperty { get; set; } = [];
    }

    public class ClassWithReplaceTokenAttributeOnSomeProperties
    {
        [PipelineStepReplaceToken]
        public char CharProperty { get; set; }
        public int IntProperty { get; set; }
        [PipelineStepReplaceToken]
        public float FloatProperty { get; set; }
        public string StringProperty { get; set; } = string.Empty;
        public object? ObjectProperty { get; set; }
        [PipelineStepReplaceToken]
        public List<object> ObjectListProperty { get; set; } = [];
    }

    public class ClassWithNoReplaceTokenAttributes
    {
        public char CharProperty { get; set; }
        public int IntProperty { get; set; }
        public float FloatProperty { get; set; }
        public string StringProperty { get; set; } = string.Empty;
        public object? ObjectProperty { get; set; }
        public List<object> ObjectListProperty { get; set; } = [];
    }

    [TestClass]
    public class TestPipelineStepReplaceTokenPropertiesGetter
    {
        [TestMethod]
        public void GetProperties_WithReplaceTokensAttributeOnAllProperties()
        {
            var expected = typeof(ClassWithReplaceTokenAttributeOnAllProperties).GetProperties().Where(p => Attribute.IsDefined(p, typeof(PipelineStepReplaceTokenAttribute))).ToList();

            var propertiesGetter = new PipelineStepReplaceTokenPropertiesGetter();
            var propsWithReplaceTokenAttributes = propertiesGetter.GetProperties(new ClassWithReplaceTokenAttributeOnAllProperties()).ToList();

            CollectionAssert.AreEquivalent(expected, propsWithReplaceTokenAttributes);
        }

        [TestMethod]
        public void GetProperties_WithReplaceTokensAttributeOnSomeProperties()
        {
            var expected = typeof(ClassWithReplaceTokenAttributeOnSomeProperties).GetProperties().Where(p => Attribute.IsDefined(p, typeof(PipelineStepReplaceTokenAttribute))).ToList();

            var propertiesGetter = new PipelineStepReplaceTokenPropertiesGetter();
            var propsWithReplaceTokenAttributes = propertiesGetter.GetProperties(new ClassWithReplaceTokenAttributeOnSomeProperties()).ToList();

            CollectionAssert.AreEquivalent(expected, propsWithReplaceTokenAttributes);
        }


        [TestMethod]
        public void GetProperties_WithNoReplaceTokensAttributeOnProperties()
        {
            var expected = typeof(ClassWithNoReplaceTokenAttributes).GetProperties().Where(p => Attribute.IsDefined(p, typeof(PipelineStepReplaceTokenAttribute))).ToList();

            var propertiesGetter = new PipelineStepReplaceTokenPropertiesGetter();
            var propsWithReplaceTokenAttributes = propertiesGetter.GetProperties(new ClassWithNoReplaceTokenAttributes()).ToList();

            CollectionAssert.AreEquivalent(expected, propsWithReplaceTokenAttributes);
        }
    }
}