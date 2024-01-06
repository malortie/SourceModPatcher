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

    public class PauseHandlerMock : IPauseHandler
    {
        public int PauseTotal { get; private set; } = 0;

        public void Pause()
        {
            ++PauseTotal;
        }
    }

    [TestClass]
    public class TestPipelineStage
    {
        [TestMethod]
        public void PauseHandler_CalledAfterEachStep_WhenPauseAfterEachStepIsTrue()
        {
            var pauseHandler = new PauseHandlerMock();
            var stage = new NullStage
            {
                PauseAfterEachStep = true,
                PauseHandler = pauseHandler,
                StepsDatas = [
                    new NullStepDataComplete(),
                    new NullStepDataComplete(),
                    new NullStepDataComplete()
                ]
            };

            stage.DoStage(new NullContext());
            Assert.AreEqual(3, pauseHandler.PauseTotal);
        }

        [TestMethod]
        public void PauseHandler_NotCalledAfterEachStep_WhenPauseAfterEachStepIsFalse()
        {
            var pauseHandler = new PauseHandlerMock();
            var stage = new NullStage
            {
                PauseAfterEachStep = false,
                PauseHandler = pauseHandler,
                StepsDatas = [
                    new NullStepDataComplete(),
                    new NullStepDataComplete(),
                    new NullStepDataComplete()
                ]
            };

            stage.DoStage(new NullContext());
            Assert.AreEqual(0, pauseHandler.PauseTotal);
        }
    }
}