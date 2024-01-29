using Pipelines;

namespace test_installsourcecontent
{
    public class SaveVariableInstallStepData : IPipelineStepData
    {
        public string Name { get; set; } = string.Empty;
        [PipelineStepReplaceToken]
        public string Description { get; set; } = string.Empty;
        public List<string> DependsOn { get; set; } = [];
        [PipelineStepReplaceToken]
        public string VariableName { get; set; } = string.Empty;
        [PipelineStepReplaceToken]
        public string VariableValue { get; set; } = string.Empty;

    }

    public interface ISaveVariableInstallStepEventHandler
    {
        void NoVariableNameSpecified();
        void NoVariableValueSpecified();
    }

    public class SaveVariableInstallStep : IPipelineStep<Context>
    {
        ISaveVariableInstallStepEventHandler? _eventHandler;

        public SaveVariableInstallStep(ISaveVariableInstallStepEventHandler? eventHandler = null)
        {
            _eventHandler = eventHandler;
        }

        public PipelineStepStatus DoStep(Context context, IPipelineStepData stepData, IWriter writer)
        {
            var saveVariableData = (SaveVariableInstallStepData)stepData;
            var Name = saveVariableData.VariableName;
            var Value = saveVariableData.VariableValue;

            if (null == Name || Name == string.Empty)
            {
                _eventHandler?.NoVariableNameSpecified();
                writer.Error("No variable name specified.");
                return PipelineStepStatus.Failed;
            }
            if (null == Value || Value == string.Empty)
            {
                _eventHandler?.NoVariableValueSpecified();
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