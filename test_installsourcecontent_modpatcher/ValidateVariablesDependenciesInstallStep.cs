using test_installsourcecontent;

namespace test_installsourcecontent_modpatcher
{
    public class ValidateVariablesDependenciesInstallStepData : IPipelineStepData
    {
        public string Name { get; set; } = string.Empty;
        [PipelineStepReplaceToken]
        public string Description { get; set; } = string.Empty;
        public List<string> DependsOn { get; set; } = [];
        public List<List<string>> Dependencies { get; set; } = [];
    }

    public class ValidateVariablesDependenciesInstallStep : IPipelineStep<Context>
    {
        public PipelineStepStatus DoStep(Context context, IPipelineStepData stepData, IWriter writer)
        {
            return PipelineStepStatus.Complete;
        }
    }
}