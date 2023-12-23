using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace test_installsourcecontent
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

    public interface IPipelineProgressWriter<ContextT>
    {
        void WriteStepDependenciesNotCompleted(IPipelineProgressContext pipelineContext, IPipelineStepData stepData);
        void WriteStepExecute(IPipelineProgressContext pipelineContext, IPipelineStepData stepData);
        void WriteStepCompleted(IPipelineProgressContext pipelineContext, IPipelineStepData stepData);
        void WriteStepPartiallyCompleted(IPipelineProgressContext pipelineContext, IPipelineStepData stepData);
        void WriteStepFailed(IPipelineProgressContext pipelineContext, IPipelineStepData stepData);
        void WriteStepCancelled(IPipelineProgressContext pipelineContext, IPipelineStepData stepData);

        void WriteStageExecute(IPipelineProgressContext pipelineContext, IPipelineStage<ContextT> stage);
        void WriteStageCompleted(IPipelineProgressContext pipelineContext, IPipelineStage<ContextT> stage);
        void WriteStagePartiallyCompleted(IPipelineProgressContext pipelineContext, IPipelineStage<ContextT> stage);
        void WriteStageFailed(IPipelineProgressContext pipelineContext, IPipelineStage<ContextT> stage);
        void WriteStageCancelled(IPipelineProgressContext pipelineContext, IPipelineStage<ContextT> stage);
    }

    public class PipelineProgressWriter<ContextT> : IPipelineProgressWriter<ContextT>
    {
        ILogger _logger;

        public PipelineProgressWriter(ILogger logger)
        {
            _logger = logger;
        }

        string Format(IPipelineProgressContext pipelineContext, IPipelineStepData stepData)
        {
            return $"({pipelineContext.StepNumber}/{pipelineContext.NumStepsTotal}) {stepData.Description}";
        }

        string Format(IPipelineProgressContext pipelineContext, IPipelineStage<ContextT> stage)
        {
            return $"({pipelineContext.StepNumber}/{pipelineContext.NumStepsTotal}) {stage.Description}";
        }

        public void WriteStepDependenciesNotCompleted(IPipelineProgressContext pipelineContext, IPipelineStepData stepData)
        {
            _logger.LogWarning($"Step {stepData.Name} will not be executed: The following dependencies were not completed: <{string.Join(',', stepData.DependsOn)}>");
        }

        public void WriteStepExecute(IPipelineProgressContext pipelineContext, IPipelineStepData stepData)
        {
            _logger.LogInfo(Format(pipelineContext, stepData));
        }

        public void WriteStepCompleted(IPipelineProgressContext pipelineContext, IPipelineStepData stepData)
        {
            _logger.LogInfo($"{Format(pipelineContext, stepData)} [COMPLETED]");
        }

        public void WriteStepPartiallyCompleted(IPipelineProgressContext pipelineContext, IPipelineStepData stepData)
        {
            _logger.LogInfo($"{Format(pipelineContext, stepData)} [PARTIALLY COMPLETED]");
        }

        public void WriteStepFailed(IPipelineProgressContext pipelineContext, IPipelineStepData stepData)
        {
            _logger.LogInfo($"{Format(pipelineContext, stepData)} [FAILED]");
        }

        public void WriteStepCancelled(IPipelineProgressContext pipelineContext, IPipelineStepData stepData)
        {
            _logger.LogInfo($"{Format(pipelineContext, stepData)} [CANCELLED]");
        }

        public void WriteStageExecute(IPipelineProgressContext pipelineContext, IPipelineStage<ContextT> stage)
        {
            _logger.LogInfo(Format(pipelineContext, stage));
        }

        public void WriteStageCompleted(IPipelineProgressContext pipelineContext, IPipelineStage<ContextT> stage)
        {
            _logger.LogInfo($"{Format(pipelineContext, stage)} [COMPLETED]");
        }

        public void WriteStagePartiallyCompleted(IPipelineProgressContext pipelineContext, IPipelineStage<ContextT> stage)
        {
            _logger.LogInfo($"{Format(pipelineContext, stage)} [PARTIALLY COMPLETED]");
        }

        public void WriteStageFailed(IPipelineProgressContext pipelineContext, IPipelineStage<ContextT> stage)
        {
            _logger.LogInfo($"{Format(pipelineContext, stage)} [FAILED]");
        }

        public void WriteStageCancelled(IPipelineProgressContext pipelineContext, IPipelineStage<ContextT> stage)
        {
            _logger.LogInfo($"{Format(pipelineContext, stage)} [CANCELLED]");
        }
    }

    public interface IConsoleLogWriter
    {
        void WriteInfo(string message);
        void WriteWarning(string message);
        void WriteError(string message);
    }

    public interface ILogProvider
    {
        ReadOnlyCollection<string> GetInfos();
        ReadOnlyCollection<string> GetWarnings();
        ReadOnlyCollection<string> GetErrors();
    }

    public interface IConsoleLogReportWriter
    {
        void WriteInfos();
        void WriteWarnings();
        void WriteErrors();
    }

    public class ConsoleLogReportWriter : IConsoleLogReportWriter
    {
        IConsoleLogWriter _logWriter;
        ILogProvider _logProvider;

        public ConsoleLogReportWriter(IConsoleLogWriter logWriter, ILogProvider logProvider)
        {
            _logWriter = logWriter;
            _logProvider = logProvider;
        }

        public void WriteInfos()
        {
            foreach (var info in _logProvider.GetInfos())
                _logWriter.WriteInfo(info);
        }

        public void WriteWarnings()
        {
            foreach (var warning in _logProvider.GetWarnings())
                _logWriter.WriteWarning(warning);
        }

        public void WriteErrors()
        {
            foreach (var error in _logProvider.GetErrors())
                _logWriter.WriteError(error);
        }
    }

    public interface IPipelineStepStatsResults
    {
        int NumStepsTotal { get; set; }
        int NumStepsCompleted { get; set; }
        int NumStepsPartiallyCompleted { get; set; }
        int NumStepsFailed { get; set; }
        int NumStepsCancelled { get; set; }
    }

    public class PipelineStepStatsResults : IPipelineStepStatsResults
    {
        public int NumStepsTotal { get; set; }
        public int NumStepsCompleted { get; set; }
        public int NumStepsPartiallyCompleted { get; set; }
        public int NumStepsFailed { get; set; }
        public int NumStepsCancelled { get; set; }
    }

    public interface IPipelineStatsResults
    {
        int NumStagesTotal { get; set; }
        int NumStagesCompleted { get; set; }
        int NumStagesPartiallyCompleted { get; set; }
        int NumStagesFailed { get; set; }
        int NumStagesCancelled { get; set; }

        int NumStepsTotal { get; set; }
        int NumStepsCompleted { get; set; }
        int NumStepsPartiallyCompleted { get; set; }
        int NumStepsFailed { get; set; }
        int NumStepsCancelled { get; set; }
    }

    public class PipelineStatsResults : IPipelineStatsResults
    {
        public int NumStagesTotal { get; set; }
        public int NumStagesCompleted { get; set; }
        public int NumStagesPartiallyCompleted { get; set; }
        public int NumStagesFailed { get; set; }
        public int NumStagesCancelled { get; set; }

        public int NumStepsTotal { get; set; }
        public int NumStepsCompleted { get; set; }
        public int NumStepsPartiallyCompleted { get; set; }
        public int NumStepsFailed { get; set; }
        public int NumStepsCancelled { get; set; }
    }

    public interface IPipelineStatsWriter
    {
        void WriteStats(IPipelineStatsResults statsResults);
    }

    public class PipelineStatsWriter : IPipelineStatsWriter
    {
        ILogger _logger;

        public PipelineStatsWriter(ILogger logger)
        {
            _logger = logger;
        }

        public void WriteStats(IPipelineStatsResults statsResults)
        {
            _logger.LogInfo("=====================");
            _logger.LogInfo("Summary:");
            _logger.LogInfo("=====================");
            _logger.LogInfo($"[Stages]");
            _logger.LogInfo($"Completed: {statsResults.NumStagesCompleted}");
            _logger.LogInfo($"Partially completed: {statsResults.NumStagesPartiallyCompleted}");
            _logger.LogInfo($"Failed: {statsResults.NumStagesFailed}");
            _logger.LogInfo($"Cancelled: {statsResults.NumStagesCancelled}");
            _logger.LogInfo(string.Empty);
            _logger.LogInfo($"[Steps]");
            _logger.LogInfo($"Completed: {statsResults.NumStepsCompleted}");
            _logger.LogInfo($"Partially completed: {statsResults.NumStepsPartiallyCompleted}");
            _logger.LogInfo($"Failed: {statsResults.NumStepsFailed}");
            _logger.LogInfo($"Cancelled: {statsResults.NumStepsCancelled}");
            _logger.LogInfo("=====================");
        }
    }
}
