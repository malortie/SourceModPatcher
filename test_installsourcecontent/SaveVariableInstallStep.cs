using System.Text.RegularExpressions;

namespace test_installsourcecontent
{
    public class SaveVariableInstallStepData : IPipelineStepData
    {
        public string Name { get; set; } = "";
        public string Value { get; set; } = "";
    }

    public class SaveVariableInstallStep : IPipelineStep
    {
        public PipelineStepStatus DoStep(StepContext context, IPipelineStepData stepData, IPipelineStepLogger stepLogger)
        {
            var saveVariableData = (SaveVariableInstallStepData)stepData;
            var Name = saveVariableData.Name;
            var Value = saveVariableData.Value;

            if (null == Name)
            {
                stepLogger.LogError("No variable name specified.");
                return PipelineStepStatus.Cancelled;
            }
            if (null == Value)
            {
                stepLogger.LogError("No variable value specified.");
                return PipelineStepStatus.Cancelled;
            }

            // Write the variable.
            stepLogger.LogInfo($"Writing variable {Name}=\"{Value}\"");
            context.SaveVariable(Name, Value);
            return PipelineStepStatus.Complete;
        }
    }
}