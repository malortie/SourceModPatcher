using CommandLine;
using SteamDatabase.ValvePak;
using System.IO.Abstractions;

namespace test_installsourcecontent
{
    public interface IVPKFileFilter
    {
        bool PassesFilter(string vpkFile);
    }

    public enum VPKExtractionResult
    {
        Complete = 0,
        CompleteWithErrors,
        Failed
    }

    public interface IVPKExtractor
    {
        VPKExtractionResult Extract(IFileSystem fileSystem, IWriter writer, string vpkPath, string outputDir, IVPKFileFilter fileFilter);
    }

    public class VPKExtractor : IVPKExtractor
    {
        public VPKExtractionResult Extract(IFileSystem fileSystem, IWriter writer, string vpkPath, string outputDir, IVPKFileFilter fileFilter) 
        {
            VPKExtractionResult result = VPKExtractionResult.Complete;

            try
            {
                if (!fileSystem.Directory.Exists(outputDir))
                    fileSystem.Directory.CreateDirectory(outputDir);

                using var package = new Package();

                var fs = fileSystem.FileStream.New(vpkPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                package.SetFileName(vpkPath);
                package.Read(fs);

                package.VerifyHashes();

                foreach (var extension in package.Entries.Values)
                {
                    foreach (var entry in extension)
                    {
                        // Only allow entries that pass the filter.
                        if (!fileFilter.PassesFilter(entry.GetFullPath()))
                            continue;

                        string? entryDir = fileSystem.Path.GetDirectoryName(entry.GetFullPath());
                        if (null == entryDir)
                        {
                            writer.Error($"Error: {entry.GetFullPath()}");
                            result = VPKExtractionResult.CompleteWithErrors;
                            continue;
                        }
                        entryDir = PathExtensions.JoinWithSeparator(fileSystem, outputDir, entryDir.Replace(fileSystem.Path.DirectorySeparatorChar, fileSystem.Path.AltDirectorySeparatorChar));

                        if (!fileSystem.Directory.Exists(entryDir))
                            fileSystem.Directory.CreateDirectory(entryDir);

                        writer.Info($"Extracting {entry.GetFullPath()}");
                        var fullPath = PathExtensions.JoinWithSeparator(fileSystem, outputDir, entry.GetFullPath());
                        package.ReadEntry(entry, out byte[] fileContents);
                        fileSystem.File.WriteAllBytes(fullPath, fileContents);
                    }
                }

                fs?.Close();
            }
            catch (Exception e)
            {
                writer.Error(e.Message);
                result = VPKExtractionResult.Failed;
            }

            return result;
        }
    }
}