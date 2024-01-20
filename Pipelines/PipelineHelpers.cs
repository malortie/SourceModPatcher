using System.Collections.Immutable;

namespace Pipelines
{

    public interface ILogProvider
    {
        ImmutableList<string> GetInfos();
        ImmutableList<string> GetWarnings();
        ImmutableList<string> GetErrors();
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
                _consoleWriter.Warning($"[WARNING] {warning}");
        }

        public void WriteErrors()
        {
            foreach (var error in _logProvider.GetErrors())
                _consoleWriter.Error($"[ERROR] {error}");
        }
    }

    public interface IPipelineStepStatsResults : ICloneable
    {
        int NumStepsTotal { get; set; }
        int NumStepsCompleted { get; set; }
        int NumStepsPartiallyCompleted { get; set; }
        int NumStepsFailed { get; set; }
        int NumStepsCancelled { get; set; }
    }

    public class PipelineStepStatsResults : IPipelineStepStatsResults
    {
        public PipelineStepStatsResults()
        {
        }

        public PipelineStepStatsResults(IPipelineStepStatsResults statsResults)
        {
            NumStepsTotal = statsResults.NumStepsTotal;
            NumStepsCompleted = statsResults.NumStepsCompleted;
            NumStepsPartiallyCompleted = statsResults.NumStepsPartiallyCompleted;
            NumStepsFailed = statsResults.NumStepsFailed;
            NumStepsCancelled = statsResults.NumStepsCancelled;
        }

        public int NumStepsTotal { get; set; }
        public int NumStepsCompleted { get; set; }
        public int NumStepsPartiallyCompleted { get; set; }
        public int NumStepsFailed { get; set; }
        public int NumStepsCancelled { get; set; }

        public object Clone()
        {
            return new PipelineStepStatsResults(this);
        }
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
