using SteamDatabase.ValvePak;
using System.IO.Abstractions;

namespace test_installsourcecontent
{
    public interface IVPKExtractor
    {
        void Extract(IFileSystem fileSystem, IWriter writer, string vpkPath, string outputDir);
    }

    public class VPKExtractor : IVPKExtractor
    {
        public void Extract(IFileSystem fileSystem, IWriter writer, string vpkPath, string outputDir) 
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
                    string? entryDir = fileSystem.Path.GetDirectoryName(entry.GetFullPath());
                    if (null == entryDir)
                    {
                        writer.WriteLine($"Error: {entry.GetFullPath()}");
                        continue;
                    }
                    entryDir = PathExtensions.JoinWithSeparator(fileSystem, outputDir, entryDir.Replace(fileSystem.Path.DirectorySeparatorChar, fileSystem.Path.AltDirectorySeparatorChar));

                    if (!fileSystem.Directory.Exists(entryDir))
                        fileSystem.Directory.CreateDirectory(entryDir);

                    writer.WriteLine($"Extracting {entry.GetFullPath()}");
                    var fullPath = PathExtensions.JoinWithSeparator(fileSystem, outputDir, entry.GetFullPath());
                    package.ReadEntry(entry, out byte[] fileContents);
                    fileSystem.File.WriteAllBytes(fullPath, fileContents);
                }
            }

            fs?.Close();
        }
    }
}