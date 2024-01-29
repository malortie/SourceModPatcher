using Pipelines;
using System.IO.Abstractions;

namespace SourceModPatcher
{
    public interface IFileCopier
    {
        bool CopyFile(IFileSystem fileSystem, IWriter writer, string sourceFilePath, string destFilePath);
    }

    public class FileCopier : IFileCopier
    {
        public bool CopyFile(IFileSystem fileSystem, IWriter writer, string sourceFilePath, string destFilePath)
        {
            try
            {
                string? destFileDir = fileSystem.Path.GetDirectoryName(destFilePath);
                if (null == destFileDir)
                {
                    writer.Error($"Failed to get directory name for {destFilePath}");
                    return false;
                }

                if (!fileSystem.Directory.Exists(destFileDir))
                    fileSystem.Directory.CreateDirectory(destFileDir);

                // Copy file to destination. Overwrite if it exists.
                fileSystem.File.Copy(sourceFilePath, destFilePath, true);

                if (!fileSystem.File.Exists(destFilePath))
                    return false;

                // File copied successfully.
                return true;
            }
            catch (Exception e)
            {
                writer.Error(e.Message);
                return false;
            }
        }
    }
}
