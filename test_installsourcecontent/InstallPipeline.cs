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

        public IWriter Writer { get { return _context.Writer; } }

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

    public interface IPipelineStepLogger
    {
        ReadOnlyCollection<string> Warnings { get; }
        ReadOnlyCollection<string> Errors { get; }

        void LogInfo(string message);
        void LogWarning(string message);
        void LogError(string message);

        void Clear();
    }

    public class PipelineStepLogger : IPipelineStepLogger
    {
        public readonly List<string> _warnings = new();
        public readonly List<string> _errors = new();
        IWriter _writer;

        public ReadOnlyCollection<string> Warnings { get { return new ReadOnlyCollection<string>(_warnings); } }
        public ReadOnlyCollection<string> Errors { get { return new ReadOnlyCollection<string>(_errors); } }

        public PipelineStepLogger (IWriter writer)
        {
            _writer = writer;
        }

        public void LogInfo(string message)
        {
            _writer.WriteLine(message);
        }

        public void LogWarning(string message)
        {
            _writer.WriteLine(message);
            _warnings.Add(message);
        }

        public void LogError(string message)
        {
            _writer.WriteLine(message);
            _errors.Add(message);
        }

        public void Clear()
        {
            _warnings.Clear();
            _errors.Clear();
        }
    }

    public interface IPipelineStepData
    {
        string Name { get; set; }
        string Description { get; set; }
    }

    public interface IPipelineStep
    {
        PipelineStepStatus DoStep(StepContext context, IPipelineStepData stepData, IPipelineStepLogger stepLogger);
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
        IWriter _writer;
        IConfiguration _configuration;
        IPauseHandler _pauseHandler;
        IContextFactory _contextFactory;
        InstallPipelineStepDictionary _appsSteps = new();
        List<int> _steamAppsToInstall = new();

        public InstallPipeline(IFileSystem fileSystem, IWriter writer, IConfiguration configuration, IPauseHandler pauseHandler, IContextFactory contextFactory) 
        {
            _fileSystem = fileSystem;
            _writer = writer;
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

        public class PipelineStepLogResult
        {
            public string StepName { get; set; }
            public List<string> Warnings { get; set; }
            public List<string> Errors { get; set; }
        }

        public void ExecuteSteps()
        {
            ITokenReplacer tokenReplacer = new TokenReplacer();
            tokenReplacer.Prefix = "$(";
            tokenReplacer.Suffix = ")";

            var context = _contextFactory.CreateContext();
            var stepContext = _contextFactory.CreateStepContext(context);
            var stepLogger = new PipelineStepLogger(_writer);
            var stepsLogsResults = new List<PipelineStepLogResult>();

            // Select only the steps we want to install.
            var appsInstallSteps = _appsSteps.Where(kv => _steamAppsToInstall.Contains(kv.Key));

            foreach (var (appID, installSteps) in appsInstallSteps)
            {
                // Setup context.
                context.AppID = appID;
                context.ContextVariables.Clear();
                context.ContextVariables["install_settings_install_dir"] = PathExtensions.ConvertToUnixDirectorySeparator(_fileSystem, _fileSystem.Path.GetFullPath(_configuration.GetContentInstallDir(appID)));

                context.ContextVariables["variables_config_file_name"] = _configuration.GetVariablesFileName();

                _writer.WriteLine($"Installing [{appID}] {_configuration.GetSteamAppName(appID)}");
                _writer.WriteLine();
                foreach (var stepData in installSteps)
                {
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

                    // Clear step logger warnings and errors.
                    stepLogger.Clear();

                    // Execute step.
                    var step = _stepsDataToInstallStep[stepData.GetType()];
                    PipelineStepStatus stepStatus = step.DoStep(stepContext, stepData, stepLogger);
                    switch (stepStatus)
                    {
                        case PipelineStepStatus.Complete:
                            _writer.WriteLine($"Step completed.");
                            break;
                        case PipelineStepStatus.PartiallyComplete:
                            _writer.WriteLine($"Step partially completed.");
                            break;
                        case PipelineStepStatus.Failed:
                            _writer.WriteLine($"Step failed.");
                            break;
                        case PipelineStepStatus.Cancelled:
                            _writer.WriteLine($"Step cancelled.");
                            break;
                    }

                    // Save warnings and errors raised by this step.
                    stepsLogsResults.Add(new PipelineStepLogResult
                    {
                        StepName = stepData.Name,
                        Warnings = stepLogger.Warnings.ToList(),
                        Errors = stepLogger.Errors.ToList()
                    });
                }
                _writer.WriteLine();

                if (PauseAfterEachStep)
                    _pauseHandler.Pause();
            }

            // Write all warnings and errors raised by steps.
            foreach (var stepLogResult in stepsLogsResults)
            {
                // Only write if we have either warnings or errors.
                if (stepLogResult.Warnings.Count > 0 || stepLogResult.Errors.Count > 0)
                {
                    _writer.WriteLine($"{stepLogResult.StepName}");
                    foreach (var warning in stepLogResult.Warnings)
                        _writer.WriteLine($"[WARNING] {warning}");
                    foreach (var error in stepLogResult.Errors)
                        _writer.WriteLine($"[ERROR] {error}");
                }
            }
        }
    }
}