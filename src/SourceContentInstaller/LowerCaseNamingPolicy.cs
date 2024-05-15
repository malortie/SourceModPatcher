using System.Text.Json;

namespace SourceContentInstaller
{
    public class LowerCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            return name.ToLower();
        }

        public static JsonNamingPolicy LowerCase { get; } = new LowerCaseNamingPolicy();
    }
}