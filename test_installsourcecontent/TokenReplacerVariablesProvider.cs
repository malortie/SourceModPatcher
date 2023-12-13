namespace test_installsourcecontent
{
    public interface ITokenReplacerVariablesProvider
    {
        Dictionary<string, string> GetVariables(Context context);
    }

    public class TokenReplacerVariablesProvider : ITokenReplacerVariablesProvider
    {
        public Dictionary<string, string> GetVariables(Context context)
        {
            return new() {
              { "install_settings_install_dir", PathExtensions.ConvertToUnixDirectorySeparator(context.FileSystem, context.FileSystem.Path.GetFullPath(context.GetContentInstallDir())) },
              { "variables_config_file_name", context.GetVariablesFileName() }
            };
        }
    }
}
