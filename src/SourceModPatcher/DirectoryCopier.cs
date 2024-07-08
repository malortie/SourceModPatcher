using Pipelines;
using SourceContentInstaller;
using System.IO;
using System.IO.Abstractions;

namespace SourceModPatcher
{
    public enum DirectoryCopierFileCopyStatus
    {
        Ok = 0,
        Failed = 1,
    }

    public class DirectoryCopierFileCopyResult
    {
        public System.IO.Abstractions.IFileInfo? FileInfo { get; set; }
        public DirectoryCopierFileCopyStatus Status { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }

    public class DirectoryCopierResult
    {
        public List<DirectoryCopierFileCopyResult> FileCopyResults { get; set; } = [];
        public bool Success { get; set; }
    }

    public interface IDirectoryCopier
    {
        DirectoryCopierResult CopyDirectory(IFileSystem fileSystem, IWriter writer, string sourceDirectoryPath, string destDirectoryPath);
    }

    public class DirectoryCopier : IDirectoryCopier
    {
        public DirectoryCopierResult CopyDirectory(IFileSystem fileSystem, IWriter writer, string sourceDirectoryPath, string destDirectoryPath)
        {
            DirectoryCopierResult result = new() { Success = false };

            try
            {
                var sourceDirectoryInfo = fileSystem.DirectoryInfo.New(sourceDirectoryPath);
                var destDirectoryInfo = fileSystem.DirectoryInfo.New(destDirectoryPath);

                string sourceDirectoryPathFixed = PathExtensions.ConvertToUnixDirectorySeparator(fileSystem, fileSystem.Path.GetFullPath(sourceDirectoryPath));

                if (!sourceDirectoryInfo.Exists)
                {
                    writer.Error($"Source directory does not exist: {sourceDirectoryPath}");
                    return result;
                }

                if (!destDirectoryInfo.Exists)
                    fileSystem.Directory.CreateDirectory(destDirectoryPath);

                bool atLeastOneFileFailedToCopy = false;

                // Get the files in the source directory and copy to the destination directory
                foreach (var file in sourceDirectoryInfo.EnumerateFiles("*.*", SearchOption.AllDirectories))
                {
                    try
                    {
                        // Extract file path relative to the source folder.
                        string sourceFilePathRelative = PathExtensions.ConvertToUnixDirectorySeparator(fileSystem, file.FullName);
                        sourceFilePathRelative = sourceFilePathRelative.Replace(sourceDirectoryPathFixed + fileSystem.Path.AltDirectorySeparatorChar, "");

                        // Get the destination file full path.
                        string targetFilePath = PathExtensions.ConvertToUnixDirectorySeparator(fileSystem, fileSystem.Path.Combine(destDirectoryPath, sourceFilePathRelative));
                        
                        // Create the directory if it doesn't already exist.
                        string? destFileDir = fileSystem.Path.GetDirectoryName(targetFilePath);
                        if (!fileSystem.Directory.Exists(destFileDir))
                            fileSystem.Directory.CreateDirectory(destFileDir);

                        writer.Info($"Copying \"{file.FullName}\" to \"{targetFilePath}\"");
                        file.CopyTo(targetFilePath, true);
                        if (fileSystem.File.Exists(targetFilePath))
                        {
                            result.FileCopyResults.Add(new()
                            {
                                FileInfo = file,
                                Status = DirectoryCopierFileCopyStatus.Ok,
                                ErrorMessage = string.Empty
                            });
                        }
                        else
                        {
                            atLeastOneFileFailedToCopy = true;

                            string errorMessage = $"Failed to copy \"{file.FullName}\" to \"{targetFilePath}\"";
                            writer.Error(errorMessage);
                            result.FileCopyResults.Add(new()
                            {
                                FileInfo = file,
                                Status = DirectoryCopierFileCopyStatus.Failed,
                                ErrorMessage = errorMessage
                            });
                        }
                    }
                    catch (Exception e)
                    {
                        atLeastOneFileFailedToCopy = true;

                        writer.Error(e.Message);
                        result.FileCopyResults.Add(new()
                        {
                            FileInfo = file,
                            Status = DirectoryCopierFileCopyStatus.Failed,
                            ErrorMessage = e.Message
                        });
                    }
                }

                if (!atLeastOneFileFailedToCopy)
                    result.Success = true;
            }
            catch (Exception e)
            {
                writer.Error(e.Message);
            }

            return result;
        }
    }
}
