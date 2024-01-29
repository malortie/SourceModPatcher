using Pipelines;
using System.IO.Abstractions;

namespace SourceContentInstaller
{
    public interface IStepMapper<ConfigT>
    {
        IPipelineStepData Map(ConfigT jsonInstallStep);
    }

    public class StepsLoader<ConfigT>(IFileSystem fileSystem, IWriter writer, IConfigurationSerializer<IList<ConfigT>> serializer, IStepMapper<ConfigT> stepMapper)
    {
        readonly IStepMapper<ConfigT> _stepMapper = stepMapper;
        readonly IConfigurationSerializer<IList<ConfigT>> _serializer = serializer;
        readonly IFileSystem _fileSystem = fileSystem;
        readonly IWriter _writer = writer;

        public IList<IPipelineStepData> Load(string stepsFilePath)
        {
            var steps = new List<IPipelineStepData>();
            _writer.Info($"Reading {_fileSystem.Path.GetFileName(stepsFilePath)}");
            var deserializedStepsList = _serializer.Deserialize(_fileSystem.File.ReadAllText(stepsFilePath));
            if (null != deserializedStepsList)
                return deserializedStepsList.Select(a => _stepMapper.Map(a)).ToList();
            else
                throw new Exception($"Failed to read {stepsFilePath}");
        }
    }
}