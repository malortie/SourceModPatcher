using System.IO.Abstractions;
using Pipelines;

namespace SourceContentInstaller
{
    public interface IStepMapper<ConfigT>
    {
        IPipelineStepData Map(ConfigT jsonInstallStep);
    }

    public class StepsLoader<ConfigT>
    {
        IStepMapper<ConfigT> _stepMapper;
        IConfigurationSerializer<IList<ConfigT>> _serializer;
        IFileSystem _fileSystem;
        IWriter _writer;

        public StepsLoader(IFileSystem fileSystem, IWriter writer, IConfigurationSerializer<IList<ConfigT>> serializer, IStepMapper<ConfigT> stepMapper)
        {
            _fileSystem = fileSystem;
            _writer = writer;
            _serializer = serializer;
            _stepMapper = stepMapper;
        }

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