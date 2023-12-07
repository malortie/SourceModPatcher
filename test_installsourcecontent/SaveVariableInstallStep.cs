using System.Text.RegularExpressions;

namespace test_installsourcecontent
{
    public class SaveVariableInstallStepData : IPipelineStepData
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public List<string> DependsOn { get; set; } = new();
        public string VariableName { get; set; } = "";
        public string VariableValue { get; set; } = "";

    }

    public class SaveVariableInstallStep : IPipelineStep
    {
        public PipelineStepStatus DoStep(StepContext context, IPipelineStepData stepData, IPipelineStepLogger stepLogger)
        {
            var saveVariableData = (SaveVariableInstallStepData)stepData;
            var Name = saveVariableData.VariableName;
            var Value = saveVariableData.VariableValue;

            if (null == Name)
            {
                stepLogger.LogError("No variable name specified.");
                return PipelineStepStatus.Failed;
            }
            if (null == Value)
            {
                stepLogger.LogError("No variable value specified.");
                return PipelineStepStatus.Failed;
            }

            // Write the variable.
            stepLogger.LogInfo($"Writing variable {Name}=\"{Value}\"");
            context.SaveVariable(Name, Value);
            return PipelineStepStatus.Complete;
        }
    }
}