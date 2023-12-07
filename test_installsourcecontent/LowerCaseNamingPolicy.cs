using System.Text.Json;

namespace test_installsourcecontent
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