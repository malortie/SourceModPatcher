using System;
using test_installsourcecontent;

namespace test_installsourcecontent_modpatcher
{
    public class CopyFilesInstallStepDataFile
    {
        [PipelineStepReplaceToken]
        public string Source { get; set; } = string.Empty;
        [PipelineStepReplaceToken]
        public string Destination { get; set; } = string.Empty;
    }

    public class CopyFilesInstallStepData : IPipelineStepData
    {
        public string Name { get; set; } = string.Empty;
        [PipelineStepReplaceToken]
        public string Description { get; set; } = string.Empty;
        public List<string> DependsOn { get; set; } = [];
        [PipelineStepReplaceToken]
        public List<CopyFilesInstallStepDataFile> Files { get; set; } = [];
    }

    public interface ICopyFilesInstallStepEventHandler
    {
        void NoFilesSpecified();
        void BlankSourceEntry();
        void BlankDestinationEntry();
        void SourceFileDoesNotExist();
        void FileCopySuccess();
        void FileCopyFailed();
        void NoFilesCopied();
    }

    public class CopyFilesInstallStep : IPipelineStep<Context>
    {
        ICopyFilesInstallStepEventHandler? _eventHandler;

        public CopyFilesInstallStep(ICopyFilesInstallStepEventHandler? eventHandler = null)
        {
            _eventHandler = eventHandler;
        }

        public PipelineStepStatus DoStep(Context context, IPipelineStepData stepData, IWriter writer)
        {
            var stepDataCopyFiles = (CopyFilesInstallStepData)stepData;
            var Files = stepDataCopyFiles.Files;

            if (null == Files || Files.Count <= 0)
            {
                _eventHandler?.NoFilesSpecified();
                writer.Error("No file(s) specified.");
                return PipelineStepStatus.Failed;
            }
            else
            {
                // Check for empty source/destination strings.
                List<int> emptySrcIndices = [];
                List<int> emptyDestIndices = [];
                for (int i = 0; i < Files.Count; i++)
                {
                    if (Files[i].Source == string.Empty)
                        emptySrcIndices.Add(i);
                    if (Files[i].Destination == string.Empty)
                        emptyDestIndices.Add(i);
                }

                if (emptySrcIndices.Count > 0)
                {
                    _eventHandler?.BlankSourceEntry();
                    writer.Error($"Source entries [{string.Join(',', emptySrcIndices)}] are blank or empty.");
                    return PipelineStepStatus.Failed;
                }

                if (emptyDestIndices.Count > 0)
                {
                    _eventHandler?.BlankDestinationEntry();
                    writer.Error($"Destination entries [{string.Join(',', emptyDestIndices)}] are blank or empty.");
                    return PipelineStepStatus.Failed;
                }
            }

            var files = Files.Select(file => new CopyFilesInstallStepDataFile
            {
                Source = PathExtensions.ConvertToUnixDirectorySeparator(context.FileSystem, context.FileSystem.Path.GetFullPath(file.Source)),
                Destination = PathExtensions.ConvertToUnixDirectorySeparator(context.FileSystem, context.FileSystem.Path.GetFullPath(file.Destination))
            }).ToList();

            int numCopiedFiles = 0;
            PipelineStepStatus status = PipelineStepStatus.Complete;
            foreach (var file in files)
            {
                string sourceFilePath = file.Source;
                string destFilePath = file.Destination;
                if (!context.FileSystem.File.Exists(sourceFilePath))
                {
                    _eventHandler?.SourceFileDoesNotExist();
                    writer.Warning($"{sourceFilePath} does not exist. Skipping...");
                    status = PipelineStepStatus.PartiallyComplete;
                    continue;
                }

                writer.Info($"Copying \"{sourceFilePath}\" to \"{destFilePath}\"");
                try
                {
                    string? destFileDir = context.FileSystem.Path.GetDirectoryName(destFilePath);
                    if (null == destFileDir)
                    {
                        writer.Error($"Failed to get directory name for {destFilePath}");
                        // Other files copy might work, so mark it as partially completed.
                        status = PipelineStepStatus.PartiallyComplete;
                        continue;
                    }

                    if (!context.FileSystem.Directory.Exists(destFileDir))
                        context.FileSystem.Directory.CreateDirectory(destFileDir);

                    // Copy file to destination. Overwrite if it exists.
                    context.FileSystem.File.Copy(sourceFilePath, destFilePath, true);
                    // File successfully copied.
                    _eventHandler?.FileCopySuccess();
                    ++numCopiedFiles;
                }
                catch (Exception e)
                {
                    writer.Error(e.Message);
                    // Other files copy might work, so mark it as partially completed.
                    status = PipelineStepStatus.PartiallyComplete;
                    _eventHandler?.FileCopyFailed();
                }
            }

            // If no file has been copied, mark it as failed.
            if (numCopiedFiles == 0)
            {
                _eventHandler?.NoFilesCopied();
                status = PipelineStepStatus.Failed;
            }

            return status;
        }
    }
}