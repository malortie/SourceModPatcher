namespace test_installsourcecontent
{
    public interface ITokenReplacerVariablesProvider<ContextT>
    {
        Dictionary<string, string> GetVariables(ContextT context);
    }
}
