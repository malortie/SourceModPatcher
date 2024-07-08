using Pipelines;
using SourceContentInstaller;

namespace SourceModPatcher
{
    public class ValidateVariablesDependenciesInstallStepData : IPipelineStepData
    {
        public string Name { get; set; } = string.Empty;
        [PipelineStepReplaceToken]
        public string Description { get; set; } = string.Empty;
        public List<string> DependsOn { get; set; } = [];
    }

    public interface IValidateVariablesDependenciesInstallStepEventHandler
    {
        void MissingSingleVariableRequiredDependency();
        void MissingMultiVariableRequiredDependency();

        void MissingSingleVariableOptionalDependency();
        void MissingMultiVariableOptionalDependency();
    }

    public class ValidateVariablesDependenciesInstallStep(IDependencyValidation dependencyValidation, IValidateVariablesDependenciesInstallStepEventHandler? eventHandler = null) : IPipelineStep<Context>
    {
        readonly IDependencyValidation _dependencyValidation = dependencyValidation;
        readonly IValidateVariablesDependenciesInstallStepEventHandler? _eventHandler = eventHandler;

        public PipelineStepStatus DoStep(Context context, IPipelineStepData stepData, IWriter writer)
        {
            var validateVariablesDependenciesStepData = (ValidateVariablesDependenciesInstallStepData)stepData;

            // Validate dependencies.
            var requiredDependenciesValidationResults = new List<DependencyValidationResult>();
            var optionalDependenciesValidationResults = new List<DependencyValidationResult>();

            PipelineStepStatus status = PipelineStepStatus.Complete;

            foreach (var dependency in context.GetRequiredContentDependencies())
            {
                requiredDependenciesValidationResults.Add(_dependencyValidation.Validate(context, dependency));
            }

            foreach (var dependency in context.GetOptionalContentDependencies())
            {
                optionalDependenciesValidationResults.Add(_dependencyValidation.Validate(context, dependency));
            }

            if (requiredDependenciesValidationResults.Any(d => !d.FulFilled))
            {
                status = PipelineStepStatus.Failed; // At least one required dependency wasn't fulfilled.

                // Write all unfulfilled required dependencies.
                foreach (var dependency in requiredDependenciesValidationResults.Where(d => !d.FulFilled).ToList())
                {
                    if (dependency.EquivalentContent.Count == 1)
                    {
                        _eventHandler?.MissingSingleVariableRequiredDependency();
                        DependencyValidationContentEntry contentEntry = dependency.EquivalentContent.First();
                        writer.Error($"Missing variable(s) in {context.GetVariablesFileName()} : [{string.Join(',', contentEntry.MissingVariables)}]");
                        writer.Error($"Install content: {context.GetContentName(contentEntry.ContentID)}");
                    }
                    else
                    {
                        _eventHandler?.MissingMultiVariableRequiredDependency();
                        var allMissingVariables = dependency.EquivalentContent.Select(c => c.MissingVariables);
                        var allContentNames = dependency.EquivalentContent.Select(c => context.GetContentName(c.ContentID));

                        writer.Error($"Missing variable(s) in {context.GetVariablesFileName()} : [{string.Join(',', allMissingVariables)}]");
                        writer.Error($"Install either one of the following content: {string.Join(" OR ", allContentNames)}");
                    }
                }
            }

            if (optionalDependenciesValidationResults.Any(d => !d.FulFilled))
            {
                // Write all unfulfilled optional dependencies.
                foreach (var dependency in optionalDependenciesValidationResults.Where(d => !d.FulFilled).ToList())
                {
                    if (dependency.EquivalentContent.Count == 1)
                    {
                        _eventHandler?.MissingSingleVariableOptionalDependency();
                        DependencyValidationContentEntry contentEntry = dependency.EquivalentContent.First();
                        writer.Warning($"Missing variable(s) in {context.GetVariablesFileName()} : [{string.Join(',', contentEntry.MissingVariables)}]");
                        writer.Warning($"Optionally install content: {context.GetContentName(contentEntry.ContentID)}");
                    }
                    else
                    {
                        _eventHandler?.MissingMultiVariableOptionalDependency();
                        var allMissingVariables = dependency.EquivalentContent.Select(c => c.MissingVariables);
                        var allContentNames = dependency.EquivalentContent.Select(c => context.GetContentName(c.ContentID));

                        writer.Warning($"Missing variable(s) in {context.GetVariablesFileName()} : [{string.Join(',', allMissingVariables)}]");
                        writer.Warning($"Optionally install either one of the following content: {string.Join(" OR ", allContentNames)}");
                    }
                }
            }

            return status;
        }
    }
}