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

    public class SaveVariableInstallStep : IPipelineStep<Context>
    {
        public PipelineStepStatus DoStep(Context context, IPipelineStepData stepData, IPipelineLogger logger)
        {
            var saveVariableData = (SaveVariableInstallStepData)stepData;
            var Name = saveVariableData.VariableName;
            var Value = saveVariableData.VariableValue;

            if (null == Name)
            {
                logger.LogError("No variable name specified.");
                return PipelineStepStatus.Failed;
            }
            if (null == Value)
            {
                logger.LogError("No variable value specified.");
                return PipelineStepStatus.Failed;
            }

            // Write the variable.
            logger.LogInfo($"Writing variable {Name}=\"{Value}\"");
            context.SaveVariable(Name, Value);
            return PipelineStepStatus.Complete;
        }
    }
}