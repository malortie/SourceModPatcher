using System.Collections.ObjectModel;
using System.Reflection;

namespace Pipelines.Tests
{
    public class NullPropertiesGetter : IPropertiesGetter
    {
        public IEnumerable<PropertyInfo> GetProperties(object obj)
        {
            return [];
        }
    }

    public class SingleStringPropertyGetterMock : IPropertiesGetter
    {
        public int GetPropertiesTotal { get; private set; } = 0;

        public IEnumerable<PropertyInfo> GetProperties(object obj)
        {
            ++GetPropertiesTotal;
            // Return the first string property.
            return new List<PropertyInfo>() { obj.GetType().GetProperties().First(p => p.PropertyType == typeof(string)) };
        }
    }

    public class PipelineStepTokenReplacerMock : IPipelineStepTokenReplacer
    {
        public IPropertiesGetter PropertiesGetter { get; set; } = new NullPropertiesGetter();

        public int ReplaceTotal { get; private set; } = 0;

        public void Replace(object obj, ITokenReplacer tokenReplacer)
        {
            ++ReplaceTotal;
        }
    }

    public class TokenReplacerMock : ITokenReplacer
    {
        public string Prefix { get; set; } = string.Empty;
        public string Suffix { get; set; } = string.Empty;
        public ReadOnlyDictionary<string, string> Variables { get; set; } = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>());

        public int ReplaceTotal { get; private set; } = 0;

