using AutoMapper;
using CommandLine;
using NLog;
using NLog.Targets;
using Pipelines;
using System.IO.Abstractions;
using System.Linq;
using System.Runtime.Versioning;

namespace SourceContentInstaller
{
    public class InstallStepMapper<ConfigT> : IStepMapper<ConfigT>
    {
        readonly IMapper _mapper;

        public InstallStepMapper()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.DisableConstructorMapping();
                cfg.ShouldMapField = fi => false;
                cfg.ShouldMapMethod = m => false;
                cfg.CreateMap<JSONInstallStep, IPipelineStepData>()
                    .Include<JSONExtractVPKInstallStep, ExtractVPKInstallStepData>()
                    .Include<JSONSaveVariableInstallStep, SaveVariableInstallStepData>();
                cfg.CreateMap<JSONExtractVPKInstallStep, ExtractVPKInstallStepData>();
                cfg.CreateMap<JSONExtractVPKInstallStepVPK, ExtractVPKInstallStepDataVPK>();
                cfg.CreateMap<JSONSaveVariableInstallStep, SaveVariableInstallStepData>();
            });
#if DEBUG
            mapperConfig.AssertConfigurationIsValid();
#endif
            mapperConfig.CompileMappings();
            _mapper = mapperConfig.CreateMapper();
        }

        public IPipelineStepData Map(ConfigT jsonInstallStep)
        {
            return _mapper.Map<IPipelineStepData>(jsonInstallStep);
        }
    }

    public class InstallContentStage : PipelineStage<Context>
    {
        public string ContentID { get; set; } = string.Empty;

        // Map each step data type to a single step instance.
        static readonly Dictionary<Type, IPipelineStep<Context>> _stepsDataToInstallStep = new()
        {
            { typeof(ExtractVPKInstallStepData), new ExtractVPKInstallStep(new VPKExtractor(), new StringToRegexConverter(), new VPKFileResolver()) },
            { typeof(SaveVariableInstallStepData), new SaveVariableInstallStep() }
        };

        public override void SetupContext(Context context)
        {
            context.ContentID = ContentID;
        }

        public override IPipelineStep<Context> GetStepForStepData(IPipelineStepData stepData)
        {
            return _stepsDataToInstallStep[stepData.GetType()];
        }
    }

    public class TokenReplacerVariablesProvider : ITokenReplacerVariablesProvider<Context>
    {
        public Dictionary<string, string> GetVariables(Context context)
        {
            return new Dictionary<string, string>() {
              { "install_settings_install_dir", PathExtensions.ConvertToUnixDirectorySeparator(context.FileSystem, context.FileSystem.Path.GetFullPath(context.GetContentInstallDir())) },
              { "variables_config_file_name", context.GetVariablesFileName() },
              { "content_name", context.GetContentName() }
            }
            .Concat(context.GetSteamAppsInstallDirVariables())
            .ToDictionary();
        }
    }

    [SupportedOSPlatform("windows")]
    internal class Program
    {
        const string INSTALL_ENVVAR = "TEST_INSTALLSOURCECONTENT";
        const string VARIABLES_CONFIG_FILENAME = "variables.json";
        const string STEAMAPPS_CONFIG_FILENAME = "steamapps.json";
        const string CONTENTS_CONFIG_FILENAME = "contents.json";
        const string INSTALL_SETTINGS_FILENAME = "contents.install.settings.json";

        public class Options
        {
            [Option('c', "use-config-file", Required = false, Default = false, HelpText = $"Read install path from {STEAMAPPS_CONFIG_FILENAME} file.")]
            public bool UseConfigFile { get; set; }
            [Option('y', "no-confirm-install", Required = false, Default = false, HelpText = "Suppress prompt to confirm installation.")]
            public bool NoConfirmInstallationPrompt { get; set; }
            [Option('p', "pause-after-step", Required = false, Default = false, HelpText = "Pause after each step.")]
            public bool PauseAfterEachStep { get; set; }
        }

        static void Dispose()
        {
            NLog.LogManager.Shutdown();
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed(options =>
            {
                var fileSystem = new FileSystem();

                NLog.LogManager.Configuration = new NLog.Config.XmlLoggingConfiguration("nlog.config");

                // Clear existing log files.
                NLogServant.ClearAllLogFiles(NLog.LogManager.Configuration, fileSystem);

                var logProvider = new LogProvider(
                    (MemoryTarget)LogManager.Configuration.FindTargetByName("warningmemory"),
                    (MemoryTarget)LogManager.Configuration.FindTargetByName("errormemory"));
                var consoleWriter = new ConsoleWriter();
                var logger = new Logger(NLog.LogManager.GetCurrentClassLogger());
                var writer = new Writer(logger, consoleWriter);

                try
                {
#if false // Re-enable when Mod patcher is made obsolete.
                    if (null == Environment.GetEnvironmentVariable(INSTALL_ENVVAR, EnvironmentVariableTarget.User))
                    {
                        // Create a user environment variable to allow locating this application's directory.
                        writer.Info($"Creating user environment variable {INSTALL_ENVVAR}=\"{Environment.CurrentDirectory}\"");
                        Environment.SetEnvironmentVariable(INSTALL_ENVVAR, Environment.CurrentDirectory, EnvironmentVariableTarget.User);
                    }
#endif

                    string MakeFullPath(string x) => PathExtensions.JoinWithSeparator(fileSystem, Environment.CurrentDirectory, x);

                    var steamAppsConfig = new SteamAppsConfig(fileSystem, writer, MakeFullPath(STEAMAPPS_CONFIG_FILENAME), new JSONConfigurationSerializer<JSONSteamAppsConfig>(), new WindowsSteamPathFinder())
                    {
                        UseConfigFile = options.UseConfigFile
                    };
                    steamAppsConfig.LoadConfig();

                    var contentsConfig = new ContentsConfig(fileSystem, writer, MakeFullPath(CONTENTS_CONFIG_FILENAME), new JSONConfigurationSerializer<JSONContentsConfig>());
                    contentsConfig.LoadConfig();

                    var variablesConfig = new VariablesConfig(fileSystem, writer, MakeFullPath(VARIABLES_CONFIG_FILENAME), new JSONConfigurationSerializer<JSONVariablesConfig>());
                    variablesConfig.LoadConfig();

                    var installVariablesConfig = new InstallVariablesConfig(fileSystem, writer, MakeFullPath("sourcecontentinstaller.install.variables.json"), new JSONConfigurationSerializer<JSONInstallVariablesConfig>());
                    installVariablesConfig.LoadConfig();

                    // Add install variables to variables file.
                    variablesConfig.SaveVariables(installVariablesConfig.InstallVariables);

                    var installStepsConfig = new InstallStepsConfig(fileSystem, writer, MakeFullPath("contents.install.steps.json"), new JSONConfigurationSerializer<JSONInstallStepsConfig>());
                    installStepsConfig.LoadConfig();

                    var installSettings = new InstallSettings(fileSystem, writer, MakeFullPath(INSTALL_SETTINGS_FILENAME), new JSONConfigurationSerializer<JSONInstallSettings>());
                    installSettings.LoadConfig();

                    var contentSteamAppsDependencies = new ContentSteamAppsDependencies(steamAppsConfig, contentsConfig, variablesConfig);

                    writer.Info("Installed Source games:");
                    foreach (var appID in steamAppsConfig.SupportedSourceGamesAppIDs)
                    {
                        if (steamAppsConfig.IsSteamAppInstalled(appID))
                            writer.Info($"\t[*] [{appID}] {steamAppsConfig.GetSteamAppName(appID)}");
                        else
                            writer.Info($"\t[ ] [{appID}] {steamAppsConfig.GetSteamAppName(appID)}");
                    }

                    writer.Info("Content installed:");
                    foreach (var contentID in contentsConfig.SupportedContentIDs)
                    {
                        if (contentSteamAppsDependencies.IsContentInstalled(contentID))
                            writer.Info($"\t[*] [{contentID}] {contentsConfig.GetContentName(contentID)}");
                        else
                            writer.Info($"\t[ ] [{contentID}] {contentsConfig.GetContentName(contentID)}");
                    }

                    writer.Info("Content marked for installation:");
                    foreach (var contentID in contentsConfig.SupportedContentIDs)
                    {
                        if (installSettings.IsContentMarkedForInstall(contentID))
                            writer.Info($"\t[*] [{contentID}] {contentsConfig.GetContentName(contentID)}");
                        else
                            writer.Info($"\t[ ] [{contentID}] {contentsConfig.GetContentName(contentID)}");
                    }

                    var installedSteamApps = steamAppsConfig.GetInstalledSteamApps();
                    var contentsUserWantsToInstall = installSettings.GetContentsToInstall();

                    // Content that we can actually install.
                    var contentsToInstall = contentSteamAppsDependencies.GetInstallableContent()
                        .Intersect(contentsUserWantsToInstall).ToList();
                    if (0 == contentsToInstall.Count)
                    {
                        writer.Info("Program exited: No content can be installed.");
                        Dispose();
                        return;
                    }

                    writer.Info("The following content will be installed:");
                    foreach (var contentID in contentsToInstall)
                        writer.Info($"\t[{contentID}] {contentsConfig.GetContentName(contentID)}");

                    if (!options.NoConfirmInstallationPrompt)
                    {
                        // Ask the user to confirm if they want to install the content.
                        ConsoleKey answer;
                        do
                        {
                            writer.Info("Do you want to proceed [y/n] : ");
                            answer = Console.ReadKey(false).Key;
                        } while (answer != ConsoleKey.Y && answer != ConsoleKey.N);

                        if (answer == ConsoleKey.Y)
                        {
                            // User accepted content installation.
                            writer.Info("Proceeding...");
                        }
                        else
                        {
                            // User declined content installation.
                            writer.Info($"Edit {INSTALL_SETTINGS_FILENAME} and relaunch the program.");
                            Dispose();
                            return;
                        }
                    }

                    var progressWriter = new PipelineProgressWriter(writer);
                    var stageProgressWriter = new PipelineStageProgressWriter(writer);
                    var consoleLogReportWriter = new ConsoleLogReportWriter(consoleWriter, logProvider);
                    var statsWriter = new PipelineStatsWriter(writer);
                    var tokenReplacer = new TokenReplacer
                    {
                        Prefix = "$(",
                        Suffix = ")"
                    };
                    var tokenReplacerVariablesProvider = new TokenReplacerVariablesProvider();

                    var configuration = new Configuration(steamAppsConfig, contentsConfig, installSettings, variablesConfig);
                    var pauseHandler = new ConsolePauseHandler(writer);
                    var context = new Context(fileSystem, configuration);

                    // Load each steps file and convert them to pipeline step list. 
                    var stepsLoader = new StepsLoader<JSONInstallStep>(fileSystem, writer, new JSONConfigurationSerializer<IList<JSONInstallStep>>(), new InstallStepMapper<JSONInstallStep>());
                    var contentsStepsDatas = new Dictionary<string, IList<IPipelineStepData>>();
                    contentsToInstall.ForEach(contentID => contentsStepsDatas.Add(contentID, stepsLoader.Load(installStepsConfig.Config[contentID])));

                    int stageIndex = 0;
                    // Create the stages.
                    IPipelineStage<Context>[] stages = contentsStepsDatas.Select(kv => new InstallContentStage()
                    {
                        Name = $"stage_{stageIndex++}",
                        Description = configuration.GetContentName(kv.Key),
                        ContentID = kv.Key,
                        Writer = writer,
                        PauseAfterEachStep = options.PauseAfterEachStep,
                        PauseHandler = pauseHandler,
                        ProgressWriter = stageProgressWriter,
                        StepsDatas = [.. kv.Value],
                        TokenReplacer = tokenReplacer,
                        TokenReplacerVariablesProvider = tokenReplacerVariablesProvider
                    }).ToArray();

                    var pipeline = new Pipeline<Context>(stages, progressWriter);
                    pipeline.Execute(context);

                    statsWriter.WriteStats(pipeline.StatsResults);

                    writer.Info("Installation finished.");

                    if (pipeline.StatsResults.NumStagesPartiallyCompleted != 0 ||
                        pipeline.StatsResults.NumStagesFailed != 0 ||
                        pipeline.StatsResults.NumStagesCancelled != 0)
                    {
                        writer.Info("One or more errors occured.");
                    }
                    else
                    {
                        writer.Success("All steps successfully completed.");
                    }

                    consoleLogReportWriter.WriteWarnings();
                    consoleLogReportWriter.WriteErrors();
                }
                catch (Exception e)
                {
                    writer.Error(e.Message);
                }

                Dispose();
            });
        }
    }
}