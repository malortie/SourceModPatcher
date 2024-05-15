using Pipelines;

namespace SourceModPatcher
{
    public class ValidateVariablesDependenciesInstallStepData : IPipelineStepData
    {
        public string Name { get; set; } = string.Empty;
        [PipelineStepReplaceToken]
        public string Description { get; set; } = string.Empty;
        public List<string> DependsOn { get; set; } = [];
        public List<List<string>> Dependencies { get; set; } = [];
    }

    public interface IValidateVariablesDependenciesInstallStepEventHandler
    {
        void NoDependenciesSpecified();
        void MissingSingleVariableDependency();
        void MissingMultiVariableDependency();
        void MissingDependencies();
    }

    public class ValidateVariablesDependenciesInstallStep(IValidateVariablesDependenciesInstallStepEventHandler? eventHandler = null) : IPipelineStep<Context>
    {
        readonly IValidateVariablesDependenciesInstallStepEventHandler? _eventHandler = eventHandler;

        public PipelineStepStatus DoStep(Context context, IPipelineStepData stepData, IWriter writer)
        {
            var validateVariablesDependenciesStepData = (ValidateVariablesDependenciesInstallStepData)stepData;
            var Dependencies = validateVariablesDependenciesStepData.Dependencies;

            if (null == Dependencies || 0 == Dependencies.Count)
            {
                _eventHandler?.NoDependenciesSpecified();
                writer.Error("No dependencies specified.");
                return PipelineStepStatus.Failed;
            }

            var sourceContentVariablesNames = context.GetSourceContentVariables().Keys.ToList();

            int numMissingDependencies = 0;
            PipelineStepStatus status = PipelineStepStatus.Complete;
            foreach (var entry in Dependencies)
            {
                // Check that at least one of the variables defined in each entry is present.
                var fulfilledDependencies = entry.Intersect(sourceContentVariablesNames);
                if (!fulfilledDependencies.Any())
                {
                    if (entry.Count == 1)
                    {
                        _eventHandler?.MissingSingleVariableDependency();
                        writer.Error($"Missing variable in {context.GetVariablesFileName()} : {entry.First()}");
                    }
                    else
                    {
                        _eventHandler?.MissingMultiVariableDependency();
                        writer.Error($"Missing one of the following variables in {context.GetVariablesFileName()} : [{string.Join(" OR ", entry)}]");
                    }

                    ++numMissingDependencies;
                }
            }

            // If at least one dependency is missing, mark it as failed.
            if (numMissingDependencies > 0)
            {
                _eventHandler?.MissingDependencies();
                status = PipelineStepStatus.Failed;
            }

            return status;
        }
    }
}