namespace Pipelines
{
    public interface ITokenReplacerVariablesProvider<ContextT>
    {
        Dictionary<string, string> GetVariables(ContextT context);
    }
}
