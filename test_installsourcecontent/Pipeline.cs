using System.Reflection;
using System.Collections.ObjectModel;
using System.Collections;

namespace test_installsourcecontent
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
        public string Name { get; set; }
        public string Description { get; set; }
        public ReadOnlyCollection<string> DependsOn { get; set; }
    }

    public class ReadOnlyPipelineStageData
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ReadOnlyPipelineStepData[] StepsDatas { get; set; }
        public IPipelineStepStatsResults StatsResults { get; set; }
    }

    public interface IPipelineStage<ContextT>
    {
        string Name { get; set; }

        string Description { get; set; }

        IPipelineStepData[] StepsDatas { get; set; }

        IPipelineStepStatsResults StatsResults { get; set; }

        void SetupContext(ContextT context);
        void OnBeginStage(ContextT context);
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
                DependsOn = new ReadOnlyCollection<string>(stepData.DependsOn)
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

    public class StageStepWriterDecorator : IWriter
    {
        readonly IWriter _writer;

        public ReadOnlyPipelineStageData StageData { get; set; }
        public ReadOnlyPipelineStepData StepData { get; set; }

        public IStageStepWriterFormatter SuccessFormatter { get; set; } = new IdempotentStageStepWriterFormatter();
        public IStageStepWriterFormatter InfoFormatter { get; set; } = new IdempotentStageStepWriterFormatter();
        public IStageStepWriterFormatter WarningFormatter { get; set; } = new DefaultStageStepWriterFormatter();
        public IStageStepWriterFormatter ErrorFormatter { get; set; } = new DefaultStageStepWriterFormatter();
        public IStageStepWriterFormatter FailureFormatter { get; set; } = new DefaultStageStepWriterFormatter();
        public IStageStepWriterFormatter CancellationFormatter { get; set; } = new DefaultStageStepWriterFormatter();

        public StageStepWriterDecorator(IWriter writer)
        {
            _writer = writer;
        }

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

        public bool PauseAfterEachStep { get; set; }
        public IPipelineStepData[] StepsDatas { get; set; }
        public IPipelineStageProgressWriter ProgressWriter { get; set; }
        public IWriter Writer { get; set; }
        public ITokenReplacer TokenReplacer { get; set; }
        public ITokenReplacerVariablesProvider<ContextT> TokenReplacerVariablesProvider { get; set; }
        public IPauseHandler PauseHandler { get; set; }
        public IPipelineStepStatsResults StatsResults { get; set; } = new PipelineStepStatsResults();
        public IPipelineProgressContextFactory ProgressContextFactory { get; set; } = new PipelineProgressContextFactory();
        public IReadOnlyPipelineDataFactory ReadOnlyPipelineStageDataFactory = new ReadOnlyPipelineDataFactory();

        public virtual void SetupContext(ContextT context)
        {
        }

        public virtual void OnBeginStage(ContextT context)
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

        void ReplaceTokensRecursively(object obj)
        {
            var propsWithReplacetoken = obj.GetType().GetProperties().Where(p => Attribute.IsDefined(p, typeof(PipelineStepReplaceTokenAttribute)));

            foreach (PropertyInfo prop in propsWithReplacetoken)
            {
                var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                if (type == typeof(string))
                {
                    string propertyValue = prop.GetValue(obj) as string;
                    if (null != propertyValue)
                        prop.SetValue(obj, TokenReplacer.Replace(propertyValue));
                }
                else
                {
                    var propertyValue = prop.GetValue(obj);
                    if (null != propertyValue)
                    {
                        var enumerable = propertyValue as IEnumerable;
                        if (null != enumerable)
                        {
                            foreach (var item in enumerable)
                                ReplaceTokensRecursively(item);
                        }
                        else 
                            ReplaceTokensRecursively(propertyValue);
                    }
                }
            }
        }

        public PipelineStepStatus[] DoStage(ContextT context)
        {
            SetupContext(context);

            var progressContext = ProgressContextFactory.CreateContext();
            var stageStepWriterDecorator = new StageStepWriterDecorator(Writer);

            HashSet<string> stepsComplete = new();
            var stepStatuses = new List<PipelineStepStatus>();

            StatsResults.NumStepsTotal = StepsDatas.Length;
            StatsResults.NumStepsCompleted = 0;
            StatsResults.NumStepsPartiallyCompleted = 0;
            StatsResults.NumStepsFailed = 0;
            StatsResults.NumStepsCancelled = 0;

            for (int stepIndex = 0; stepIndex < StepsDatas.Length; ++stepIndex)
            {
                var stepData = StepsDatas[stepIndex];

                // Perform token replacement in step string properties.
                TokenReplacer.Variables = new ReadOnlyDictionary<string, string>(TokenReplacerVariablesProvider.GetVariables(context));

                ReplaceTokensRecursively(stepData);

                progressContext.StepNumber = stepIndex + 1;
                progressContext.NumStepsTotal = StepsDatas.Length;

                PipelineStepStatus stepStatus;

                var uncompletedDependencies = stepData.DependsOn.Except(stepsComplete).ToArray();
                if (uncompletedDependencies.Length > 0)
                {
                    ProgressWriter.WriteStepDependenciesNotCompleted(progressContext, GetReadOnlyStepData(stepData));
                    stepStatus = PipelineStepStatus.Cancelled;
                }
                else
                {
                    ProgressWriter.WriteStepExecute(progressContext, GetReadOnlyStepData(stepData));

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
                        ProgressWriter.WriteStepCompleted(progressContext, GetReadOnlyStepData(stepData));
                        ++StatsResults.NumStepsCompleted;
                        stepsComplete.Add(stepData.Name);
                        break;
                    case PipelineStepStatus.PartiallyComplete:
                        ProgressWriter.WriteStepPartiallyCompleted(progressContext, GetReadOnlyStepData(stepData));
                        ++StatsResults.NumStepsPartiallyCompleted;
                        break;
                    case PipelineStepStatus.Failed:
                        ProgressWriter.WriteStepFailed(progressContext, GetReadOnlyStepData(stepData));
                        ++StatsResults.NumStepsFailed;
                        break;
                    case PipelineStepStatus.Cancelled:
                        ProgressWriter.WriteStepCancelled(progressContext, GetReadOnlyStepData(stepData));
                        ++StatsResults.NumStepsCancelled;
                        break;
                }
            }

            if (PauseAfterEachStep)
                PauseHandler.Pause();

            return stepStatuses.ToArray();
        }
    }

    public interface IPipeline<ContextT>
    {
        void Execute(ContextT context);
    }

    public class Pipeline<ContextT> : IPipeline<ContextT>
    {
        public IPipelineStage<ContextT>[] Stages { get; set; }

        public IPipelineProgressWriter ProgressWriter { get; set; }
        public IWriter Writer { get; set; }

        public IPipelineStatsResults StatsResults { get; set; } = new PipelineStatsResults();
        public IPipelineProgressContextFactory ProgressContextFactory { get; set; } = new PipelineProgressContextFactory();

        public Pipeline(IPipelineStage<ContextT>[] stages, IWriter writer, IPipelineProgressWriter progressWriter)
        {
            Stages = stages;
            Writer = writer;
            ProgressWriter = progressWriter;
        }

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
                ProgressWriter.WriteStageExecute(progressContext, stage.GetReadOnlyStageData());
                PipelineStepStatus[] stepsStatuses = stage.DoStage(context);

                int numStepsCompleted = stepsStatuses.Count(s => s == PipelineStepStatus.Complete);
                int numStepsPartiallyCompleted = stepsStatuses.Count(s => s == PipelineStepStatus.PartiallyComplete);
                int numStepsFailed = stepsStatuses.Count(s => s == PipelineStepStatus.Failed);
                int numStepsCancelled = stepsStatuses.Count(s => s == PipelineStepStatus.Cancelled);

                StatsResults.NumStepsCompleted += numStepsCompleted;
                StatsResults.NumStepsPartiallyCompleted += numStepsPartiallyCompleted;
                StatsResults.NumStepsFailed += numStepsFailed;
                StatsResults.NumStepsCancelled += numStepsCancelled;

                PipelineStepStatus status = PipelineStepStatus.Complete; // Assume that it will be fully completed.

                if (numStepsPartiallyCompleted > 0)
                    status = PipelineStepStatus.PartiallyComplete; // At least one step was not fully completed. Mark as partially complete.
                else if (numStepsCancelled > 0)
                    status = PipelineStepStatus.PartiallyComplete; // At least one step got cancelled. Mark as partially complete.
                else if (numStepsFailed > 0)
                    status = PipelineStepStatus.Failed; // One step failed. Mark as failed.

                switch (status)
                {
                    case PipelineStepStatus.Complete:
                        ProgressWriter.WriteStageCompleted(progressContext, stage.GetReadOnlyStageData());
                        ++StatsResults.NumStagesCompleted;
                        break;
                    case PipelineStepStatus.PartiallyComplete:
                        ProgressWriter.WriteStagePartiallyCompleted(progressContext, stage.GetReadOnlyStageData());
                        ++StatsResults.NumStagesPartiallyCompleted;
                        break;
                    case PipelineStepStatus.Failed:
                        ProgressWriter.WriteStageFailed(progressContext, stage.GetReadOnlyStageData());
                        ++StatsResults.NumStagesFailed;
                        break;
                    case PipelineStepStatus.Cancelled:
                        ProgressWriter.WriteStageCancelled(progressContext, stage.GetReadOnlyStageData());
                        ++StatsResults.NumStagesCancelled;
                        break;
                }
            }
        }
    }
}
