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

    public class ReplaceTokensInstallStep : IPipelineStep<Context>
    {
        public PipelineStepStatus DoStep(Context context, IPipelineStepData stepData, IWriter writer)
        {
            return PipelineStepStatus.Complete;
        }
    }
}