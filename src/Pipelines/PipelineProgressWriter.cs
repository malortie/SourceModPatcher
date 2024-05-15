namespace Pipelines
{
    public interface IPipelineProgressContext
    {
        int StepNumber { get; set; }
        int NumStepsTotal { get; set; }
    }

    public class PipelineProgressContext : IPipelineProgressContext
    {
        public int StepNumber { get; set; }
        public int NumStepsTotal { get; set; }
    }

    public interface IPipelineProgressContextFactory
    {
        IPipelineProgressContext CreateContext();
    }

    public class PipelineProgressContextFactory : IPipelineProgressContextFactory
    {
        public IPipelineProgressContext CreateContext()
        {
            return new PipelineProgressContext();
        }
    }

    public interface IPipelineStageProgressStepFormatter
    {
        string Format(IPipelineProgressContext pipelineContext, ReadOnlyPipelineStepData stepData);
    }

    public class DefaultPipelineStageProgressStepFormatter : IPipelineStageProgressStepFormatter
    {
        public virtual string Format(IPipelineProgressContext pipelineContext, ReadOnlyPipelineStepData stepData)
        {
            return $"({pipelineContext.StepNumber}/{pipelineContext.NumStepsTotal}) {stepData.Description}";
        }
    }

    public class StepDependenciesNotCompletedFormatter : IPipelineStageProgressStepFormatter
    {
        public virtual string Format(IPipelineProgressContext pipelineContext, ReadOnlyPipelineStepData stepData)
        {
            return $"Step {stepData.Name} will not be executed: The following dependencies were not completed: <{string.Join(',', stepData.DependsOn)}>";
        }
    }

    public class StepCompletedFormatter : DefaultPipelineStageProgressStepFormatter
    {
        public override string Format(IPipelineProgressContext pipelineContext, ReadOnlyPipelineStepData stepData)
        {
            return $"{base.Format(pipelineContext, stepData)} [COMPLETED]";
        }
    }

    public class StepPartiallyCompletedFormatter : DefaultPipelineStageProgressStepFormatter
    {
        public override string Format(IPipelineProgressContext pipelineContext, ReadOnlyPipelineStepData stepData)
        {
            return $"{base.Format(pipelineContext, stepData)} [PARTIALLY COMPLETED]";
        }
    }

    public class StepFailedFormatter : DefaultPipelineStageProgressStepFormatter
    {
        public override string Format(IPipelineProgressContext pipelineContext, ReadOnlyPipelineStepData stepData)
        {
            return $"{base.Format(pipelineContext, stepData)} [FAILED]";
        }
    }

    public class StepCancelledFormatter : DefaultPipelineStageProgressStepFormatter
    {
        public override string Format(IPipelineProgressContext pipelineContext, ReadOnlyPipelineStepData stepData)
        {
            return $"{base.Format(pipelineContext, stepData)} [CANCELLED]";
        }
    }


    public interface IPipelineStageProgressWriter
    {
        void WriteStepDependenciesNotCompleted(IPipelineProgressContext pipelineContext, ReadOnlyPipelineStepData stepData);
        void WriteStepExecute(IPipelineProgressContext pipelineContext, ReadOnlyPipelineStepData stepData);
        void WriteStepCompleted(IPipelineProgressContext pipelineContext, ReadOnlyPipelineStepData stepData);
        void WriteStepPartiallyCompleted(IPipelineProgressContext pipelineContext, ReadOnlyPipelineStepData stepData);
        void WriteStepFailed(IPipelineProgressContext pipelineContext, ReadOnlyPipelineStepData stepData);
        void WriteStepCancelled(IPipelineProgressContext pipelineContext, ReadOnlyPipelineStepData stepData);
    }

    public class PipelineStageProgressWriter(IWriter writer) : IPipelineStageProgressWriter
    {
        readonly IWriter _writer = writer;
        public IPipelineStageProgressStepFormatter StepExecuteFormatter { get; set; } = new DefaultPipelineStageProgressStepFormatter();
        public IPipelineStageProgressStepFormatter StepDependenciesNotCompletedFormatter { get; set; } = new StepDependenciesNotCompletedFormatter();
        public IPipelineStageProgressStepFormatter StepCompletedFormatter { get; set; } = new StepCompletedFormatter();
        public IPipelineStageProgressStepFormatter StepPartiallyCompletedFormatter { get; set; } = new StepPartiallyCompletedFormatter();
        public IPipelineStageProgressStepFormatter StepFailedFormatter { get; set; } = new StepFailedFormatter();
        public IPipelineStageProgressStepFormatter StepCancelledFormatter { get; set; } = new StepCancelledFormatter();

        public void WriteStepDependenciesNotCompleted(IPipelineProgressContext pipelineContext, ReadOnlyPipelineStepData stepData)
        {
            _writer.Warning(StepDependenciesNotCompletedFormatter.Format(pipelineContext, stepData));
        }

        public void WriteStepExecute(IPipelineProgressContext pipelineContext, ReadOnlyPipelineStepData stepData)
        {
            _writer.Info(StepExecuteFormatter.Format(pipelineContext, stepData));
        }

        public void WriteStepCompleted(IPipelineProgressContext pipelineContext, ReadOnlyPipelineStepData stepData)
        {
            _writer.Info(StepCompletedFormatter.Format(pipelineContext, stepData));
        }

        public void WriteStepPartiallyCompleted(IPipelineProgressContext pipelineContext, ReadOnlyPipelineStepData stepData)
        {
            _writer.Warning(StepPartiallyCompletedFormatter.Format(pipelineContext, stepData));
        }

        public void WriteStepFailed(IPipelineProgressContext pipelineContext, ReadOnlyPipelineStepData stepData)
        {
            _writer.Failure(StepFailedFormatter.Format(pipelineContext, stepData));
        }

        public void WriteStepCancelled(IPipelineProgressContext pipelineContext, ReadOnlyPipelineStepData stepData)
        {
            _writer.Cancellation(StepCancelledFormatter.Format(pipelineContext, stepData));
        }
    }

    public interface IPipelineProgressStageFormatter
    {
        string Format(IPipelineProgressContext pipelineContext, ReadOnlyPipelineStageData stageData);
    }

    public class DefaultPipelineProgressStageFormatter : IPipelineProgressStageFormatter
    {
        public virtual string Format(IPipelineProgressContext pipelineContext, ReadOnlyPipelineStageData stageData)
        {
            return $"({pipelineContext.StepNumber}/{pipelineContext.NumStepsTotal}) {stageData.Description}";
        }
    }

    public class StageCompletedFormatter : DefaultPipelineProgressStageFormatter
    {
        public override string Format(IPipelineProgressContext pipelineContext, ReadOnlyPipelineStageData stageData)
        {
            return $"{base.Format(pipelineContext, stageData)} [COMPLETED]";
        }
    }

    public class StagePartiallyCompletedFormatter : DefaultPipelineProgressStageFormatter
    {
        public override string Format(IPipelineProgressContext pipelineContext, ReadOnlyPipelineStageData stageData)
        {
            return $"{base.Format(pipelineContext, stageData)} [PARTIALLY COMPLETED]";
        }
    }

    public class StageFailedFormatter : DefaultPipelineProgressStageFormatter
    {
        public override string Format(IPipelineProgressContext pipelineContext, ReadOnlyPipelineStageData stageData)
        {
            return $"{base.Format(pipelineContext, stageData)} [FAILED]";
        }
    }

    public class StageCancelledFormatter : DefaultPipelineProgressStageFormatter
    {
        public override string Format(IPipelineProgressContext pipelineContext, ReadOnlyPipelineStageData stageData)
        {
            return $"{base.Format(pipelineContext, stageData)} [CANCELLED]";
        }
    }

    public interface IPipelineProgressWriter
    {
        void WriteStageExecute(IPipelineProgressContext pipelineContext, ReadOnlyPipelineStageData stageData);
        void WriteStageCompleted(IPipelineProgressContext pipelineContext, ReadOnlyPipelineStageData stageData);
        void WriteStagePartiallyCompleted(IPipelineProgressContext pipelineContext, ReadOnlyPipelineStageData stageData);
        void WriteStageFailed(IPipelineProgressContext pipelineContext, ReadOnlyPipelineStageData stageData);
        void WriteStageCancelled(IPipelineProgressContext pipelineContext, ReadOnlyPipelineStageData stageData);
    }

    public class PipelineProgressWriter(IWriter writer) : IPipelineProgressWriter
    {
        readonly IWriter _writer = writer;
        public IPipelineProgressStageFormatter StageExecuteFormatter { get; set; } = new DefaultPipelineProgressStageFormatter();
        public IPipelineProgressStageFormatter StageCompletedFormatter { get; set; } = new StageCompletedFormatter();
        public IPipelineProgressStageFormatter StagePartiallyCompletedFormatter { get; set; } = new StagePartiallyCompletedFormatter();
        public IPipelineProgressStageFormatter StageFailedFormatter { get; set; } = new StageFailedFormatter();
        public IPipelineProgressStageFormatter StageCancelledFormatter { get; set; } = new StageCancelledFormatter();

        public void WriteStageExecute(IPipelineProgressContext pipelineContext, ReadOnlyPipelineStageData stageData)
        {
            _writer.Info(StageExecuteFormatter.Format(pipelineContext, stageData));
        }

        public void WriteStageCompleted(IPipelineProgressContext pipelineContext, ReadOnlyPipelineStageData stageData)
        {
            _writer.Info(StageCompletedFormatter.Format(pipelineContext, stageData));
        }

        public void WriteStagePartiallyCompleted(IPipelineProgressContext pipelineContext, ReadOnlyPipelineStageData stageData)
        {
            _writer.Warning(StagePartiallyCompletedFormatter.Format(pipelineContext, stageData));
        }

        public void WriteStageFailed(IPipelineProgressContext pipelineContext, ReadOnlyPipelineStageData stageData)
        {
            _writer.Failure(StageFailedFormatter.Format(pipelineContext, stageData));
        }

        public void WriteStageCancelled(IPipelineProgressContext pipelineContext, ReadOnlyPipelineStageData stageData)
        {
            _writer.Cancellation(StageCancelledFormatter.Format(pipelineContext, stageData));
        }
    }
}
