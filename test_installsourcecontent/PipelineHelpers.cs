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

    public interface IPipelineProgressWriter
    {
        void WriteStepDependenciesNotCompleted(IPipelineProgressContext pipelineContext, IPipelineStepData stepData);
        void WriteStepExecute(IPipelineProgressContext pipelineContext, IPipelineStepData stepData);
        void WriteStepCompleted(IPipelineProgressContext pipelineContext, IPipelineStepData stepData);
        void WriteStepPartiallyCompleted(IPipelineProgressContext pipelineContext, IPipelineStepData stepData);
        void WriteStepFailed(IPipelineProgressContext pipelineContext, IPipelineStepData stepData);
        void WriteStepCancelled(IPipelineProgressContext pipelineContext, IPipelineStepData stepData);
    }

    public class PipelineProgressWriter : IPipelineProgressWriter
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

        public void WriteStepDependenciesNotCompleted(IPipelineProgressContext pipelineContext, IPipelineStepData stepData)
        {
            _writer.WriteLine($"Step {stepData.Name} will not be executed: The following dependencies were not completed: <{string.Join(',', stepData.DependsOn)}>");
        }

        public void WriteStepExecute(IPipelineProgressContext pipelineContext, IPipelineStepData stepData)
        {
            _writer.WriteLine(Format(pipelineContext, stepData));
        }

        public void WriteStepCompleted(IPipelineProgressContext pipelineContext, IPipelineStepData stepData)
        {
            _writer.WriteLine($"{Format(pipelineContext, stepData)} [COMPLETED]");
        }

        public void WriteStepPartiallyCompleted(IPipelineProgressContext pipelineContext, IPipelineStepData stepData)
        {
            _writer.WriteLine($"{Format(pipelineContext, stepData)} [PARTIALLY COMPLETED]");
        }

        public void WriteStepFailed(IPipelineProgressContext pipelineContext, IPipelineStepData stepData)
        {
            _writer.WriteLine($"{Format(pipelineContext, stepData)} [FAILED]");
        }

        public void WriteStepCancelled(IPipelineProgressContext pipelineContext, IPipelineStepData stepData)
        {
            _writer.WriteLine($"{Format(pipelineContext, stepData)} [CANCELLED]");
        }
    }

    public interface IPipelineLogCollector
    {
        ReadOnlyCollection<string> GetInfos();
        List<KeyValuePair<string, List<string>>> GetInfosPerKeys();
        ReadOnlyCollection<string> GetInfos(string key);

        ReadOnlyCollection<string> GetWarnings();
        List<KeyValuePair<string, List<string>>> GetWarningsPerKeys();
        ReadOnlyCollection<string> GetWarnings(string key);

        ReadOnlyCollection<string> GetErrors();
        List<KeyValuePair<string, List<string>>> GetErrorsPerKeys();
        ReadOnlyCollection<string> GetErrors(string key);

        void LogInfo(string key, string message);
        void LogWarning(string key, string message);
        void LogError(string key, string message);
    }

    public class PipelineLogResult
    {
        public List<string> Infos { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
        public List<string> Errors { get; set; } = new();
    }

    public class PipelineLogCollector : IPipelineLogCollector
    {
        public Dictionary<string, PipelineLogResult> _entries = new();

        public ReadOnlyCollection<string> GetInfos() {
            return new ReadOnlyCollection<string>(_entries.SelectMany(kv => kv.Value.Infos).ToList());
        }
        public List<KeyValuePair<string, List<string>>> GetInfosPerKeys() {
            var result = new List<KeyValuePair<string, List<string>>>();
            foreach (var kv in _entries)
                result.Add(new KeyValuePair<string, List<string>>(kv.Key, kv.Value.Infos));
            return result;
        }
        public ReadOnlyCollection<string> GetInfos(string key) {
            return new ReadOnlyCollection<string>(_entries[key].Infos.ToList());
        }

        public ReadOnlyCollection<string> GetWarnings() {
            return new ReadOnlyCollection<string>(_entries.SelectMany(kv => kv.Value.Warnings).ToList());
        }
        public List<KeyValuePair<string, List<string>>> GetWarningsPerKeys() {
            var result = new List<KeyValuePair<string, List<string>>>();
            foreach (var kv in _entries)
                result.Add(new KeyValuePair<string, List<string>>(kv.Key, kv.Value.Warnings));
            return result;
        }
        public ReadOnlyCollection<string> GetWarnings(string key) {
            return new ReadOnlyCollection<string>(_entries[key].Warnings.ToList());
        }

        public ReadOnlyCollection<string> GetErrors() {
            return new ReadOnlyCollection<string>(_entries.SelectMany(kv => kv.Value.Errors).ToList());
        }
        public List<KeyValuePair<string, List<string>>> GetErrorsPerKeys() {
            var result = new List<KeyValuePair<string, List<string>>>();
            foreach (var kv in _entries)
                result.Add(new KeyValuePair<string, List<string>>(kv.Key, kv.Value.Errors));
            return result;
        }
        public ReadOnlyCollection<string> GetErrors(string key) {
            return new ReadOnlyCollection<string>(_entries[key].Errors.ToList());
        }

        public void LogInfo(string key, string message)
        {
            CreateKeyLogResultIfNull(key);
            _entries[key].Infos.Add(message);
        }
        public void LogWarning(string key, string message)
        {
            CreateKeyLogResultIfNull(key);
            _entries[key].Warnings.Add(message);
        }
        public void LogError(string key, string message)
        {
            CreateKeyLogResultIfNull(key);
            _entries[key].Errors.Add(message);
        }

        void CreateKeyLogResultIfNull(string key)
        {
            if (!_entries.ContainsKey(key))
                _entries[key] = new PipelineLogResult();
        }
    }

    public interface IPipelineLogFormatter
    {
        string FormatInfo(string message);
        string FormatWarning(string message);
        string FormatError(string message);
    }

    public class PipelineLogFormatter : IPipelineLogFormatter
    {
        public bool PrefixInfo { get; set; } = false;

        public string FormatInfo(string message)
        {
            if (PrefixInfo)
                return $"[INFO] {message}";
            else
                return message;
        }

        public string FormatWarning(string message)
        {
            return $"[WARNING] {message}";
        }

        public string FormatError(string message)
        {
            return $"[ERROR] {message}";
        }
    }

    public interface IPipelineLogReportWriter
    {
        void WriteInfos();
        void WriteWarnings();
        void WriteErrors();
    }

    public class PipelineLogReportWriter : IPipelineLogReportWriter
    {
        IPipelineLogCollector _collector;
        IWriter _writer;
        IPipelineLogFormatter _formatter;

        public PipelineLogReportWriter(IPipelineLogCollector collector, IWriter writer, IPipelineLogFormatter formatter)
        {
            _collector = collector;
            _writer = writer;
            _formatter = formatter;
        }

        public void WriteInfos()
        {
            foreach (var infosKV in _collector.GetInfosPerKeys())
            {
                foreach (var message in infosKV.Value)
                    _writer.WriteLine(_formatter.FormatInfo($"{infosKV.Key} {message}"));
            }
        }

        public void WriteWarnings()
        {
            foreach (var warningsKV in _collector.GetWarningsPerKeys())
            {
                foreach (var message in warningsKV.Value)
                    _writer.WriteLine(_formatter.FormatWarning($"{warningsKV.Key} {message}"));
            }
        }

        public void WriteErrors()
        {
            foreach (var errorsKV in _collector.GetErrorsPerKeys())
            {
                foreach (var message in errorsKV.Value)
                    _writer.WriteLine(_formatter.FormatError($"{errorsKV.Key} {message}"));
            }
        }
    }

    public interface IPipelineLogger
    {
        string Name { get; set; }
        void LogInfo(string message);
        void LogWarning(string message);
        void LogError(string message);
    }

    public class PipelineLogger : IPipelineLogger
    {
        IPipelineLogCollector _collector;
        IWriter _writer;

        public bool LogInfosToCollector { get; set; } = false;
        public string Name { get; set; } = "null";

        public PipelineLogger(IPipelineLogCollector collector, IWriter writer)
        {
            _collector = collector;
            _writer = writer;
        }

        public void LogInfo(string message)
        {
            _writer.WriteLine(message);
            if (LogInfosToCollector)
                _collector.LogInfo(Name, message);
        }

        public void LogWarning(string message)
        {
            _writer.WriteLine(message);
            _collector.LogWarning(Name, message);
        }

        public void LogError(string message)
        {
            _writer.WriteLine(message);
            _collector.LogError(Name, message);
        }
    }

    public interface IPipelineStatsResults
    {
        int NumStepsTotal { get; set; }
        int NumStepsCompleted { get; set; }
        int NumStepsPartiallyCompleted { get; set; }
        int NumStepsFailed { get; set; }
        int NumStepsCancelled { get; set; }
    }

    public class PipelineStatsResults : IPipelineStatsResults
    {
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
            _writer.WriteLine("=====================");
            _writer.WriteLine("Summary:");
            _writer.WriteLine("=====================");
            _writer.WriteLine($"Completed: {statsResults.NumStepsCompleted}");
            _writer.WriteLine($"Partially completed: {statsResults.NumStepsPartiallyCompleted}");
            _writer.WriteLine($"Failed: {statsResults.NumStepsFailed}");
            _writer.WriteLine($"Cancelled: {statsResults.NumStepsCancelled}");
            _writer.WriteLine("=====================");

            if (statsResults.NumStepsPartiallyCompleted != 0 ||
                statsResults.NumStepsFailed != 0 ||
                statsResults.NumStepsCancelled != 0)
            {
                _writer.WriteLine("One or more errors occured.");
            }
            else
            {
                _writer.WriteLine("All steps successfully completed.");
            }
        }
    }
}
