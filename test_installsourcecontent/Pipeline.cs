using AutoMapper;
using AutoMapper.Internal.Mappers;
using System.Collections.Immutable;
using System.IO.Abstractions;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace test_installsourcecontent
{
    // More restricted context for steps to disallow access to certain properties.
    public class StepContext
    {
        Context _context;

        public StepContext(Context context)
        {
            _context = context;
        }

        public IFileSystem FileSystem { get { return _context.FileSystem; } }

        public string GetSteamAppInstallDir()
        {
            return _context.GetSteamAppInstallDir();
        }

        public void SaveVariable(string name, string value)
        {
            _context.SaveVariable(name, value);
        }

        public string GetContentInstallDir()
        {
            return _context.GetContentInstallDir();
        }
    }

    public enum PipelineStepStatus
    {
        Complete = 0,
        PartiallyComplete = 1,
        Failed = 2,
        Cancelled = 3,
    }

    public interface IPipelineStepData
    {
        string Name { get; set; }
        string Description { get; set; }
        List<string> DependsOn { get; set; }
    }

    public interface IPipelineStep
    {
        PipelineStepStatus DoStep(StepContext context, IPipelineStepData stepData, IPipelineLogger logger);
    }

    public interface IPipelineStage
    {
        string Name { get; set; }

        string Description { get; set; }

        IPipelineStepData[] StepsDatas { get; set; }

        IPipelineStepStatsResults StatsResults { get; set; }

        PipelineStepStatus[] DoStage(Context context, IPipelineLogger logger);
    }

    public class Pipeline
    {
        IPipelineStage[] _stages;

        public IPipelineProgressWriter ProgressWriter { get; set; }
        public IPipelineLogger Logger { get; set; }

        public IPipelineStatsResults StatsResults { get; set; } = new PipelineStatsResults();

        public Pipeline(IPipelineStage[] stages) 
        {
            _stages = stages;
        }

        public void Execute(Context context)
        {
            StatsResults.NumStagesTotal = _stages.Length;
            StatsResults.NumStagesCompleted = 0;
            StatsResults.NumStagesPartiallyCompleted = 0;
            StatsResults.NumStagesFailed = 0;
            StatsResults.NumStagesCancelled = 0;

            StatsResults.NumStepsTotal = _stages.Sum(a => a.StepsDatas.Length);
            StatsResults.NumStepsCompleted = 0;
            StatsResults.NumStepsPartiallyCompleted = 0;
            StatsResults.NumStepsFailed = 0;
            StatsResults.NumStepsCancelled = 0;

            var progressContext = new PipelineProgressContext();
            progressContext.NumStepsTotal = _stages.Length;

            for (int stageIndex = 0; stageIndex < _stages.Length; ++stageIndex)
            {
                var stage = _stages[stageIndex];

                progressContext.StepNumber = stageIndex + 1;

                // Use the stage name for the logger name.
                Logger.Name = stage.Name;

                // Execute stage.
                ProgressWriter.WriteStageExecute(progressContext, stage);
                PipelineStepStatus[] stepsStatuses = stage.DoStage(context, Logger);

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