        public string Replace(string str)
        {
            ++ReplaceTotal;
            return string.Empty;
        }
    }

    public class TokenReplacerVariablesProviderMock : ITokenReplacerVariablesProvider<NullContext>
    {
        public int GetVariablesTotal { get; private set; } = 0;

        public Dictionary<string, string> GetVariables(NullContext context)
        {
            ++GetVariablesTotal;
            return new Dictionary<string, string>();
        }
    }

    [TestClass]
    public class TestPipelineStage_PipelineStepTokenReplacer
    {
        static NullContext NullContext = new NullContext();

        [TestMethod]
        public void PipelineStepTokenReplacer_NotCalled_WhenStageHasNoSteps()
        {
            var stepTokenReplacer = new PipelineStepTokenReplacerMock();
            var stage = new NullStage
            {
                PipelineStepTokenReplacer = stepTokenReplacer,
                TokenReplacer = new TokenReplacerMock(),
                TokenReplacerVariablesProvider = new TokenReplacerVariablesProviderMock()
            };

            stage.DoStage(NullContext);
            Assert.AreEqual(0, stepTokenReplacer.ReplaceTotal);
        }

        [TestMethod]
        public void PipelineStepTokenReplacer_NotCalled_When_TokenReplacer_Null_And_TokenReplacerVariableProvider_Null()
        {
            var stepTokenReplacer = new PipelineStepTokenReplacerMock();
            var stage = new NullStage
            {
                PipelineStepTokenReplacer = stepTokenReplacer,
                TokenReplacer = null,
                TokenReplacerVariablesProvider = null,
                StepsDatas = [
                    new NullStepDataComplete(),
                ]
            };

            stage.DoStage(NullContext);
            Assert.AreEqual(0, stepTokenReplacer.ReplaceTotal);
        }

        [TestMethod]
        public void PipelineStepTokenReplacer_NotCalled_When_TokenReplacer_Null_And_TokenReplacerVariableProvider_NotNull()
        {
            var stepTokenReplacer = new PipelineStepTokenReplacerMock();
            var stage = new NullStage
            {
                PipelineStepTokenReplacer = stepTokenReplacer,
                TokenReplacer = null,
                TokenReplacerVariablesProvider = new TokenReplacerVariablesProviderMock(),
                StepsDatas = [
                    new NullStepDataComplete(),
                ]
            };

            stage.DoStage(NullContext);
            Assert.AreEqual(0, stepTokenReplacer.ReplaceTotal);
        }

        [TestMethod]
        public void PipelineStepTokenReplacer_NotCalled_When_TokenReplacer_NotNull_And_TokenReplacerVariableProvider_Null()
        {
            var stepTokenReplacer = new PipelineStepTokenReplacerMock();
            var stage = new NullStage
            {
                PipelineStepTokenReplacer = stepTokenReplacer,
                TokenReplacer = new TokenReplacerMock(),
                TokenReplacerVariablesProvider = null,
                StepsDatas = [
                    new NullStepDataComplete(),
                ]
            };

            stage.DoStage(NullContext);
            Assert.AreEqual(0, stepTokenReplacer.ReplaceTotal);
        }

        [TestMethod]
        public void PipelineStepTokenReplacer_Called_When_TokenReplacer_NotNull_And_TokenReplacerVariableProvider_NotNull()
        {
            var stepTokenReplacer = new PipelineStepTokenReplacerMock();
            var stage = new NullStage
            {
                PipelineStepTokenReplacer = stepTokenReplacer,
                TokenReplacer = new TokenReplacerMock(),
                TokenReplacerVariablesProvider = new TokenReplacerVariablesProviderMock(),
                StepsDatas = [
                    new NullStepDataComplete(),
                ]
            };

            stage.DoStage(NullContext);
            Assert.AreEqual(1, stepTokenReplacer.ReplaceTotal);
        }

        [TestMethod]
        public void PipelineStepTokenReplacer_Called_3_Times()
        {
            var stepTokenReplacer = new PipelineStepTokenReplacerMock();
            var stage = new NullStage
            {
                PipelineStepTokenReplacer = stepTokenReplacer,
                TokenReplacer = new TokenReplacerMock(),
                TokenReplacerVariablesProvider = new TokenReplacerVariablesProviderMock(),
                StepsDatas = [
                    new NullStepDataComplete(),
                    new NullStepDataComplete(),
                    new NullStepDataComplete(),
                ]
            };

            stage.DoStage(NullContext);
            Assert.AreEqual(3, stepTokenReplacer.ReplaceTotal);
        }

        [TestMethod]
        public void PipelineStepTokenReplacer_TokenReplacer_Called_3_Times()
        {
            var tokenReplacer = new TokenReplacerMock();
            var stage = new NullStage
            {
                PipelineStepTokenReplacer = new PipelineStepTokenReplacer
                {
                    PropertiesGetter = new SingleStringPropertyGetterMock()
                },
                TokenReplacer = tokenReplacer,
                TokenReplacerVariablesProvider = new TokenReplacerVariablesProviderMock(),
                StepsDatas = [
                    new NullStepDataComplete(),
                    new NullStepDataComplete(),
                    new NullStepDataComplete(),
                ]
            };

            stage.DoStage(NullContext);
            Assert.AreEqual(3, tokenReplacer.ReplaceTotal);
        }

        [TestMethod]
        public void PipelineStepTokenReplacer_TokenReplacerVariablesProvider_Called_3_Times()
        {
            var tokenReplacerVariablesProvider = new TokenReplacerVariablesProviderMock();
            var stage = new NullStage
            {
                PipelineStepTokenReplacer = new PipelineStepTokenReplacer
                {
                    PropertiesGetter = new SingleStringPropertyGetterMock()
                },
                TokenReplacer = new TokenReplacerMock(),
                TokenReplacerVariablesProvider = tokenReplacerVariablesProvider,
                StepsDatas = [
                    new NullStepDataComplete(),
                    new NullStepDataComplete(),
                    new NullStepDataComplete(),
                ]
            };

            stage.DoStage(NullContext);
            Assert.AreEqual(3, tokenReplacerVariablesProvider.GetVariablesTotal);
        }

        [TestMethod]
        public void PipelineStepTokenReplacer_PropertiesGetter_Called_3_Times()
        {
            var propertiesGetter = new SingleStringPropertyGetterMock();
            var stage = new NullStage
            {
                PipelineStepTokenReplacer = new PipelineStepTokenReplacer
                {
                    PropertiesGetter = propertiesGetter
                },
                TokenReplacer = new TokenReplacerMock(),
                TokenReplacerVariablesProvider = new TokenReplacerVariablesProviderMock(),
                StepsDatas = [
                    new NullStepDataComplete(),
                    new NullStepDataComplete(),
                    new NullStepDataComplete(),
                ]
            };

            stage.DoStage(NullContext);
            Assert.AreEqual(3, propertiesGetter.GetPropertiesTotal);
        }
    }
}