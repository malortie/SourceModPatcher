using Microsoft.Win32;
using System.Runtime.Versioning;

namespace SourceContentInstaller
{
    public interface ISteamPathFinder
    {
        string? GetSteamPath();
    }

    [SupportedOSPlatform("windows")]
    public class WindowsSteamPathFinder : ISteamPathFinder
    {
        public string? GetSteamPath()
        {
            string? result = null;

            using (RegistryKey? steamKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Valve\Steam", false))
            {
                result = steamKey?.GetValue("SteamPath") as string;
            }

            return result;
        }
    }
}
