using System.Text.RegularExpressions;

namespace test_installsourcecontent
{
    public class ExtractVPKInstallStepData : IPipelineStepData
    {
        public List<string> Vpks { get; set; } = new();
        public string OutDir { get; set; } = "";
    }

    public class ExtractVPKInstallStep : IPipelineStep
    {
        IVPKExtractor _extractor;

        public ExtractVPKInstallStep(IVPKExtractor vpkExtractor) 
        {
            _extractor = vpkExtractor;
        }

        public PipelineStepStatus DoStep(StepContext context, IPipelineStepData stepData, IPipelineStepLogger stepLogger)
        {
            var stepDataVPK = (ExtractVPKInstallStepData)stepData;
            var Vpks = stepDataVPK.Vpks;
            var OutDir = stepDataVPK.OutDir;

            if (null == Vpks || Vpks.Count <= 0)
            {
                stepLogger.LogError("No vpk(s) specified.");
                return PipelineStepStatus.Failed;
            }
            if (null == OutDir)
            {
                stepLogger.LogError("No output directory specified.");
                return PipelineStepStatus.Failed;
            }

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
                    stepLogger.LogWarning($"{vpkPath} does not exist. Skipping...");
                    status = PipelineStepStatus.PartiallyComplete;
                    continue;
                }
                stepLogger.LogInfo($"Extracting \"{vpkPath}\" to \"{OutDir}\"");
                _extractor.Extract(context.FileSystem, context.Writer, vpkPath, OutDir);
            }

            return status;
        }
    }
}