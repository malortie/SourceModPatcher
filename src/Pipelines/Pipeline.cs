using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace Pipelines
{
    public enum PipelineStepStatus
    {
        Complete = 0,
        PartiallyComplete = 1,
        Failed = 2,
        Cancelled = 3,
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class PipelineStepReplaceTokenAttribute : Attribute
    {
        public PipelineStepReplaceTokenAttribute()
        {
        }
    }

    public interface IPipelineStepData
    {
        string Name { get; set; }
        string Description { get; set; }
        List<string> DependsOn { get; set; }
    }

    public interface IPipelineStep<ContextT>
    {
        PipelineStepStatus DoStep(ContextT context, IPipelineStepData stepData, IWriter writer);
    }

    public class ReadOnlyPipelineStepData
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public IImmutableList<string> DependsOn { get; set; } = [];
    }

    public class ReadOnlyPipelineStageData
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ReadOnlyPipelineStepData[] StepsDatas { get; set; } = [];
        public IPipelineStepStatsResults StatsResults { get; set; } = new PipelineStepStatsResults();
    }

    public interface IPipelineStage<ContextT>
    {
        string Name { get; set; }

        string Description { get; set; }

        IPipelineStepData[] StepsDatas { get; set; }

        IPipelineStepStatsResults StatsResults { get; set; }

        void SetupContext(ContextT context);
        IPipelineStep<ContextT> GetStepForStepData(IPipelineStepData stepData);
        ReadOnlyPipelineStageData GetReadOnlyStageData();
        ReadOnlyPipelineStepData GetReadOnlyStepData(IPipelineStepData stepData);

        PipelineStepStatus[] DoStage(ContextT context);
    }

    public interface IReadOnlyPipelineDataFactory
    {
        ReadOnlyPipelineStepData CreateStepData(IPipelineStepData stepData);
        ReadOnlyPipelineStageData CreateStageData<ContextT>(IPipelineStage<ContextT> stage);
    }

    public class ReadOnlyPipelineDataFactory : IReadOnlyPipelineDataFactory
    {
        public ReadOnlyPipelineStepData CreateStepData(IPipelineStepData stepData)
        {
            return new ReadOnlyPipelineStepData
            {
                Name = stepData.Name,
                Description = stepData.Description,
                DependsOn = stepData.DependsOn.ToImmutableList()
            };
        }

        public ReadOnlyPipelineStageData CreateStageData<ContextT>(IPipelineStage<ContextT> stage)
        {
            return new ReadOnlyPipelineStageData
            {
                Name = stage.Name,
                Description = stage.Description,
                StepsDatas = stage.StepsDatas.Select(CreateStepData).ToArray(),
                StatsResults = (IPipelineStepStatsResults)stage.StatsResults.Clone()
            };
        }
    }

    public interface IStageStepWriterFormatter
    {
        string Format(string message, ReadOnlyPipelineStageData stageData, ReadOnlyPipelineStepData stepData);
    }

    public class DefaultStageStepWriterFormatter : IStageStepWriterFormatter
    {
        public string Format(string message, ReadOnlyPipelineStageData stageData, ReadOnlyPipelineStepData stepData)
        {
            return $"{message} [{stageData.Name}][{stepData.Name}]";
        }
    }

    public class IdempotentStageStepWriterFormatter : IStageStepWriterFormatter
    {
        public string Format(string message, ReadOnlyPipelineStageData stageData, ReadOnlyPipelineStepData stepData)
        {
            return message;
        }
    }

    public class StageStepWriterDecorator(IWriter writer) : IWriter
    {
        readonly IWriter _writer = writer;

        public ReadOnlyPipelineStageData StageData { get; set; } = new();
        public ReadOnlyPipelineStepData StepData { get; set; } = new();

        public IStageStepWriterFormatter SuccessFormatter { get; set; } = new IdempotentStageStepWriterFormatter();
        public IStageStepWriterFormatter InfoFormatter { get; set; } = new IdempotentStageStepWriterFormatter();
        public IStageStepWriterFormatter WarningFormatter { get; set; } = new DefaultStageStepWriterFormatter();
        public IStageStepWriterFormatter ErrorFormatter { get; set; } = new DefaultStageStepWriterFormatter();
        public IStageStepWriterFormatter FailureFormatter { get; set; } = new DefaultStageStepWriterFormatter();
        public IStageStepWriterFormatter CancellationFormatter { get; set; } = new DefaultStageStepWriterFormatter();

        public void Success(string message)
        {
            _writer.Success(SuccessFormatter.Format(message, StageData, StepData));
        }

        public void Info(string message)
        {
            _writer.Info(InfoFormatter.Format(message, StageData, StepData));
        }

        public void Warning(string message)
        {
            _writer.Warning(WarningFormatter.Format(message, StageData, StepData));
        }

        public void Error(string message)
        {
            _writer.Error(ErrorFormatter.Format(message, StageData, StepData));
        }

        public void Failure(string message)
        {
            _writer.Failure(FailureFormatter.Format(message, StageData, StepData));
        }

        public void Cancellation(string message)
        {
            _writer.Cancellation(CancellationFormatter.Format(message, StageData, StepData));
        }
    }

    public abstract class PipelineStage<ContextT> : IPipelineStage<ContextT>
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public bool PauseAfterEachStep { get; set; } = false;
        public IPipelineStepData[] StepsDatas { get; set; } = [];
        public IPipelineStageProgressWriter? ProgressWriter { get; set; }
        public IWriter Writer { get; set; } = new NullWriter();
        public ITokenReplacer? TokenReplacer { get; set; }
        public ITokenReplacerVariablesProvider<ContextT>? TokenReplacerVariablesProvider { get; set; }
        public IPipelineStepTokenReplacer? PipelineStepTokenReplacer { get; set; } = new PipelineStepTokenReplacer();
        public IPauseHandler? PauseHandler { get; set; }
        public IPipelineStepStatsResults StatsResults { get; set; } = new PipelineStepStatsResults();
        public IPipelineProgressContextFactory ProgressContextFactory { get; set; } = new PipelineProgressContextFactory();
        public IReadOnlyPipelineDataFactory ReadOnlyPipelineStageDataFactory = new ReadOnlyPipelineDataFactory();

        public virtual void SetupContext(ContextT context)
        {
        }

        public abstract IPipelineStep<ContextT> GetStepForStepData(IPipelineStepData stepData);

        public ReadOnlyPipelineStageData GetReadOnlyStageData()
        {
            return ReadOnlyPipelineStageDataFactory.CreateStageData(this);
        }
        public ReadOnlyPipelineStepData GetReadOnlyStepData(IPipelineStepData stepData)
        {
            return ReadOnlyPipelineStageDataFactory.CreateStepData(stepData);
        }

        public PipelineStepStatus[] DoStage(ContextT context)
        {
            SetupContext(context);

            var progressContext = ProgressContextFactory.CreateContext();
            var stageStepWriterDecorator = new StageStepWriterDecorator(Writer);

            HashSet<string> stepsComplete = [];
            var stepStatuses = new List<PipelineStepStatus>();

            StatsResults.NumStepsTotal = StepsDatas.Length;
            StatsResults.NumStepsCompleted = 0;
            StatsResults.NumStepsPartiallyCompleted = 0;
            StatsResults.NumStepsFailed = 0;
            StatsResults.NumStepsCancelled = 0;

            for (int stepIndex = 0; stepIndex < StepsDatas.Length; ++stepIndex)
            {
                var stepData = StepsDatas[stepIndex];

                if (PipelineStepTokenReplacer != null && TokenReplacer != null && TokenReplacerVariablesProvider != null)
                {
                    // Perform token replacement in step string properties.
                    TokenReplacer.Variables = new ReadOnlyDictionary<string, string>(TokenReplacerVariablesProvider.GetVariables(context));
                    PipelineStepTokenReplacer?.Replace(stepData, TokenReplacer);
                }

                progressContext.StepNumber = stepIndex + 1;
                progressContext.NumStepsTotal = StepsDatas.Length;

                PipelineStepStatus stepStatus;

                var uncompletedDependencies = stepData.DependsOn.Except(stepsComplete).ToArray();
                if (uncompletedDependencies.Length > 0)
                {
                    ProgressWriter?.WriteStepDependenciesNotCompleted(progressContext, GetReadOnlyStepData(stepData));
                    stepStatus = PipelineStepStatus.Cancelled;
                }
                else
                {
                    ProgressWriter?.WriteStepExecute(progressContext, GetReadOnlyStepData(stepData));

                    // Setup writer decorator.
                    stageStepWriterDecorator.StageData = GetReadOnlyStageData();
                    stageStepWriterDecorator.StepData = GetReadOnlyStepData(stepData);

                    // Execute step.
                    stepStatus = GetStepForStepData(stepData).DoStep(context, stepData, stageStepWriterDecorator);
                }

                stepStatuses.Add(stepStatus);

                switch (stepStatus)
                {
                    case PipelineStepStatus.Complete:
                        ProgressWriter?.WriteStepCompleted(progressContext, GetReadOnlyStepData(stepData));
                        ++StatsResults.NumStepsCompleted;
                        stepsComplete.Add(stepData.Name);
                        break;
                    case PipelineStepStatus.PartiallyComplete:
                        ProgressWriter?.WriteStepPartiallyCompleted(progressContext, GetReadOnlyStepData(stepData));
                        ++StatsResults.NumStepsPartiallyCompleted;
                        break;
                    case PipelineStepStatus.Failed:
                        ProgressWriter?.WriteStepFailed(progressContext, GetReadOnlyStepData(stepData));
                        ++StatsResults.NumStepsFailed;
                        break;
                    case PipelineStepStatus.Cancelled:
                        ProgressWriter?.WriteStepCancelled(progressContext, GetReadOnlyStepData(stepData));
                        ++StatsResults.NumStepsCancelled;
                        break;
                }

                if (PauseAfterEachStep)
                    PauseHandler?.Pause();
            }

            return [.. stepStatuses];
        }
    }

    public interface IPipeline<ContextT>
    {
        void Execute(ContextT context);
    }

    public class Pipeline<ContextT>(IPipelineStage<ContextT>[] stages, IPipelineProgressWriter? progressWriter = null) : IPipeline<ContextT>
    {
        public IPipelineStage<ContextT>[] Stages { get; set; } = stages;

        public IPipelineProgressWriter? ProgressWriter { get; set; } = progressWriter;

        public IPipelineStatsResults StatsResults { get; set; } = new PipelineStatsResults();
        public IPipelineProgressContextFactory ProgressContextFactory { get; set; } = new PipelineProgressContextFactory();

        public void Execute(ContextT context)
        {
            StatsResults.NumStagesTotal = Stages.Length;
            StatsResults.NumStagesCompleted = 0;
            StatsResults.NumStagesPartiallyCompleted = 0;
            StatsResults.NumStagesFailed = 0;
            StatsResults.NumStagesCancelled = 0;

            StatsResults.NumStepsTotal = Stages.Sum(a => a.StepsDatas.Length);
            StatsResults.NumStepsCompleted = 0;
            StatsResults.NumStepsPartiallyCompleted = 0;
            StatsResults.NumStepsFailed = 0;
            StatsResults.NumStepsCancelled = 0;

            var progressContext = ProgressContextFactory.CreateContext();
            progressContext.NumStepsTotal = Stages.Length;

            for (int stageIndex = 0; stageIndex < Stages.Length; ++stageIndex)
            {
                var stage = Stages[stageIndex];
                progressContext.StepNumber = stageIndex + 1;

                // Execute stage.
                ProgressWriter?.WriteStageExecute(progressContext, stage.GetReadOnlyStageData());
                PipelineStepStatus[] stepsStatuses = stage.DoStage(context);

                int numStepsCompleted = stepsStatuses.Count(s => s == PipelineStepStatus.Complete);
                int numStepsPartiallyCompleted = stepsStatuses.Count(s => s == PipelineStepStatus.PartiallyComplete);
                int numStepsFailed = stepsStatuses.Count(s => s == PipelineStepStatus.Failed);
                int numStepsCancelled = stepsStatuses.Count(s => s == PipelineStepStatus.Cancelled);

                StatsResults.NumStepsCompleted += numStepsCompleted;
                StatsResults.NumStepsPartiallyCompleted += numStepsPartiallyCompleted;
                StatsResults.NumStepsFailed += numStepsFailed;
                StatsResults.NumStepsCancelled += numStepsCancelled;

                PipelineStepStatus status = PipelineStepStatus.Complete;

                if (numStepsCompleted < stage.StepsDatas.Length)
                {
                    // Here, at least one step was either partially completed, failed or cancelled.

                    if (numStepsPartiallyCompleted > 0)
                    {
                        // At least one step was not fully completed. Mark as partially complete.
                        status = PipelineStepStatus.PartiallyComplete;
                    }
                    else
                    {
                        // Here, no partially completed steps. Only either failed or cancelled steps.

                        if (numStepsFailed > 0)
                        {
                            // At least one step failed to complete. Mark as failed.
                            status = PipelineStepStatus.Failed;
                        }
                        else
                        {
                            // All steps were cancelled. Mark as cancelled.
                            status = PipelineStepStatus.Cancelled;
                        }
                    }
                }

                switch (status)
                {
                    case PipelineStepStatus.Complete:
                        ProgressWriter?.WriteStageCompleted(progressContext, stage.GetReadOnlyStageData());
                        ++StatsResults.NumStagesCompleted;
                        break;
                    case PipelineStepStatus.PartiallyComplete:
                        ProgressWriter?.WriteStagePartiallyCompleted(progressContext, stage.GetReadOnlyStageData());
                        ++StatsResults.NumStagesPartiallyCompleted;
                        break;
                    case PipelineStepStatus.Failed:
                        ProgressWriter?.WriteStageFailed(progressContext, stage.GetReadOnlyStageData());
                        ++StatsResults.NumStagesFailed;
                        break;
                    case PipelineStepStatus.Cancelled:
                        ProgressWriter?.WriteStageCancelled(progressContext, stage.GetReadOnlyStageData());
                        ++StatsResults.NumStagesCancelled;
                        break;
                }
            }
        }
    }
}
