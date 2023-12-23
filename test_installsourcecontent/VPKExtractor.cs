using SteamDatabase.ValvePak;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Text.RegularExpressions;

namespace test_installsourcecontent
{
    public interface IVPKFileFilter
    {
        bool PassesFilter(string vpkFile);
    }

    public interface IVPKExtractor
    {
        void Extract(IFileSystem fileSystem, IWriter writer, string vpkPath, string outputDir, IVPKFileFilter fileFilter);
    }

    public class VPKExtractor : IVPKExtractor
    {
        public void Extract(IFileSystem fileSystem, IWriter writer, string vpkPath, string outputDir, IVPKFileFilter fileFilter) 
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
    }
}