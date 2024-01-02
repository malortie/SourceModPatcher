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

    public class CopyFilesInstallStep : IPipelineStep<Context>
    {
        public PipelineStepStatus DoStep(Context context, IPipelineStepData stepData, IWriter writer)
        {
            return PipelineStepStatus.Complete;
        }
    }
}