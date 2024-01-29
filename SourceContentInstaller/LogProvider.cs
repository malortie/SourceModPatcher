using NLog.Targets;
using Pipelines;
using System.Collections.Immutable;

namespace SourceContentInstaller
{
    public class LogProvider(MemoryTarget warningMemoryTarget, MemoryTarget errorMemoryTarget) : ILogProvider
    {
        readonly MemoryTarget _warningMemoryTarget = warningMemoryTarget, _errorMemoryTarget = errorMemoryTarget;

        public ImmutableList<string> GetErrors()
        {
            return [.. _errorMemoryTarget.Logs];
        }

        public ImmutableList<string> GetInfos()
        {
            throw new NotImplementedException();
        }

        public ImmutableList<string> GetWarnings()
        {
            return [.. _warningMemoryTarget.Logs];
        }
    }
}