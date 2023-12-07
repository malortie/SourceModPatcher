using Gameloop.Vdf;
using Microsoft.Win32;
using System.IO.Abstractions;
using System.Text.Json.Serialization;

namespace test_installsourcecontent
{
    public class JSONSteamAppsConfigEntry
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";
        [JsonPropertyName("appmanifest_file")]
        public string AppManifestFile { get; set; } = "";
        [JsonPropertyName("install_dir")]
        public string InstallDir { get; set; } = "";
    }

    public class JSONSteamAppsConfig : SortedDictionary<int, JSONSteamAppsConfigEntry>
    {
    }

    public class SteamDirectoryNotFoundException : Exception
    {
        public SteamDirectoryNotFoundException() : base() 
        {
        }
    }

    public class SteamAppsConfig : ConfigurationManager<JSONSteamAppsConfig>
    {
        internal class SteamAppManifest
        {
            public int AppID { get; set; } = 0;
            public int SteamLibraryFolderID { get; set; } = 0;
            public string FilePath { get; set; } = "";
        }

        const string LIBRARYFOLDERS_FILE = "libraryfolders.vdf";

        public List<int> SupportedSourceGamesAppIDs = new();

        public SteamAppsConfig(IFileSystem fileSystem, IWriter writer, string filePath, IConfigurationSerializer<JSONSteamAppsConfig> configSerializer) : base(fileSystem, writer, filePath, configSerializer)
        {
        }

        public bool UseConfigFile { get; set; }

        public string GetSteamAppName(int appID)
        {
            return Config[appID].Name;
        }

        public string GetSteamAppInstallDir(int appID)
        {
            return Config[appID].InstallDir;
        }

        public bool IsSteamAppInstalled(int appID)
        {
            return Config[appID].InstallDir != string.Empty;
        }

        public List<int> GetInstalledSteamApps()
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
            SupportedSourceGamesAppIDs = Config.Keys.ToList();
            SupportedSourceGamesAppIDs.Sort();
        }

        void TrySetupFromRegistry()
        {
            // Check if Steam is installed.
            // Set AppID to the one found in steamapps.json
            // Set Name to value from appmanifest_id.acf
            // Set InstallDir to the found directory using appmanifest

            string? steamPath = null;

            // Read SteamApps from registry.

            using (RegistryKey? steamKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Valve\Steam", false))
            {
                if (null == steamKey)
                {
                    throw new Exception("Failed to locate Steam directory.");
                }

                steamPath = steamKey.GetValue("SteamPath") as string;

                if (null == steamPath)
                {
                    throw new Exception("Failed to locate Steam directory.");
                }
            }

            string steamappsPath = PathExtensions.JoinWithSeparator(FileSystem, steamPath, "steamapps");
            string libraryFoldersVDFPath = PathExtensions.JoinWithSeparator(FileSystem, steamappsPath, LIBRARYFOLDERS_FILE);

            /*
            if (!FileSystem.File.Exists(libraryFoldersVDFPath))
            {
                throw new FileNotFoundException($"{libraryFoldersVDFPath} does not exist.");
            }
            */

            // Collect appmanifest_id.acf files data.

            var steamLibraryPaths = new Dictionary<int, string>();
            var appManifestDictionary = new Dictionary<int, SteamAppManifest>();

            Writer.WriteLine($"Reading {libraryFoldersVDFPath}");
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
                Writer.WriteLine($"Reading {appManifestACFPath}");
                dynamic appManifestACF = VdfConvert.Deserialize(FileSystem.File.ReadAllText(appManifestACFPath));

                string name = (appManifestACF.Value["name"]).ToString();
                string installDir = (appManifestACF.Value["installdir"]).ToString();
                string gameInstallDir = PathExtensions.JoinWithSeparator(FileSystem, steamLibraryPaths[appManifest.SteamLibraryFolderID], "steamapps", "common", installDir);

                Config[appID].InstallDir = gameInstallDir;
            }
        }
    }
 }
