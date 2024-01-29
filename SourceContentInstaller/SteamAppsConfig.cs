using Gameloop.Vdf;
using Pipelines;
using System.IO.Abstractions;
using System.Text.Json.Serialization;

namespace SourceContentInstaller
{
    public class JSONSteamAppsConfigEntry
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("appmanifest_file")]
        public string AppManifestFile { get; set; } = string.Empty;
        [JsonPropertyName("install_dir")]
        public string InstallDir { get; set; } = string.Empty;
    }

    public class JSONSteamAppsConfig : SortedDictionary<int, JSONSteamAppsConfigEntry>
    {
    }

    public class SteamAppsConfig(IFileSystem fileSystem, IWriter writer, string filePath, IConfigurationSerializer<JSONSteamAppsConfig> configSerializer, ISteamPathFinder steamPathFinder) : ConfigurationManager<JSONSteamAppsConfig>(fileSystem, writer, filePath, configSerializer)
    {
        internal class SteamAppManifest
        {
            public int AppID { get; set; } = 0;
            public int SteamLibraryFolderID { get; set; } = 0;
            public string FilePath { get; set; } = string.Empty;
        }

        const string LIBRARYFOLDERS_FILE = "libraryfolders.vdf";

        public List<int> SupportedSourceGamesAppIDs = [];

        readonly ISteamPathFinder _steamPathFinder = steamPathFinder;

        public bool UseConfigFile { get; set; }

        public virtual string GetSteamAppName(int appID)
        {
            return Config[appID].Name;
        }

        public virtual string GetSteamAppInstallDir(int appID)
        {
            return Config[appID].InstallDir;
        }

        public virtual bool IsSteamAppInstalled(int appID)
        {
            return Config[appID].InstallDir != string.Empty;
        }

        public virtual List<int> GetInstalledSteamApps()
        {
            return Config.Where(kv => kv.Value.InstallDir != string.Empty).Select(kv => kv.Key).ToList();
        }

        protected override void PostLoadConfig()
        {
            SetupSupportedSourceGames();

            if (!UseConfigFile)
                TrySetupFromRegistry();
        }

        void SetupSupportedSourceGames()
        {
            SupportedSourceGamesAppIDs = [.. Config.Keys];
            SupportedSourceGamesAppIDs.Sort();
        }

        protected virtual void TrySetupFromRegistry()
        {
            // Check if Steam is installed.
            // Set AppID to the one found in steamapps.json
            // Set Name to value from appmanifest_id.acf
            // Set InstallDir to the found directory using appmanifest

            // Find SteamPath.
            string? steamPath = _steamPathFinder.GetSteamPath();
            if (null == steamPath)
                throw new Exception("Failed to locate Steam directory.");

            string steamappsPath = PathExtensions.JoinWithSeparator(FileSystem, steamPath, "steamapps");
            string libraryFoldersVDFPath = PathExtensions.JoinWithSeparator(FileSystem, steamappsPath, LIBRARYFOLDERS_FILE);

            // Collect appmanifest_id.acf files data.

            var steamLibraryPaths = new Dictionary<int, string>();
            var appManifestDictionary = new Dictionary<int, SteamAppManifest>();

            Writer.Info($"Reading {libraryFoldersVDFPath}");
            dynamic libraryFoldersVDF = VdfConvert.Deserialize(FileSystem.File.ReadAllText(libraryFoldersVDFPath));

            foreach (var libraryfolder in libraryFoldersVDF.Value)
            {
                string libraryFolderPath = (libraryfolder.Value["path"]).ToString();

                if (int.TryParse(libraryfolder.Key, out int folderID))
                {
                    steamLibraryPaths.Add(folderID, libraryFolderPath);

                    dynamic apps = libraryfolder.Value["apps"];
                    foreach (var app in apps)
                    {
                        if (int.TryParse(app.Key, out int appID) && SupportedSourceGamesAppIDs.Contains(appID))
                        {
                            string appManifestPath = PathExtensions.JoinWithSeparator(FileSystem, new[] {
                                libraryFolderPath,
                                "steamapps",
                                Config[appID].AppManifestFile
                            });

                            appManifestDictionary.Add(appID, new SteamAppManifest
                            {
                                AppID = appID,
                                SteamLibraryFolderID = folderID,
                                FilePath = appManifestPath
                            });
                        }
                    }
                }
            }

            foreach (var appManifestKV in appManifestDictionary)
            {
                var appManifest = appManifestKV.Value;

                int appID = appManifestKV.Key;
                string appManifestACFPath = appManifest.FilePath;
                Writer.Info($"Reading {appManifestACFPath}");
                dynamic appManifestACF = VdfConvert.Deserialize(FileSystem.File.ReadAllText(appManifestACFPath));

                string installDir = (appManifestACF.Value["installdir"]).ToString();
                string gameInstallDir = PathExtensions.JoinWithSeparator(FileSystem, steamLibraryPaths[appManifest.SteamLibraryFolderID], "steamapps", "common", installDir);

                Config[appID].InstallDir = gameInstallDir;
            }
        }
    }
}
