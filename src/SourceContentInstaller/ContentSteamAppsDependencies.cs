namespace SourceContentInstaller
{
    public class ContentSteamAppsDependencies(SteamAppsConfig steamAppsConfig, ContentsConfig contentsConfig, VariablesConfig variablesConfig)
    {
        public bool AreAllContentSteamAppsDependenciesInstalled(string contentID)
        {
            var dependencies = contentsConfig.GetContentSteamAppsDependencies(contentID);
            return dependencies.Intersect(steamAppsConfig.GetInstalledSteamApps()).Count() == dependencies.Count;
        }

        public List<string> GetInstallableContent()
        {
            return contentsConfig.Config.Keys.Where(AreAllContentSteamAppsDependenciesInstalled).ToList();
        }

        public bool IsContentInstalled(string contentID)
        {
            return contentsConfig.GetContentOutputVariables(contentID).All(variablesConfig.IsVariablePresentAndValid);
        }
    }
}