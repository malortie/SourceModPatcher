using System.Text.RegularExpressions;

namespace test_installsourcecontent
{
    public class ExtractVPKInstallStepData : IPipelineStepData
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public List<string> DependsOn { get; set; } = new();
        public List<string> Vpks { get; set; } = new();
        public List<string> FilesToExclude { get; set; } = new();
        public List<string> FilesToExtract { get; set; } = new();
        public string OutDir { get; set; } = "";
    }

    public class VPKFileFilter : IVPKFileFilter
    {
        readonly List<Regex> _filesToExcludeRegex;
        readonly List<Regex> _filesToExtractRegex;

        public VPKFileFilter(List<Regex> filesToExcludeRegex, List<Regex> filesToExtractRegex)
        {
            _filesToExcludeRegex = filesToExcludeRegex;
            _filesToExtractRegex = filesToExtractRegex;
        }

        public bool PassesFilter(string vpkFile)
        {
            // If input matches one of the file exclusion pattern, ignore.
            if (_filesToExcludeRegex.Count > 0 && _filesToExcludeRegex.Any(r => r.IsMatch(vpkFile)))
                return false;

            // If input doesn't match any of the files to extract pattern, ignore.
            if (_filesToExtractRegex.Count > 0 && !_filesToExtractRegex.Any(r => r.IsMatch(vpkFile)))
                return false;

            return true;
        }
    }

    public class ExtractVPKInstallStep : IPipelineStep<Context>
    {
        IVPKExtractor _extractor;

        public ExtractVPKInstallStep(IVPKExtractor vpkExtractor) 
        {
            _extractor = vpkExtractor;
        }

        public PipelineStepStatus DoStep(Context context, IPipelineStepData stepData, IPipelineLogger logger)
        {
            var stepDataVPK = (ExtractVPKInstallStepData)stepData;
            var Vpks = stepDataVPK.Vpks;
            var OutDir = stepDataVPK.OutDir;
            var FilesToExclude = stepDataVPK.FilesToExclude;
            var FilesToExtract = stepDataVPK.FilesToExtract;

            if (null == Vpks || Vpks.Count <= 0)
            {
                logger.LogError("No vpk(s) specified.");
                return PipelineStepStatus.Failed;
            }
            if (null == OutDir)
            {
                logger.LogError("No output directory specified.");
                return PipelineStepStatus.Failed;
            }

            // Build compiled regex patterns.
            List<Regex> filesToExcludeRegex, filesToExtractRegex;

            try
            {
                filesToExcludeRegex = 0 == FilesToExclude.Count
                    ? new List<Regex>()
                    : FilesToExclude.Select(s => new Regex(s, RegexOptions.Compiled | RegexOptions.IgnoreCase)).ToList();

                filesToExtractRegex = 0 == FilesToExtract.Count
                    ? new List<Regex>()
                    : FilesToExtract.Select(s => new Regex(s, RegexOptions.Compiled | RegexOptions.IgnoreCase)).ToList();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return PipelineStepStatus.Failed;
            }

            // Create file filter.
            var fileFilter = new VPKFileFilter(filesToExcludeRegex, filesToExtractRegex);

            var vpks = Vpks.Select(vpkPath => PathExtensions.JoinWithSeparator(context.FileSystem, context.GetSteamAppInstallDir(), vpkPath)).ToList();

            ////////////////////////////
            // Find wildcards in VPKs and find the files.
            Regex wildcard = new Regex(@"[*]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            int index = 0;
            do
            {
                index = vpks.FindIndex(index, wildcard.IsMatch);
                if (index >= 0)
                {
                    string vpkPath = vpks[index];
                    string? vpkDirectory = context.FileSystem.Path.GetDirectoryName(vpkPath);
                    string? vpkFileName = context.FileSystem.Path.GetFileName(vpkPath);
                    if (vpkDirectory != null && vpkFileName != null)
                    {
                        var files = context.FileSystem.Directory.GetFiles(vpkDirectory, vpkFileName);
                        vpks.RemoveAt(index);
                        vpks.InsertRange(index, files.Select(s => PathExtensions.ConvertToUnixDirectorySeparator(context.FileSystem, s)));
                    }
                }
            } while (index >= 0);
            ////////////////////////////

            PipelineStepStatus status = PipelineStepStatus.Complete;
            foreach (var vpkPath in vpks)
            {
                if (!context.FileSystem.File.Exists(vpkPath))
                {
                    logger.LogWarning($"{vpkPath} does not exist. Skipping...");
                    status = PipelineStepStatus.PartiallyComplete;
                    continue;
                }
                logger.LogInfo($"Extracting \"{vpkPath}\" to \"{OutDir}\"");
                _extractor.Extract(context.FileSystem, logger, vpkPath, OutDir, fileFilter);
            }

            return status;
        }
    }
}