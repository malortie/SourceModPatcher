namespace Pipelines.Tests
{
    public abstract class AbstractNullStepData : IPipelineStepData
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set;} = string.Empty;
        public List<string> DependsOn { get; set; } = [];
    }

    public class NullStepDataComplete : AbstractNullStepData {}
    public class NullStepDataPartiallyComplete : AbstractNullStepData {}
    public class NullStepDataFailed : AbstractNullStepData {}
    public class NullStepDataCancelled : AbstractNullStepData {}

    public class NullContext
    {
    }

    public class NullStepAlwaysComplete : IPipelineStep<NullContext>
    {
        public PipelineStepStatus DoStep(NullContext context, IPipelineStepData stepData, IWriter writer)
        {
            return PipelineStepStatus.Complete;
        }
    }

    public class NullStepAlwaysPartiallyComplete : IPipelineStep<NullContext>
    {
        public PipelineStepStatus DoStep(NullContext context, IPipelineStepData stepData, IWriter writer)
        {
            return PipelineStepStatus.PartiallyComplete;
        }
    }

    public class NullStepAlwaysFailed : IPipelineStep<NullContext>
    {
        public PipelineStepStatus DoStep(NullContext context, IPipelineStepData stepData, IWriter writer)
        {
            return PipelineStepStatus.Failed;
        }
    }

    public class NullStepAlwaysCancelled : IPipelineStep<NullContext>
    {
        public PipelineStepStatus DoStep(NullContext context, IPipelineStepData stepData, IWriter writer)
        {
            return PipelineStepStatus.Cancelled;
        }
    }

    public class NullStage : PipelineStage<NullContext>
    {
        static Dictionary<Type, IPipelineStep<NullContext>> _stepsDataToStepMap = new()
        {
            { typeof(NullStepDataComplete), new NullStepAlwaysComplete() },
            { typeof(NullStepDataPartiallyComplete), new NullStepAlwaysPartiallyComplete() },
            { typeof(NullStepDataFailed), new NullStepAlwaysFailed() },
            { typeof(NullStepDataCancelled), new NullStepAlwaysCancelled() },
        };

        public override IPipelineStep<NullContext> GetStepForStepData(IPipelineStepData stepData)
        {
            return _stepsDataToStepMap[stepData.GetType()];
        }
    }

    public class SetupContextStage : NullStage
    {
        public int SetupContextTotal { get; private set; } = 0;

        public override void SetupContext(NullContext context)
        {
            ++SetupContextTotal;
        }
    }

    [TestClass]
    public class TestPipelineStage
    {
        static NullContext NullContext = new NullContext();

        [TestMethod]
        public void SetupContext_CalledOncePerStageRun()
        {
            var stage = new SetupContextStage
            {
                StepsDatas = [
                    new NullStepDataComplete(),
                    new NullStepDataPartiallyComplete(),
                    new NullStepDataFailed(),
                    new NullStepDataCancelled(),
                ]
            };

            stage.DoStage(NullContext);
            Assert.AreEqual(1, stage.SetupContextTotal);
            stage.DoStage(NullContext);
            Assert.AreEqual(2, stage.SetupContextTotal);
            stage.DoStage(NullContext);
            Assert.AreEqual(3, stage.SetupContextTotal);
        }
    }
}