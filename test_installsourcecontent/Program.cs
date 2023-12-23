using AutoMapper;
using CommandLine;
using Gameloop.Vdf.Linq;
using NLog;
using NLog.Targets;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Abstractions;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;

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
            return _stepsDataToInstallStep[stepData.GetType()].DoStep(context, stepData, Logger);
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

    public class Logger : ILogger
    {
        readonly NLog.Logger _logger;

        public Logger(NLog.Logger logger)
        {
            _logger = logger;
        }

        public void LogInfo(string message)
        {
            _logger.Info(message);
        }

        public void LogWarning(string message)
        {
            _logger.Warn(message);
        }

        public void LogError(string message)
        {
            _logger.Error(message);
        }
    }

    public class ConsoleLogWriter : IConsoleLogWriter
    {
        readonly NLog.Logger _writer;

        public ConsoleLogWriter(NLog.Logger writer)
        {
            _writer = writer;
        }

        public void WriteInfo(string message)
        {
            _writer.Info(message);
        }

        public void WriteWarning(string message)
        {
            _writer.Warn(message);
        }

        public void WriteError(string message)
        {
            _writer.Error(message);
        }
    }

    public class LogProvider : ILogProvider
    {
        readonly MemoryTarget _warningMemoryTarget, _errorMemoryTarget;
        public LogProvider(MemoryTarget warningMemoryTarget, MemoryTarget errorMemoryTarget)
        {
            _warningMemoryTarget = warningMemoryTarget;
            _errorMemoryTarget = errorMemoryTarget;
        }

        public ReadOnlyCollection<string> GetErrors()
        {
            return new ReadOnlyCollection<string>(_errorMemoryTarget.Logs);
        }

        public ReadOnlyCollection<string> GetInfos()
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<string> GetWarnings()
        {
            return new ReadOnlyCollection<string>(_warningMemoryTarget.Logs);
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

            NLog.LogManager.Configuration = new NLog.Config.XmlLoggingConfiguration("nlog.config");

            // Clear existing log files.
            foreach (var target in NLog.LogManager.Configuration.AllTargets.OfType<FileTarget>())
            {
                var dummyEventInfo = new LogEventInfo { TimeStamp = DateTime.UtcNow };
                var logFilePath = target.FileName.Render(dummyEventInfo);
                var fullLogFilePath = Path.GetFullPath(logFilePath);
                if (File.Exists(fullLogFilePath))
                    File.WriteAllText(fullLogFilePath, string.Empty);
            }

            var warningMemoryTarget = (MemoryTarget)LogManager.Configuration.FindTargetByName("warningmemory");
            var errorMemoryTarget = (MemoryTarget)LogManager.Configuration.FindTargetByName("errormemory");
            var warnings = warningMemoryTarget.Logs;
            var errors = errorMemoryTarget.Logs;

            var logProvider = new LogProvider(warningMemoryTarget, errorMemoryTarget);
            var logger = new Logger(NLog.LogManager.GetCurrentClassLogger());
            var consoleLogWriter = new ConsoleLogWriter(NLog.LogManager.GetLogger("console"));

            try
            {
                if (null == Environment.GetEnvironmentVariable(INSTALL_ENVVAR, EnvironmentVariableTarget.User)) 
                {
                    // Create a user environment variable to allow locating this application's directory.
                    logger.LogInfo($"Creating user environment variable {INSTALL_ENVVAR}=\"{Environment.CurrentDirectory}\"");
                    Environment.SetEnvironmentVariable(INSTALL_ENVVAR, Environment.CurrentDirectory, EnvironmentVariableTarget.User);
                }

                var fileSystem = new FileSystem();

                Func<string, string> MakeFullPath = x => PathExtensions.JoinWithSeparator(fileSystem, Environment.CurrentDirectory, x);

                var steamAppsConfig = new SteamAppsConfig(fileSystem, logger, MakeFullPath(STEAMAPPS_CONFIG_FILENAME), new JSONConfigurationSerializer<JSONSteamAppsConfig>());
                steamAppsConfig.UseConfigFile = options.UseConfigFile;
                steamAppsConfig.LoadConfig();

                var variablesConfig = new VariablesConfig(fileSystem, logger, MakeFullPath("variables.json"), new JSONConfigurationSerializer<JSONVariablesConfig>());
                variablesConfig.LoadConfig();

                var installStepsConfig = new InstallStepsConfig(fileSystem, logger, MakeFullPath("install.steps.json"), new JSONConfigurationSerializer<JSONInstallStepsConfig>());
                installStepsConfig.LoadConfig();

                var installSettings = new InstallSettings(fileSystem, logger, MakeFullPath(INSTALL_SETTINGS_FILENAME), new JSONConfigurationSerializer<JSONInstallSettings>());
                installSettings.LoadConfig();

                logger.LogInfo("Installed Source games:");
                foreach (var appID in steamAppsConfig.SupportedSourceGamesAppIDs)
                {
                    if (steamAppsConfig.IsSteamAppInstalled(appID))
                        logger.LogInfo($"\t[*] [{appID}] {steamAppsConfig.GetSteamAppName(appID)}");
                    else
                        logger.LogInfo($"\t[ ] [{appID}] {steamAppsConfig.GetSteamAppName(appID)}");
                }

                logger.LogInfo("Content marked for installation:");
                foreach (var appID in steamAppsConfig.SupportedSourceGamesAppIDs)
                {
                    if (installSettings.IsSteamAppMarkedForInstall(appID))
                        logger.LogInfo($"\t[*] [{appID}] {steamAppsConfig.GetSteamAppName(appID)}");
                    else
                        logger.LogInfo($"\t[ ] [{appID}] {steamAppsConfig.GetSteamAppName(appID)}");
                }

                var installedSteamApps = steamAppsConfig.GetInstalledSteamApps();
                var steamAppsUserWantsToInstall = installSettings.GetSteamAppsToInstall();

                // Content that we can actually install.
                var steamAppsToInstall = installedSteamApps.Intersect(steamAppsUserWantsToInstall).ToList();
                if (0 == steamAppsToInstall.Count)
                {
                    logger.LogInfo("Program exited: No content can be installed.");
                    return;
                }

                logger.LogInfo("The following content will be installed:");
                foreach (var appID in steamAppsToInstall)
                    logger.LogInfo($"\t[{appID}] {steamAppsConfig.GetSteamAppName(appID)}");

                if (options.ConfirmInstallationPrompt)
                {
                    // Ask the user to confirm if they want to install the content.
                    ConsoleKey answer;
                    do
                    {
                        logger.LogInfo("Do you want to proceed [y/n] : ");
                        answer = Console.ReadKey(false).Key;
                    } while (answer != ConsoleKey.Y && answer != ConsoleKey.N);

                    if (answer == ConsoleKey.Y)
                    {
                        // User accepted content installation.
                        logger.LogInfo("Proceeding...");
                    }
                    else
                    {
                        // User declined content installation.
                        logger.LogInfo($"Edit {INSTALL_SETTINGS_FILENAME} and relaunch the program.");
                        return;
                    }
                }

                var progressWriter = new PipelineProgressWriter<Context>(logger);
                var consoleLogReportWriter = new ConsoleLogReportWriter(consoleLogWriter, logProvider);
                var statsWriter = new PipelineStatsWriter(logger);
                var tokenReplacer = new TokenReplacer();
                tokenReplacer.Prefix = "$(";
                tokenReplacer.Suffix = ")";
                var tokenReplacerVariablesProvider = new TokenReplacerVariablesProvider();

                var configuration = new Configuration(steamAppsConfig, installSettings, variablesConfig);
                var pauseHandler = new ConsolePauseHandler(logger);
                var context = new Context(fileSystem, configuration);

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
                    StepsDatas = kv.Value.ToArray(),
                    TokenReplacer = tokenReplacer,
                    TokenReplacerVariablesProvider = tokenReplacerVariablesProvider
                }).ToArray();

                var pipeline = new Pipeline<Context>(stages, logger, progressWriter);
                pipeline.Execute(context);

                statsWriter.WriteStats(pipeline.StatsResults);

                logger.LogInfo("Installation finished.");
                
                if (pipeline.StatsResults.NumStagesPartiallyCompleted != 0 ||
                    pipeline.StatsResults.NumStagesFailed != 0 ||
                    pipeline.StatsResults.NumStagesCancelled != 0)
                {
                    logger.LogInfo("One or more errors occured.");
                }
                else
                {
                    logger.LogInfo("All steps successfully completed.");
                }

                consoleLogReportWriter.WriteWarnings();
                consoleLogReportWriter.WriteErrors();
                
                int o = 2;
                o++;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
            }

            NLog.LogManager.Shutdown();
        }
    }
}