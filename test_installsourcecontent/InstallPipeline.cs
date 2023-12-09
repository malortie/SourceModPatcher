using AutoMapper;
using AutoMapper.Internal.Mappers;
using System.Collections.Immutable;
using System.IO.Abstractions;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace test_installsourcecontent
{
    using InstallPipelineStepDictionary = Dictionary<int, List<IPipelineStepData>>;

    // More restricted context for steps to disallow access to certain properties.
    public class StepContext
    {
        Context _context;

        public StepContext(Context context)
        {
            _context = context;
        }

        public IFileSystem FileSystem { get { return _context.FileSystem; } }

        public string GetContextVariable(string variable)
        {
            return _context.ContextVariables[variable];
        }

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

    public enum PipelineStepStatus
    {
        Complete = 0,
        PartiallyComplete = 1,
        Failed = 2,
        Cancelled = 3,
    }

    public interface IPipelineStepData
    {
        string Name { get; set; }
        string Description { get; set; }
        List<string> DependsOn { get; set; }
    }

    public interface IPipelineStep
    {
        PipelineStepStatus DoStep(StepContext context, IPipelineStepData stepData, IPipelineLogger logger);
    }

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

    public class InstallPipeline
    {
        // Map each step data type to a single step instance.
        Dictionary<Type, IPipelineStep> _stepsDataToInstallStep = new()
        {
            { typeof(ExtractVPKInstallStepData), new ExtractVPKInstallStep(new VPKExtractor()) },
            { typeof(SaveVariableInstallStepData), new SaveVariableInstallStep() }
        };

        IFileSystem _fileSystem;
        IPipelineLogger _logger;
        IConfiguration _configuration;
        IPauseHandler _pauseHandler;
        IContextFactory _contextFactory;
        InstallPipelineStepDictionary _appsSteps = new();
        List<int> _steamAppsToInstall = new();

        public IPipelineProgressWriter ProgressWriter { get; set; }
        public IPipelineLogger StepLogger { get; set; }
        public IPipelineStatsResults StatsResults { get; set; } = new PipelineStatsResults();

        public InstallPipeline(IFileSystem fileSystem, IPipelineLogger logger, IConfiguration configuration, IPauseHandler pauseHandler, IContextFactory contextFactory) 
        {
            _fileSystem = fileSystem;
            _logger = logger;
            _configuration = configuration;
            _pauseHandler = pauseHandler;
            _contextFactory = contextFactory;
        }

        public bool PauseAfterEachStep { get; set; }

        public void SetSteamAppsToInstall(List<int> steamAppsToInstall)
        {
            _steamAppsToInstall = steamAppsToInstall;
        }

        public void SetupFromConfig(InstallStepsConfig installStepsConfig)
        {
            _appsSteps = new InstallPipelineStepsMapper().Map(installStepsConfig);
        }

        public void ExecuteSteps()
        {
            ITokenReplacer tokenReplacer = new TokenReplacer();
            tokenReplacer.Prefix = "$(";
            tokenReplacer.Suffix = ")";

            var context = _contextFactory.CreateContext();
            var stepContext = _contextFactory.CreateStepContext(context);
            var progressContext = new PipelineProgressContext();

            HashSet<string> stepsComplete = new();

            // Select only the steps we want to install.
            var appsInstallSteps = _appsSteps.Where(kv => _steamAppsToInstall.Contains(kv.Key));

            StatsResults.NumStepsTotal = appsInstallSteps.Sum(a => a.Value.Count);
            StatsResults.NumStepsCompleted = 0;
            StatsResults.NumStepsPartiallyCompleted = 0;
            StatsResults.NumStepsFailed = 0;
            StatsResults.NumStepsCancelled = 0;

            foreach (var (appID, installSteps) in appsInstallSteps)
            {
                // Setup context.
                context.AppID = appID;
                context.ContextVariables.Clear();
                context.ContextVariables["install_settings_install_dir"] = PathExtensions.ConvertToUnixDirectorySeparator(_fileSystem, _fileSystem.Path.GetFullPath(_configuration.GetContentInstallDir(appID)));

                context.ContextVariables["variables_config_file_name"] = _configuration.GetVariablesFileName();

                _logger.LogInfo($"Installing [{appID}] {_configuration.GetSteamAppName(appID)}");

                for (int stepIndex = 0; stepIndex < installSteps.Count; ++stepIndex)
                {
                    var stepData = installSteps[stepIndex];

                    // Perform token replacement in step string properties.
                    tokenReplacer.Variables = new ReadOnlyDictionary<string, string>(context.ContextVariables);

                    foreach (PropertyInfo prop in stepData.GetType().GetProperties())
                    {
                        var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                        if (type == typeof(string))
                        {
                            string propertyValue = prop.GetValue(stepData) as string;
                            if (null != propertyValue)
                                prop.SetValue(stepData, tokenReplacer.Replace(propertyValue));
                        }
                    }

                    progressContext.StepNumber = stepIndex + 1;
                    progressContext.NumStepsTotal = installSteps.Count;

                    // Use the step name for the logger name.
                    StepLogger.Name = stepData.Name;

                    PipelineStepStatus stepStatus;

                    var uncompletedDependencies = stepData.DependsOn.Except(stepsComplete).ToArray();
                    if (uncompletedDependencies.Length > 0)
                    {
                        ProgressWriter.WriteStepDependenciesNotCompleted(progressContext, stepData);
                        stepStatus = PipelineStepStatus.Cancelled;
                    }
                    else
                    {
                        // Execute step.
                        ProgressWriter.WriteStepExecute(progressContext, stepData);
                        stepStatus = _stepsDataToInstallStep[stepData.GetType()].DoStep(stepContext, stepData, StepLogger);
                    }

                    switch (stepStatus)
                    {
                        case PipelineStepStatus.Complete:
                            ProgressWriter.WriteStepCompleted(progressContext, stepData);
                            ++StatsResults.NumStepsCompleted;
                            stepsComplete.Add(stepData.Name);
                            break;
                        case PipelineStepStatus.PartiallyComplete:
                            ProgressWriter.WriteStepPartiallyCompleted(progressContext, stepData);
                            ++StatsResults.NumStepsPartiallyCompleted;
                            break;
                        case PipelineStepStatus.Failed:
                            ProgressWriter.WriteStepFailed(progressContext, stepData);
                            ++StatsResults.NumStepsFailed;
                            break;
                        case PipelineStepStatus.Cancelled:
                            ProgressWriter.WriteStepCancelled(progressContext, stepData);
                            ++StatsResults.NumStepsCancelled;
                            break;
                    }
                }

                if (PauseAfterEachStep)
                    _pauseHandler.Pause();
            }
        }
    }
}