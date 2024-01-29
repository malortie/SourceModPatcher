using Pipelines;
using System.IO.Abstractions;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace SourceContentInstaller
{
    public class ExtractVPKInstallStepDataVPK
    {
        [PipelineStepReplaceToken]
        public string VPKFile { get; set; } = string.Empty;
        public List<string> FilesToExclude { get; set; } = [];
        public List<string> FilesToExtract { get; set; } = [];
    }

    public class ExtractVPKInstallStepData : IPipelineStepData
    {
        public string Name { get; set; } = string.Empty;
        [PipelineStepReplaceToken]
        public string Description { get; set; } = string.Empty;
        public List<string> DependsOn { get; set; } = [];
        [PipelineStepReplaceToken]
        public List<ExtractVPKInstallStepDataVPK> Vpks { get; set; } = [];
        public List<string> FilesToExclude { get; set; } = [];
        public List<string> FilesToExtract { get; set; } = [];
        [PipelineStepReplaceToken]
        public string OutDir { get; set; } = string.Empty;
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

    public interface IExtractVPKInstallStepEventHandler
    {
        void NoVPKsSpecified();
        void NoOutDirSpecified();
        void BlankVPKEntry();
        void FailedToBuildGlobalRegexList();
        void FailedToBuildVPKRegexList();
        void VPKFileDoesNotExist();
        void VPKExtractionComplete();
        void VPKExtractionCompleteWithErrors();
        void VPKExtractionFailed();
        void NoVPKExtracted();
    }

    public interface IStringToRegexConverter
    {
        Regex StringToRegex(string input);
    }

    public class StringToRegexConverter : IStringToRegexConverter
    {
        public Regex StringToRegex(string input)
        {
            return new Regex(input, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }
    }

    public interface IVPKFileResolver
    {
        void ResolveFilePaths(IFileSystem fileSystem, List<ExtractVPKInstallStepDataVPK> vpks);
    }

    public class VPKFileResolver : IVPKFileResolver
    {
        public Regex Wildcard = new Regex(@"[*]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public void ResolveFilePaths(IFileSystem fileSystem, List<ExtractVPKInstallStepDataVPK> vpks)
        {
            int index = 0;
            do
            {
                index = vpks.FindIndex(index, v => Wildcard.IsMatch(v.VPKFile));
                if (index >= 0)
                {
                    string vpkPath = vpks[index].VPKFile;
                    string? vpkDirectory = fileSystem.Path.GetDirectoryName(vpkPath);
                    string? vpkFileName = fileSystem.Path.GetFileName(vpkPath);
                    if (vpkDirectory != null && vpkFileName != null)
                    {
                        // backup properties before removing it.
                        var filesToExclude = vpks[index].FilesToExclude;
                        var filesToExtract = vpks[index].FilesToExtract;

                        var files = fileSystem.Directory.GetFiles(vpkDirectory, vpkFileName);
                        vpks.RemoveAt(index);
                        vpks.InsertRange(index, files.Select(f => new ExtractVPKInstallStepDataVPK
                        {
                            VPKFile = PathExtensions.ConvertToUnixDirectorySeparator(fileSystem, f),
                            FilesToExclude = filesToExclude,
                            FilesToExtract = filesToExtract
                        }));
                    }
                }
            } while (index >= 0);
        }
    }

    public class ExtractVPKInstallStep : IPipelineStep<Context>
    {
        IVPKExtractor _extractor;
        IStringToRegexConverter _stringToRegexConverter;
        IVPKFileResolver _fileResolver;
        IExtractVPKInstallStepEventHandler? _eventHandler;

        public ExtractVPKInstallStep(IVPKExtractor vpkExtractor, IStringToRegexConverter stringToRegexConverter, IVPKFileResolver fileResolver, IExtractVPKInstallStepEventHandler? eventHandler = null)
        {
            _extractor = vpkExtractor;
            _stringToRegexConverter = stringToRegexConverter;
            _fileResolver = fileResolver;
            _eventHandler = eventHandler;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        List<Regex> StringListToRegexList(List<string> strings)
        {
            return 0 == strings.Count ? new List<Regex>() : strings.Select(s => _stringToRegexConverter.StringToRegex(s)).ToList();
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
                _eventHandler?.NoVPKsSpecified();
                writer.Error("No vpk(s) specified.");
                return PipelineStepStatus.Failed;
            }
            else
            {
                // Check for empty VPK strings.
                List<int> emptyVPKsIndices = [];
                for (int i = 0; i < Vpks.Count; i++)
                {
                    if (Vpks[i].VPKFile == string.Empty)
                        emptyVPKsIndices.Add(i);
                }

                if (emptyVPKsIndices.Count > 0)
                {
                    _eventHandler?.BlankVPKEntry();
                    writer.Error($"VPK entries [{string.Join(',', emptyVPKsIndices)}] are blank or empty.");
                    return PipelineStepStatus.Failed;
                }
            }

            if (null == OutDir || OutDir == string.Empty)
            {
                _eventHandler?.NoOutDirSpecified();
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
                _eventHandler?.FailedToBuildGlobalRegexList();
                writer.Error(e.Message);
                return PipelineStepStatus.Failed;
            }

            var vpks = Vpks.Select(vpk => new ExtractVPKInstallStepDataVPK
            {
                VPKFile = PathExtensions.JoinWithSeparator(context.FileSystem, context.GetSteamAppInstallDir(), vpk.VPKFile),
                FilesToExclude = vpk.FilesToExclude,
                FilesToExtract = vpk.FilesToExtract
            }).ToList();

            // Find wildcards in VPKs and find the files.
            _fileResolver.ResolveFilePaths(context.FileSystem, vpks);

            int numExtractedVPKs = 0;
            PipelineStepStatus status = PipelineStepStatus.Complete;
            foreach (var vpk in vpks)
            {
                string vpkPath = vpk.VPKFile;
                if (!context.FileSystem.File.Exists(vpkPath))
                {
                    _eventHandler?.VPKFileDoesNotExist();
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
                    _eventHandler?.FailedToBuildVPKRegexList();
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
                        _eventHandler?.VPKExtractionComplete();
                        ++numExtractedVPKs;
                        break;
                    case VPKExtractionResult.CompleteWithErrors: // Not all files were extracted.
                        status = PipelineStepStatus.PartiallyComplete;
                        _eventHandler?.VPKExtractionCompleteWithErrors();
                        ++numExtractedVPKs;
                        break;
                    case VPKExtractionResult.Failed: // An error occured and extraction was aborted.
                        // Other VPKs might work, so mark it as partially completed.
                        status = PipelineStepStatus.PartiallyComplete;
                        _eventHandler?.VPKExtractionFailed();
                        break;
                }
            }

            // If no VPK has been extracted, mark it as failed.
            if (numExtractedVPKs == 0)
            {
                _eventHandler?.NoVPKExtracted();
                status = PipelineStepStatus.Failed;
            }

            return status;
        }
    }
}