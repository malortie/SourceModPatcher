using System.IO.Abstractions;
using System.Runtime.CompilerServices;

namespace test_installsourcecontent
{
    public static class PathExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string JoinWithSeparator(IFileSystem fileSystem, string path1, string path2) 
        {
            return fileSystem.Path.Join(path1, path2).Replace(fileSystem.Path.DirectorySeparatorChar, fileSystem.Path.AltDirectorySeparatorChar);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string JoinWithSeparator(IFileSystem fileSystem, params string?[] paths)
        {
            return fileSystem.Path.Join(paths).Replace(fileSystem.Path.DirectorySeparatorChar, fileSystem.Path.AltDirectorySeparatorChar);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ConvertToUnixDirectorySeparator(IFileSystem fileSystem, string path)
        {
            return path.Replace(fileSystem.Path.DirectorySeparatorChar, fileSystem.Path.AltDirectorySeparatorChar);
        }
    }
}