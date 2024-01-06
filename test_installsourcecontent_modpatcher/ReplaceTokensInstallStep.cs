using Pipelines;
using test_installsourcecontent;

namespace test_installsourcecontent_modpatcher
{
    public class ReplaceTokensInstallStepData : IPipelineStepData
    {
        public string Name { get; set; } = string.Empty;
        [PipelineStepReplaceToken]
        public string Description { get; set; } = string.Empty;
        public List<string> DependsOn { get; set; } = [];
        [PipelineStepReplaceToken]
        public List<string> Files { get; set; } = [];
    }

    public interface IReplaceTokensInstallStepEventHandler
    {
        void NoFilesSpecified();
        void FileDoesNotExist();
        void FileTokenReplacementSucceeded();
        void FileTokenReplacementFailed();
        void NoFilesProcessed();
    }

    public class ReplaceTokensInstallStep : IPipelineStep<Context>
    {
        IFileTokenReplacer _fileTokenReplacer;
        IReplaceTokensInstallStepEventHandler? _eventHandler;

        public ReplaceTokensInstallStep(IFileTokenReplacer fileTokenReplacer, IReplaceTokensInstallStepEventHandler? eventHandler = null)
        {
            _fileTokenReplacer = fileTokenReplacer;
            _eventHandler = eventHandler;
        }

        public PipelineStepStatus DoStep(Context context, IPipelineStepData stepData, IWriter writer)
        {
            var stepDataReplaceTokens = (ReplaceTokensInstallStepData)stepData;
            var Files = stepDataReplaceTokens.Files;

            if (null == Files || Files.Count <= 0)
            {
                _eventHandler?.NoFilesSpecified();
                writer.Error("No file(s) specified.");
                return PipelineStepStatus.Failed;
            }

            var files = Files.Select(file => PathExtensions.ConvertToUnixDirectorySeparator(context.FileSystem, context.FileSystem.Path.GetFullPath(file))).ToList();

            // Pass source content variables to token replacer.
            _fileTokenReplacer.Variables = context.GetSourceContentVariables();

            int numFilesProcessed = 0;
            PipelineStepStatus status = PipelineStepStatus.Complete;
            foreach (var filePath in files)
            {
                if (!context.FileSystem.File.Exists(filePath))
                {
                    _eventHandler?.FileDoesNotExist();
                    writer.Warning($"{filePath} does not exist. Skipping...");
                    status = PipelineStepStatus.PartiallyComplete;
                    continue;
                }

                writer.Info($"Replacing tokens in \"{filePath}\"");
                if (_fileTokenReplacer.ReplaceInFile(context.FileSystem, writer, filePath))
                {
                    // File successfully processed.
                    _eventHandler?.FileTokenReplacementSucceeded();
                    ++numFilesProcessed;
                }
                else
                {
                    // Other files might work, so mark it as partially completed.
                    status = PipelineStepStatus.PartiallyComplete;
                    _eventHandler?.FileTokenReplacementFailed();
                }
            }

            // If no file has been processed, mark it as failed.
            if (numFilesProcessed == 0)
            {
                _eventHandler?.NoFilesProcessed();
                status = PipelineStepStatus.Failed;
            }

            return status;
        }
    }
}