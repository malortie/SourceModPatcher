using Microsoft.Win32;
using System.Runtime.Versioning;

namespace SourceModPatcher
{
    public interface ISourceModsPathFinder
    {
        string? GetSourceModsPath();
    }

    [SupportedOSPlatform("windows")]
    public class WindowsSourceModsPathFinder : ISourceModsPathFinder
    {
        public string? GetSourceModsPath()
        {
            string? result = null;

            using (RegistryKey? steamKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Valve\Steam", false))
            {
                result = steamKey?.GetValue("SourceModInstallPath") as string;
            }

            return result;
        }
    }
}