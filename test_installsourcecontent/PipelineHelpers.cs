using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;

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
        IWriter _writer;

        public PipelineProgressWriter(IWriter writer)
        {
            _writer = writer;
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
            _writer.Warning($"Step {stepData.Name} will not be executed: The following dependencies were not completed: <{string.Join(',', stepData.DependsOn)}>");
        }

        public void WriteStepExecute(IPipelineProgressContext pipelineContext, IPipelineStepData stepData)
        {
            _writer.Info(Format(pipelineContext, stepData));
        }

        public void WriteStepCompleted(IPipelineProgressContext pipelineContext, IPipelineStepData stepData)
        {
            _writer.Success($"{Format(pipelineContext, stepData)} [COMPLETED]");
        }

        public void WriteStepPartiallyCompleted(IPipelineProgressContext pipelineContext, IPipelineStepData stepData)
        {
            _writer.Warning($"{Format(pipelineContext, stepData)} [PARTIALLY COMPLETED]");
        }

        public void WriteStepFailed(IPipelineProgressContext pipelineContext, IPipelineStepData stepData)
        {
            _writer.Failure($"{Format(pipelineContext, stepData)} [FAILED]");
        }

        public void WriteStepCancelled(IPipelineProgressContext pipelineContext, IPipelineStepData stepData)
        {
            _writer.Cancellation($"{Format(pipelineContext, stepData)} [CANCELLED]");
        }

        public void WriteStageExecute(IPipelineProgressContext pipelineContext, IPipelineStage<ContextT> stage)
        {
            _writer.Info(Format(pipelineContext, stage));
        }

        public void WriteStageCompleted(IPipelineProgressContext pipelineContext, IPipelineStage<ContextT> stage)
        {
            _writer.Success($"{Format(pipelineContext, stage)} [COMPLETED]");
        }

        public void WriteStagePartiallyCompleted(IPipelineProgressContext pipelineContext, IPipelineStage<ContextT> stage)
        {
            _writer.Warning($"{Format(pipelineContext, stage)} [PARTIALLY COMPLETED]");
        }

        public void WriteStageFailed(IPipelineProgressContext pipelineContext, IPipelineStage<ContextT> stage)
        {
            _writer.Failure($"{Format(pipelineContext, stage)} [FAILED]");
        }

        public void WriteStageCancelled(IPipelineProgressContext pipelineContext, IPipelineStage<ContextT> stage)
        {
            _writer.Cancellation($"{Format(pipelineContext, stage)} [CANCELLED]");
        }
    }

    public interface IConsoleWriter
    {
        void Success(string message);
        void Info(string message);
        void Warning(string message);
        void Error(string message);
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
        IConsoleWriter _consoleWriter;
        ILogProvider _logProvider;

        public ConsoleLogReportWriter(IConsoleWriter consoleWriter, ILogProvider logProvider)
        {
            _consoleWriter = consoleWriter;
            _logProvider = logProvider;
        }

        public void WriteInfos()
        {
            foreach (var info in _logProvider.GetInfos())
                _consoleWriter.Info(info);
        }

        public void WriteWarnings()
        {
            foreach (var warning in _logProvider.GetWarnings())
                _consoleWriter.Warning(warning);
        }

        public void WriteErrors()
        {
            foreach (var error in _logProvider.GetErrors())
                _consoleWriter.Error(error);
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
        IWriter _writer;

        public PipelineStatsWriter(IWriter writer)
        {
            _writer = writer;
        }

        public void WriteStats(IPipelineStatsResults statsResults)
        {
            _writer.Info("=====================");
            _writer.Info("Summary:");
            _writer.Info("=====================");
            _writer.Info($"[Stages]");
            _writer.Info($"Completed: {statsResults.NumStagesCompleted}");
            _writer.Info($"Partially completed: {statsResults.NumStagesPartiallyCompleted}");
            _writer.Info($"Failed: {statsResults.NumStagesFailed}");
            _writer.Info($"Cancelled: {statsResults.NumStagesCancelled}");
            _writer.Info(string.Empty);
            _writer.Info($"[Steps]");
            _writer.Info($"Completed: {statsResults.NumStepsCompleted}");
            _writer.Info($"Partially completed: {statsResults.NumStepsPartiallyCompleted}");
            _writer.Info($"Failed: {statsResults.NumStepsFailed}");
            _writer.Info($"Cancelled: {statsResults.NumStepsCancelled}");
            _writer.Info("=====================");
        }
    }
}
