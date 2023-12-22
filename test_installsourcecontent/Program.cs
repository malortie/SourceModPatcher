using AutoMapper;
using CommandLine;
using System.Collections.ObjectModel;
using System.IO.Abstractions;
using System.Reflection;

namespace test_installsourcecontent
{
    using InstallPipelineStepDictionary = Dictionary<int, List<IPipelineStepData>>;

    public class InstallPipelineStepsTypeConverter : ITypeConverter<JSONInstallStepsConfig, InstallPipelineStepDictionary>
    {
        public InstallPipelineStepDictionary Convert(JSONInstallStepsConfig config, InstallPipelineStepDictionary destination, ResolutionContext context)
        {
            var convertedSteps = new InstallPipelineStepDictionary();

            // Convert each deserialized step to IPipelineStepData objects.
            foreach (var (appID, steps) in config)
            {
                convertedSteps[appID] = new();
                foreach (var step in steps)
                {
                    if (step != null)
                        convertedSteps[appID].Add(context.Mapper.Map<IPipelineStepData>(step));
                }
            }

            return convertedSteps;
        }
    }

    public class InstallPipelineStepsMapper
    {
        IMapper _mapper;

        public InstallPipelineStepsMapper()
        {
            var mapperConfig = new MapperConfiguration(cfg => {
                cfg.DisableConstructorMapping();
                cfg.ShouldMapField = fi => false;
                cfg.ShouldMapMethod = m => false;
                cfg.CreateMap<JSONInstallStep, IPipelineStepData>()
                    .Include<JSONExtractVPKInstallStep, ExtractVPKInstallStepData>()
                    .Include<JSONSaveVariableInstallStep, SaveVariableInstallStepData>();
                cfg.CreateMap<JSONExtractVPKInstallStep, ExtractVPKInstallStepData>();
                cfg.CreateMap<JSONExtractVPKInstallStepVPK, ExtractVPKInstallStepDataVPK>();
                cfg.CreateMap<JSONSaveVariableInstallStep, SaveVariableInstallStepData>();
                cfg.CreateMap<JSONInstallStepsConfig, InstallPipelineStepDictionary>().ConvertUsing(new InstallPipelineStepsTypeConverter());
            });
#if DEBUG
            mapperConfig.AssertConfigurationIsValid();
#endif
            mapperConfig.CompileMappings();
            _mapper = mapperConfig.CreateMapper();
        }

        public InstallPipelineStepDictionary Map(InstallStepsConfig installStepsConfig)
        {
            return _mapper.Map<InstallPipelineStepDictionary>(installStepsConfig.Config);
        }
    }

    // More restricted context for steps to disallow access to certain properties.
    public class StepContext
    {
        Context _context;

        public StepContext(Context context)
        {
            _context = context;
        }

        public IFileSystem FileSystem { get { return _context.FileSystem; } }

        public string GetSteamAppInstallDir()
        {
            return _context.GetSteamAppInstallDir();
        }

        public void SaveVariable(string name, string value)
        {
            _context.SaveVariable(name, value);
        }

        public string GetContentInstallDir()
        {
            return _context.GetContentInstallDir();
        }
    }

    public class InstallContentStage : PipelineStage<Context>
    {
        public int AppID { get; set; }

        // Map each step data type to a single step instance.
        static Dictionary<Type, IPipelineStep<Context>> _stepsDataToInstallStep = new()
        {
            { typeof(ExtractVPKInstallStepData), new ExtractVPKInstallStep(new VPKExtractor()) },
            { typeof(SaveVariableInstallStepData), new SaveVariableInstallStep() }
        };

        public override void SetupContext(Context context)
        {
            context.AppID = AppID;
        }

        public override void OnBeginStage(Context context)
        {
            Logger.LogInfo($"Installing [{AppID}] {context.GetSteamAppName()}");
        }

        public override PipelineStepStatus ExecuteStep(Context context, IPipelineStepData stepData)
        {
            return _stepsDataToInstallStep[stepData.GetType()].DoStep(context, stepData, StepLogger);
        }
    }

    public class TokenReplacerVariablesProvider : ITokenReplacerVariablesProvider<Context>
    {
        public Dictionary<string, string> GetVariables(Context context)
        {
            return new() {
              { "install_settings_install_dir", PathExtensions.ConvertToUnixDirectorySeparator(context.FileSystem, context.FileSystem.Path.GetFullPath(context.GetContentInstallDir())) },
              { "variables_config_file_name", context.GetVariablesFileName() },
              { "steamapp_name", context.GetSteamAppName() }
            };
        }
    }

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

                var progressWriter = new PipelineProgressWriter<Context>(writer);
                var logCollector = new PipelineLogCollector();
                var logger = new PipelineLogger(logCollector, writer);
                var stepLogger = new PipelineLogger(logCollector, writer);
                var logReportWriter = new PipelineLogReportWriter(logCollector, writer, new PipelineLogFormatter());
                var statsWriter = new PipelineStatsWriter(writer);
                var tokenReplacer = new TokenReplacer();
                tokenReplacer.Prefix = "$(";
                tokenReplacer.Suffix = ")";
                var tokenReplacerVariablesProvider = new TokenReplacerVariablesProvider();

                var configuration = new Configuration(steamAppsConfig, installSettings, variablesConfig);
                var pauseHandler = new ConsolePauseHandler(writer);
                var contextFactory = new ContextFactory(fileSystem, configuration);
                var context = contextFactory.CreateContext();

                // Select only the steps we want to install.
                var appsStepsDatas = new InstallPipelineStepsMapper().Map(installStepsConfig)
                    .Where(kv => steamAppsToInstall.Contains(kv.Key))
                    .ToList();

                int stageIndex = 0;
                // Create the stages.
                IPipelineStage<Context>[] stages = appsStepsDatas.Select(kv => new InstallContentStage() { 
                    Name = $"stage_{stageIndex++}",
                    Description = configuration.GetSteamAppName(kv.Key),
                    AppID = kv.Key,
                    Logger = logger,
                    PauseAfterEachStep = options.PauseAfterEachStep,
                    PauseHandler = pauseHandler,
                    ProgressWriter = progressWriter,
                    StepLogger = stepLogger,
                    StepsDatas = kv.Value.ToArray(),
                    TokenReplacer = tokenReplacer,
                    TokenReplacerVariablesProvider = tokenReplacerVariablesProvider
                }).ToArray();

                var pipeline = new Pipeline<Context>(stages, logger, progressWriter);
                pipeline.Execute(context);

                statsWriter.WriteStats(pipeline.StatsResults);

                writer.WriteLine("Installation finished.");

                if (pipeline.StatsResults.NumStagesPartiallyCompleted != 0 ||
                    pipeline.StatsResults.NumStagesFailed != 0 ||
                    pipeline.StatsResults.NumStagesCancelled != 0)
                {
                    writer.WriteLine("One or more errors occured.");
                }
                else
                {
                    writer.WriteLine("All steps successfully completed.");
                }

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