using NLog.Targets;
using Pipelines;
using System.Collections.Immutable;

namespace test_installsourcecontent
{
    public class LogProvider : ILogProvider
    {
        readonly MemoryTarget _warningMemoryTarget, _errorMemoryTarget;
        public LogProvider(MemoryTarget warningMemoryTarget, MemoryTarget errorMemoryTarget)
        {
            _warningMemoryTarget = warningMemoryTarget;
            _errorMemoryTarget = errorMemoryTarget;
        }

        public ImmutableList<string> GetErrors()
        {
            return _errorMemoryTarget.Logs.ToImmutableList();
        }

        public ImmutableList<string> GetInfos()
        {
            throw new NotImplementedException();
        }

        public ImmutableList<string> GetWarnings()
        {
            return _warningMemoryTarget.Logs.ToImmutableList();
        }
    }
}