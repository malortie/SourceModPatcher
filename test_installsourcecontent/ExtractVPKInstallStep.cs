using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace test_installsourcecontent
{
    public class ExtractVPKInstallStepDataVPK
    {
        [PipelineStepReplaceToken]
        public string VPKFile { get; set; } = "";
        public List<string> FilesToExclude { get; set; } = new();
        public List<string> FilesToExtract { get; set; } = new();
    }

    public class ExtractVPKInstallStepData : IPipelineStepData
    {
        public string Name { get; set; } = "";
        [PipelineStepReplaceToken]
        public string Description { get; set; } = "";
        public List<string> DependsOn { get; set; } = new();
        [PipelineStepReplaceToken]
        public List<ExtractVPKInstallStepDataVPK> Vpks { get; set; } = new();
        public List<string> FilesToExclude { get; set; } = new();
        public List<string> FilesToExtract { get; set; } = new();
        [PipelineStepReplaceToken]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static List<Regex> StringListToRegexList(List<string> strings)
        {
            return 0 == strings.Count ? new List<Regex>() : strings.Select(s => new Regex(s, RegexOptions.Compiled | RegexOptions.IgnoreCase)).ToList();
        }

        public PipelineStepStatus DoStep(Context context, IPipelineStepData stepData, IWriter writer)
        {
            var stepDataVPK = (ExtractVPKInstallStepData)stepData;
            var Vpks = stepDataVPK.Vpks;
            var OutDir = stepDataVPK.OutDir;
            var FilesToExclude = stepDataVPK.FilesToExclude;
            var FilesToExtract = stepDataVPK.FilesToExtract;

            if (null == Vpks || Vpks.Count <= 0)
            {
                writer.Error("No vpk(s) specified.");
                return PipelineStepStatus.Failed;
            }
            else
            {
                // Check for empty VPK strings.
                List<int> emptyVPKsIndices = new();
                for (int i = 0; i < Vpks.Count; i++)
                {
                    if (Vpks[i].VPKFile == string.Empty)
                        emptyVPKsIndices.Add(i);
                }

                if (emptyVPKsIndices.Count > 0)
                {
                    writer.Error($"VPK entries [{string.Join(',', emptyVPKsIndices)}] are blank or empty.");
                    return PipelineStepStatus.Failed;
                }
            }

            if (null == OutDir)
            {
                writer.Error("No output directory specified.");
                return PipelineStepStatus.Failed;
            }

            // Build compiled regex patterns for the global files filter rules.
            List<Regex> globalFilesToExcludeRegex, globalFilesToExtractRegex;
            try
            {
                globalFilesToExcludeRegex = StringListToRegexList(FilesToExclude);
                globalFilesToExtractRegex = StringListToRegexList(FilesToExtract);
            }
            catch (Exception e)
            {
                writer.Error(e.Message);
                return PipelineStepStatus.Failed;
            }

            var vpks = Vpks.Select(vpk => new ExtractVPKInstallStepDataVPK
            {
                VPKFile = PathExtensions.JoinWithSeparator(context.FileSystem, context.GetSteamAppInstallDir(), vpk.VPKFile),
                FilesToExclude = vpk.FilesToExclude,
                FilesToExtract = vpk.FilesToExtract
            }).ToList();

            ////////////////////////////
            // Find wildcards in VPKs and find the files.
            Regex wildcard = new Regex(@"[*]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            int index = 0;
            do
            {
                index = vpks.FindIndex(index, v => wildcard.IsMatch(v.VPKFile));
                if (index >= 0)
                {
                    string vpkPath = vpks[index].VPKFile;
                    string? vpkDirectory = context.FileSystem.Path.GetDirectoryName(vpkPath);
                    string? vpkFileName = context.FileSystem.Path.GetFileName(vpkPath);
                    if (vpkDirectory != null && vpkFileName != null)
                    {
                        // backup properties before removing it.
                        var filesToExclude = vpks[index].FilesToExclude;
                        var filesToExtract = vpks[index].FilesToExtract;

                        var files = context.FileSystem.Directory.GetFiles(vpkDirectory, vpkFileName);
                        vpks.RemoveAt(index);
                        vpks.InsertRange(index, files.Select(f => new ExtractVPKInstallStepDataVPK
                        {
                            VPKFile = PathExtensions.ConvertToUnixDirectorySeparator(context.FileSystem, f),
                            FilesToExclude = filesToExclude,
                            FilesToExtract = filesToExtract
                        }));
                    }
                }
            } while (index >= 0);
            ////////////////////////////

            int numExtractedVPKs = 0;
            PipelineStepStatus status = PipelineStepStatus.Complete;
            foreach (var vpk in vpks)
            {
                string vpkPath = vpk.VPKFile;
                if (!context.FileSystem.File.Exists(vpkPath))
                {
                    writer.Warning($"{vpkPath} does not exist. Skipping...");
                    status = PipelineStepStatus.PartiallyComplete;
                    continue;
                }

                IVPKFileFilter fileFilter;

                try
                {
                    // Build compiled regex patterns for files filter rules specific to this VPK.
                    // Create file filter.
                    fileFilter = new VPKFileFilter(
                       globalFilesToExcludeRegex.Concat(StringListToRegexList(vpk.FilesToExclude)).ToList(),
                       globalFilesToExtractRegex.Concat(StringListToRegexList(vpk.FilesToExtract)).ToList());
                }
                catch (Exception e)
                {
                    writer.Error(e.Message);
                    // Other VPKs might work, so mark it as partially completed.
                    status = PipelineStepStatus.PartiallyComplete;
                    continue;
                }

                writer.Info($"Extracting \"{vpkPath}\" to \"{OutDir}\"");
                switch (_extractor.Extract(context.FileSystem, writer, vpkPath, OutDir, fileFilter))
                {
                    case VPKExtractionResult.Complete:
                        // All files were correctly extracted.
                        ++numExtractedVPKs;
                        break;
                    case VPKExtractionResult.CompleteWithErrors: // Not all files were extracted.
                    case VPKExtractionResult.Failed: // An error occured and extraction was aborted.
                        // Other VPKs might work, so mark it as partially completed.
                        status = PipelineStepStatus.PartiallyComplete;
                        break;
                }
            }

            // If no VPK has been extracted, mark it as failed.
            if (numExtractedVPKs == 0)
                status = PipelineStepStatus.Failed;

            return status;
        }
    }
}