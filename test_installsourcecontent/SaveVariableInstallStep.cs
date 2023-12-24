namespace test_installsourcecontent
{
    public class SaveVariableInstallStepData : IPipelineStepData
    {
        public string Name { get; set; } = "";
        [PipelineStepReplaceToken]
        public string Description { get; set; } = "";
        public List<string> DependsOn { get; set; } = new();
        [PipelineStepReplaceToken]
        public string VariableName { get; set; } = "";
        [PipelineStepReplaceToken]
        public string VariableValue { get; set; } = "";

    }

    public class SaveVariableInstallStep : IPipelineStep<Context>
    {
        public PipelineStepStatus DoStep(Context context, IPipelineStepData stepData, IWriter writer)
        {
            var saveVariableData = (SaveVariableInstallStepData)stepData;
            var Name = saveVariableData.VariableName;
            var Value = saveVariableData.VariableValue;

            if (null == Name)
            {
                writer.Error("No variable name specified.");
                return PipelineStepStatus.Failed;
            }
            if (null == Value)
            {
                writer.Error("No variable value specified.");
                return PipelineStepStatus.Failed;
            }

            // Write the variable.
            writer.Info($"Writing variable {Name}=\"{Value}\"");
            context.SaveVariable(Name, Value);
            return PipelineStepStatus.Complete;
        }
    }
}