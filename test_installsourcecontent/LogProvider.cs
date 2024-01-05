using NLog.Targets;
using System.Collections.ObjectModel;
using Pipelines;

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

        public ReadOnlyCollection<string> GetErrors()
        {
            return new ReadOnlyCollection<string>(_errorMemoryTarget.Logs);
        }

        public ReadOnlyCollection<string> GetInfos()
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<string> GetWarnings()
        {
            return new ReadOnlyCollection<string>(_warningMemoryTarget.Logs);
        }
    }
}