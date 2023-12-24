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

    public interface IPipelineStage<ContextT>
    {
        string Name { get; set; }

        string Description { get; set; }

        IPipelineStepData[] StepsDatas { get; set; }

        IPipelineStepStatsResults StatsResults { get; set; }

        void SetupContext(ContextT context);
        void OnBeginStage(ContextT context);
        IPipelineStep<ContextT> GetStepForStepData(IPipelineStepData stepData);

        PipelineStepStatus[] DoStage(ContextT context);
    }

    public interface IStepWriterFormatter
    {
        string Format(string message, string stageName, string stepName);
    }

    public class DefaultStepWriterFormatter : IStepWriterFormatter
    {
        public string Format(string message, string stageName, string stepName)
        {
            return $"{message} [{stageName}][{stepName}]";
        }
    }

    public class IdempotentWriterFormatter : IStepWriterFormatter
    {
        public string Format(string message, string stageName, string stepName)
        {
            return message;
        }
    }

    public class StepWriterDecorator : IWriter
    {
        readonly IWriter _writer;

        public string StageName { get; set; } = string.Empty;
        public string StepName { get; set; } = string.Empty;

        public IStepWriterFormatter SuccessFormatter { get; set; } = new IdempotentWriterFormatter();
        public IStepWriterFormatter InfoFormatter { get; set; } = new IdempotentWriterFormatter();
        public IStepWriterFormatter WarningFormatter { get; set; } = new DefaultStepWriterFormatter();
        public IStepWriterFormatter ErrorFormatter { get; set; } = new DefaultStepWriterFormatter();
        public IStepWriterFormatter FailureFormatter { get; set; } = new DefaultStepWriterFormatter();
        public IStepWriterFormatter CancellationFormatter { get; set; } = new DefaultStepWriterFormatter();

        public StepWriterDecorator(IWriter writer)
        {
            _writer = writer;
        }

        public void Success(string message)
        {
            _writer.Success(SuccessFormatter.Format(message, StageName, StepName));
        }

        public void Info(string message)
        {
            _writer.Info(InfoFormatter.Format(message, StageName, StepName));
        }

        public void Warning(string message)
        {
            _writer.Warning(WarningFormatter.Format(message, StageName, StepName));
        }

        public void Error(string message)
        {
            _writer.Error(ErrorFormatter.Format(message, StageName, StepName));
        }

        public void Failure(string message)
        {
            _writer.Failure(FailureFormatter.Format(message, StageName, StepName));
        }

        public void Cancellation(string message)
        {
            _writer.Cancellation(CancellationFormatter.Format(message, StageName, StepName));
        }
    }

    public abstract class PipelineStage<ContextT> : IPipelineStage<ContextT>
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public bool PauseAfterEachStep { get; set; }
        public IPipelineStepData[] StepsDatas { get; set; }
        public IPipelineProgressWriter<ContextT> ProgressWriter { get; set; }
        public IWriter Writer { get; set; }
        public ITokenReplacer TokenReplacer { get; set; }
        public ITokenReplacerVariablesProvider<ContextT> TokenReplacerVariablesProvider { get; set; }
        public IPauseHandler PauseHandler { get; set; }
        public IPipelineStepStatsResults StatsResults { get; set; } = new PipelineStepStatsResults();
        public IPipelineProgressContextFactory ProgressContextFactory { get; set; } = new PipelineProgressContextFactory();

        public virtual void SetupContext(ContextT context)
        {
        }

        public virtual void OnBeginStage(ContextT context)
        {
        }

        public abstract IPipelineStep<ContextT> GetStepForStepData(IPipelineStepData stepData);

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
            var stepWriterDecorator = new StepWriterDecorator(Writer);

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
                    ProgressWriter.WriteStepDependenciesNotCompleted(progressContext, stepData);
                    stepStatus = PipelineStepStatus.Cancelled;
                }
                else
                {
                    ProgressWriter.WriteStepExecute(progressContext, stepData);

                    // Setup writer decorator.
                    stepWriterDecorator.StageName = Name;
                    stepWriterDecorator.StepName = stepData.Name;

                    // Execute step.
                    stepStatus = GetStepForStepData(stepData).DoStep(context, stepData, stepWriterDecorator);
                }

                stepStatuses.Add(stepStatus);

                switch (stepStatus)
                {
                    case PipelineStepStatus.Complete:
                        ProgressWriter.WriteStepCompleted(progressContext, stepData);
                        ++StatsResults.NumStepsCompleted;
                        stepsComplete.Add(stepData.Name);
                        break;
                    case PipelineStepStatus.PartiallyComplete:
                        ProgressWriter.WriteStepPartiallyCompleted(progressContext, stepData);
                        ++StatsResults.NumStepsPartiallyCompleted;
                        break;
                    case PipelineStepStatus.Failed:
                        ProgressWriter.WriteStepFailed(progressContext, stepData);
                        ++StatsResults.NumStepsFailed;
                        break;
                    case PipelineStepStatus.Cancelled:
                        ProgressWriter.WriteStepCancelled(progressContext, stepData);
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

        public IPipelineProgressWriter<ContextT> ProgressWriter { get; set; }
        public IWriter Writer { get; set; }

        public IPipelineStatsResults StatsResults { get; set; } = new PipelineStatsResults();
        public IPipelineProgressContextFactory ProgressContextFactory { get; set; } = new PipelineProgressContextFactory();

        public Pipeline(IPipelineStage<ContextT>[] stages, IWriter writer, IPipelineProgressWriter<ContextT> progressWriter)
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
                ProgressWriter.WriteStageExecute(progressContext, stage);
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
                        ProgressWriter.WriteStageCompleted(progressContext, stage);
                        ++StatsResults.NumStagesCompleted;
                        break;
                    case PipelineStepStatus.PartiallyComplete:
                        ProgressWriter.WriteStagePartiallyCompleted(progressContext, stage);
                        ++StatsResults.NumStagesPartiallyCompleted;
                        break;
                    case PipelineStepStatus.Failed:
                        ProgressWriter.WriteStageFailed(progressContext, stage);
                        ++StatsResults.NumStagesFailed;
                        break;
                    case PipelineStepStatus.Cancelled:
                        ProgressWriter.WriteStageCancelled(progressContext, stage);
                        ++StatsResults.NumStagesCancelled;
                        break;
                }
            }
        }
    }
}
