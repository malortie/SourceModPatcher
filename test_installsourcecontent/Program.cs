using CommandLine;
using System.IO.Abstractions;

namespace test_installsourcecontent
{
    internal class Program
    {
        const string INSTALL_ENVVAR = "TEST_INSTALLSOURCECONTENT";
        const string STEAMAPPS_CONFIG_FILENAME ="steamapps.json";
        const string INSTALL_SETTINGS_FILENAME ="install.settings.json";

        public class Options
        {
            [Option('c', "use-config-file", Required = false, Default = false, HelpText = $"Read install path from {STEAMAPPS_CONFIG_FILENAME} file.")]
            public bool UseConfigFile { get; set; }
            [Option('y', "confirm-install", Required = false, Default = false, HelpText = "Prompt to confirm installation.")]
            public bool ConfirmInstallationPrompt { get; set; }
            [Option('p', "pause-after-step", Required = false, Default = false, HelpText = "Pause after each step.")]
            public bool PauseAfterEachStep { get; set; }
        }

        static void Main(string[] args)
        {
            var options = Parser.Default.ParseArguments<Options>(args).Value;
            var writer = new ConsoleWriter();

            try
            {
                if (null == Environment.GetEnvironmentVariable(INSTALL_ENVVAR, EnvironmentVariableTarget.User)) 
                {
                    // Create a user environment variable to allow locating this application's directory.
                    writer.WriteLine($"Creating user environment variable {INSTALL_ENVVAR}=\"{Environment.CurrentDirectory}\"");
                    Environment.SetEnvironmentVariable(INSTALL_ENVVAR, Environment.CurrentDirectory, EnvironmentVariableTarget.User);
                }

                var fileSystem = new FileSystem();

                Func<string, string> MakeFullPath = x => PathExtensions.JoinWithSeparator(fileSystem, Environment.CurrentDirectory, x);

                var steamAppsConfig = new SteamAppsConfig(fileSystem, writer, MakeFullPath(STEAMAPPS_CONFIG_FILENAME), new JSONConfigurationSerializer<JSONSteamAppsConfig>());
                steamAppsConfig.UseConfigFile = options.UseConfigFile;
                steamAppsConfig.LoadConfig();

                var variablesConfig = new VariablesConfig(fileSystem, writer, MakeFullPath("variables.json"), new JSONConfigurationSerializer<JSONVariablesConfig>());
                variablesConfig.LoadConfig();

                var installStepsConfig = new InstallStepsConfig(fileSystem, writer, MakeFullPath("install.steps.json"), new JSONConfigurationSerializer<JSONInstallStepsConfig>());
                installStepsConfig.LoadConfig();

                var installSettings = new InstallSettings(fileSystem, writer, MakeFullPath(INSTALL_SETTINGS_FILENAME), new JSONConfigurationSerializer<JSONInstallSettings>());
                installSettings.LoadConfig();

                writer.WriteLine();

                writer.WriteLine("Installed Source games:");
                foreach (var appID in steamAppsConfig.SupportedSourceGamesAppIDs)
                {
                    if (steamAppsConfig.IsSteamAppInstalled(appID))
                        writer.WriteLine($"\t[*] [{appID}] {steamAppsConfig.GetSteamAppName(appID)}");
                    else
                        writer.WriteLine($"\t[ ] [{appID}] {steamAppsConfig.GetSteamAppName(appID)}");
                }
                writer.WriteLine();

                writer.WriteLine("Content marked for installation:");
                foreach (var appID in steamAppsConfig.SupportedSourceGamesAppIDs)
                {
                    if (installSettings.IsSteamAppMarkedForInstall(appID))
                        writer.WriteLine($"\t[*] [{appID}] {steamAppsConfig.GetSteamAppName(appID)}");
                    else
                        writer.WriteLine($"\t[ ] [{appID}] {steamAppsConfig.GetSteamAppName(appID)}");
                }
                writer.WriteLine();

                var installedSteamApps = steamAppsConfig.GetInstalledSteamApps();
                var steamAppsUserWantsToInstall = installSettings.GetSteamAppsToInstall();

                // Content that we can actually install.
                var steamAppsToInstall = installedSteamApps.Intersect(steamAppsUserWantsToInstall).ToList();
                if (0 == steamAppsToInstall.Count)
                {
                    writer.WriteLine("Program exited: No content can be installed.");
                    return;
                }

                writer.WriteLine("The following content will be installed:");
                foreach (var appID in steamAppsToInstall)
                    writer.WriteLine($"\t[{appID}] {steamAppsConfig.GetSteamAppName(appID)}");
                writer.WriteLine();

                if (options.ConfirmInstallationPrompt)
                {
                    // Ask the user to confirm if they want to install the content.
                    ConsoleKey answer;
                    do
                    {
                        writer.Write("Do you want to proceed [y/n] : ");
                        answer = Console.ReadKey(false).Key;
                        writer.WriteLine();
                    } while (answer != ConsoleKey.Y && answer != ConsoleKey.N);

                    writer.WriteLine();

                    if (answer == ConsoleKey.Y)
                    {
                        // User accepted content installation.
                        writer.WriteLine("Proceeding...");
                        writer.WriteLine();
                    }
                    else
                    {
                        // User declined content installation.
                        writer.WriteLine($"Edit {INSTALL_SETTINGS_FILENAME} and relaunch the program.");
                        return;
                    }
                }

                var progressWriter = new PipelineProgressWriter(writer);
                var logCollector = new PipelineLogCollector();
                var logger = new PipelineLogger(logCollector, writer);
                var stepLogger = new PipelineLogger(logCollector, writer);
                var logReportWriter = new PipelineLogReportWriter(logCollector, writer, new PipelineLogFormatter());
                var statsWriter = new PipelineStatsWriter(writer);

                var configuration = new Configuration(steamAppsConfig, installSettings, variablesConfig);
                var pauseHandler = new ConsolePauseHandler(writer);
                var contextFactory = new ContextFactory(fileSystem, configuration);
                var installPipeline = new InstallPipeline(fileSystem, logger, configuration, pauseHandler, contextFactory);
                installPipeline.SetupFromConfig(installStepsConfig);
                installPipeline.StepLogger = stepLogger;
                installPipeline.ProgressWriter = progressWriter;

                installPipeline.SetSteamAppsToInstall(steamAppsToInstall);
                installPipeline.PauseAfterEachStep = options.PauseAfterEachStep;
                installPipeline.ExecuteSteps();

                statsWriter.WriteStats(installPipeline.StatsResults);
                writer.WriteLine("Installation finished.");

                logReportWriter.WriteInfos();
                logReportWriter.WriteWarnings();
                logReportWriter.WriteErrors();
                
                int o = 2;
                o++;
            }
            catch (Exception e)
            {
                writer.WriteLine(e.Message);
            }
        }
    }
}