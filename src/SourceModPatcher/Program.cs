﻿using AutoMapper;
using CommandLine;
using NLog;
using NLog.Targets;
using Pipelines;
using SourceContentInstaller;
using System.IO.Abstractions;
using System.Runtime.Versioning;

namespace SourceModPatcher
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
                    .Include<JSONCopyFilesInstallStep, CopyFilesInstallStepData>()
                    .Include<JSONCopyDirectoryInstallStep, CopyDirectoryInstallStepData>()
                    .Include<JSONReplaceTokensInstallStep, ReplaceTokensInstallStepData>()
                    .Include<JSONValidateVariablesDependenciesInstallStep, ValidateVariablesDependenciesInstallStepData>();
                cfg.CreateMap<JSONCopyFilesInstallStep, CopyFilesInstallStepData>();
                cfg.CreateMap<JSONCopyDirectoryInstallStep, CopyDirectoryInstallStepData>();
                cfg.CreateMap<JSONCopyFilesInstallStepFile, CopyFilesInstallStepDataFile>();
                cfg.CreateMap<JSONReplaceTokensInstallStep, ReplaceTokensInstallStepData>();
                cfg.CreateMap<JSONValidateVariablesDependenciesInstallStep, ValidateVariablesDependenciesInstallStepData>();
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
        public string SourceModKey { get; set; } = string.Empty;

        // Map each step data type to a single step instance.
        static readonly Dictionary<Type, IPipelineStep<Context>> _stepsDataToInstallStep = new()
        {
            { typeof(CopyFilesInstallStepData), new CopyFilesInstallStep(new FileCopier()) },
            { typeof(CopyDirectoryInstallStepData), new CopyDirectoryInstallStep(new DirectoryCopier()) },
            { typeof(ReplaceTokensInstallStepData), new ReplaceTokensInstallStep(new FileTokenReplacer(new TokenReplacer { Prefix = "${{", Suffix = "}}" }))},
            { typeof(ValidateVariablesDependenciesInstallStepData), new ValidateVariablesDependenciesInstallStep(new DependencyValidation()) }
        };

        public override void SetupContext(Context context)
        {
            context.SourceModID = SourceModKey;
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
            return new Dictionary<string, string>
            {
                { "sourcemod_name", context.GetSourceModName() },
                { "sourcemod_folder", context.GetSourceModFolder() },
                { "sourcemod_dir", context.GetSourceModDir() },
                { "data_sourcemod_dir", context.GetSourceModDataDir() }
            };
        }
    }

    [SupportedOSPlatform("windows")]
    internal class Program
    {
        const string INSTALL_ENVVAR = "TEST_INSTALLSOURCECONTENT";
        const string VARIABLES_CONFIG_FILENAME = "variables.json";
        const string CONTENTS_CONFIG_FILENAME = "contents.json";
        const string COMMON_CONFIG_FILENAME = "sourcemods.common.json";
        const string SOURCEMODS_CONFIG_FILENAME = "sourcemods.json";
        const string INSTALL_SETTINGS_FILENAME = "sourcemods.install.settings.json";

        public class Options
        {
            [Option('c', "use-config-file", Required = false, Default = false, HelpText = $"Read install path from {COMMON_CONFIG_FILENAME} file.")]
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
                var logger = new SourceContentInstaller.Logger(NLog.LogManager.GetCurrentClassLogger());
                var writer = new Writer(logger, consoleWriter);

                try
                {
#if false // Re-enable when Mod patcher is made obsolete.
                    string? sourceContentInstallDirectory = Environment.GetEnvironmentVariable(INSTALL_ENVVAR, EnvironmentVariableTarget.User);
                    if (null == sourceContentInstallDirectory)
                    {
                        writer.Error($"No user environment variable {INSTALL_ENVVAR} exists. Exiting...");
                        Dispose();
                        return;
                    }
#else
                    string? sourceContentInstallDirectory = Environment.CurrentDirectory;
#endif

                    string MakeFullPath(string x) => PathExtensions.JoinWithSeparator(fileSystem, Environment.CurrentDirectory, x);

                    var contentsConfig = new ContentsConfig(fileSystem, writer, MakeFullPath(CONTENTS_CONFIG_FILENAME), new JSONConfigurationSerializer<JSONContentsConfig>());
                    contentsConfig.LoadConfig();

                    var variablesConfigFilePath = PathExtensions.JoinWithSeparator(fileSystem, sourceContentInstallDirectory, VARIABLES_CONFIG_FILENAME);
                    var variablesConfig = new VariablesConfig(fileSystem, writer, variablesConfigFilePath, new JSONConfigurationSerializer<JSONVariablesConfig>());
                    variablesConfig.LoadConfig();

                    var commonConfig = new CommonConfig(fileSystem, writer, MakeFullPath(COMMON_CONFIG_FILENAME), new JSONConfigurationSerializer<JSONCommonConfig>());
                    commonConfig.LoadConfig();

                    string? SourceModInstallPath;

                    if (options.UseConfigFile)
                        SourceModInstallPath = commonConfig.GetSourceModsPath();
                    else
                    {
                        var sourceModsPathFinder = new WindowsSourceModsPathFinder();
                        SourceModInstallPath = sourceModsPathFinder.GetSourceModsPath();
                    }

                    if (null == SourceModInstallPath)
                    {
                        writer.Error("Failed to locate Sourcemods directory. Exiting...");
                        Dispose();
                        return;
                    }

                    SourceModInstallPath = PathExtensions.ConvertToUnixDirectorySeparator(fileSystem, SourceModInstallPath);

                    var sourceModsConfig = new SourceModsConfig(fileSystem, writer, MakeFullPath(SOURCEMODS_CONFIG_FILENAME), new JSONConfigurationSerializer<JSONSourceModsConfig>(), SourceModInstallPath);
                    sourceModsConfig.LoadConfig();

                    var installStepsConfig = new InstallStepsConfig(fileSystem, writer, MakeFullPath("sourcemods.install.steps.json"), new JSONConfigurationSerializer<JSONInstallStepsConfig>());
                    installStepsConfig.LoadConfig();

                    var installSettings = new InstallSettings(fileSystem, writer, MakeFullPath(INSTALL_SETTINGS_FILENAME), new JSONConfigurationSerializer<JSONInstallSettings>());
                    installSettings.LoadConfig();

                    writer.Info("Installed Source mods:");
                    foreach (var key in sourceModsConfig.SupportedSourceModsKeys)
                    {
                        if (sourceModsConfig.IsSourceModInstalled(key))
                            writer.Info($"\t[*] [{key}] {sourceModsConfig.GetSourceModName(key)}");
                        else
                            writer.Info($"\t[ ] [{key}] {sourceModsConfig.GetSourceModName(key)}");
                    }

                    Dictionary<string, List<string>> folderToModKey = [];
                    writer.Info("Content marked for installation:");
                    foreach (var key in sourceModsConfig.SupportedSourceModsKeys)
                    {
                        if (installSettings.IsSourceModMarkedForInstall(key))
                        {
                            writer.Info($"\t[*] [{key}] {sourceModsConfig.GetSourceModName(key)}");

                            string folder = sourceModsConfig.GetSourceModFolder(key);
                            if (!folderToModKey.ContainsKey(folder))
                                folderToModKey[folder] = new();
                            folderToModKey[folder].Add(key);
                        }
                        else
                            writer.Info($"\t[ ] [{key}] {sourceModsConfig.GetSourceModName(key)}");
                    }

                    // Check if at least two mods have the same SourceMods folder.
                    var modsWithSameFolder = folderToModKey.Where(kv => kv.Value.Count > 1);
                    if (modsWithSameFolder.Any())
                    {
                        writer.Error($"The following mod(s) have the same SourceMods folder:");
                        writer.Error(string.Empty);
                        foreach (var (folder, modKeys) in modsWithSameFolder) // For each folder.
                        {
                            writer.Error($"Folder: {folder}");
                            writer.Error($"Mods:");
                            foreach (var key in modKeys) // For each mod key.
                                writer.Error($"\t[{key}] {sourceModsConfig.GetSourceModName(key)}");
                            writer.Error(string.Empty);
                        }

                        writer.Info($"Unable to determine which content to install to a specific SourceMods folder.");
                        writer.Info($"The installation will stop to prevent unintended overwriting.");
                        writer.Info($"Choose one mod.");
                        writer.Info($"Edit {INSTALL_SETTINGS_FILENAME} and relaunch the program.");
                        Dispose();
                        return;
                    }

                    var installedSourceMods = sourceModsConfig.GetInstalledSourceMods();
                    var sourceModsUserWantsToInstall = installSettings.GetSourceModsToInstall();

                    // Content that we can actually install.
                    var sourceModsToInstall = installedSourceMods.Intersect(sourceModsUserWantsToInstall).ToList();
                    if (0 == sourceModsToInstall.Count)
                    {
                        writer.Info("Program exited: No content can be installed.");
                        Dispose();
                        return;
                    }

                    writer.Info("The following content will be installed:");
                    foreach (var key in sourceModsToInstall)
                        writer.Info($"\t[{key}] {sourceModsConfig.GetSourceModName(key)}");

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

                    var configuration = new Configuration(sourceModsConfig, contentsConfig, variablesConfig);
                    var pauseHandler = new ConsolePauseHandler(writer);
                    var context = new Context(fileSystem, configuration);

                    // Load each steps file and convert them to pipeline step list. 
                    var stepsLoader = new StepsLoader<JSONInstallStep>(fileSystem, writer, new JSONConfigurationSerializer<IList<JSONInstallStep>>(), new InstallStepMapper<JSONInstallStep>());
                    var modsStepsDatas = new Dictionary<string, IList<IPipelineStepData>>();
                    sourceModsToInstall.ForEach(key => modsStepsDatas.Add(key, stepsLoader.Load(installStepsConfig.Config[key])));

                    int stageIndex = 0;
                    // Create the stages.
                    IPipelineStage<Context>[] stages = modsStepsDatas.Select(kv => new InstallContentStage()
                    {
                        Name = $"stage_{stageIndex++}",
                        Description = sourceModsConfig.GetSourceModName(kv.Key),
                        SourceModKey = kv.Key,
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
