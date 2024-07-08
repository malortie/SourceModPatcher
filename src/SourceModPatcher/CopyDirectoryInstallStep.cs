using Pipelines;
using SourceContentInstaller;

namespace SourceModPatcher
{
    public class CopyDirectoryInstallStepData : IPipelineStepData
    {
        public string Name { get; set; } = string.Empty;
        [PipelineStepReplaceToken]
        public string Description { get; set; } = string.Empty;
        public List<string> DependsOn { get; set; } = [];
        [PipelineStepReplaceToken]
        public string Source { get; set; } = string.Empty;
        [PipelineStepReplaceToken]
        public string Destination { get; set; } = string.Empty;
    }

    public interface ICopyDirectoryInstallStepEventHandler
    {
        void NoSourceSpecified();
        void NoDestinationSpecified();
        void BlankSourceEntry();
        void BlankDestinationEntry();
        void DirectoryCopySuccess();
        void DirectoryCopyFailed();
    }

    public class CopyDirectoryInstallStep(IDirectoryCopier directoryCopier, ICopyDirectoryInstallStepEventHandler? eventHandler = null) : IPipelineStep<Context>
    {
        readonly IDirectoryCopier _directoryCopier = directoryCopier;
        readonly ICopyDirectoryInstallStepEventHandler? _eventHandler = eventHandler;

        public PipelineStepStatus DoStep(Context context, IPipelineStepData stepData, IWriter writer)
        {
            var stepDataCopyDirectory = (CopyDirectoryInstallStepData)stepData;
            var Source = stepDataCopyDirectory.Source;
            var Destination = stepDataCopyDirectory.Destination;

            if (null == Source)
            {
                _eventHandler?.NoSourceSpecified();
                writer.Error("No source directory specified.");
                return PipelineStepStatus.Failed;
            }

            if (Source == string.Empty)
            {
                _eventHandler?.BlankSourceEntry();
                writer.Error("Source directory is blank or empty.");
                return PipelineStepStatus.Failed;
            }

            if (null == Destination)
            {
                _eventHandler?.NoDestinationSpecified();
                writer.Error("No destination directory specified.");
                return PipelineStepStatus.Failed;
            }

            if (Destination == string.Empty)
            {
                _eventHandler?.BlankDestinationEntry();
                writer.Error("Destination directory is blank or empty.");
                return PipelineStepStatus.Failed;
            }

            Source = PathExtensions.ConvertToUnixDirectorySeparator(context.FileSystem, Source);
            Destination = PathExtensions.ConvertToUnixDirectorySeparator(context.FileSystem, Destination);

            var directoryCopyResult = _directoryCopier.CopyDirectory(context.FileSystem, writer, Source, Destination);
            if (directoryCopyResult.Success)
            {
                // All files successfully copied.
                _eventHandler?.DirectoryCopySuccess();
                return PipelineStepStatus.Complete;
            }
            else
            {
                // Not all files were copied.
                _eventHandler?.DirectoryCopyFailed();
                return PipelineStepStatus.Failed;
            }
        }
    }
}
