using System.Text.Json;

namespace SourceContentInstaller
{
    public interface IConfigurationSerializer<ConfigT>
    {
        string Serialize(ConfigT value);
        ConfigT? Deserialize(string value);
    }

    public class JSONConfigurationSerializer<ConfigT> : IConfigurationSerializer<ConfigT>
    {
        public string Serialize(ConfigT value)
        {
            return JsonSerializer.Serialize(value, _serializerOptions);
        }

        public ConfigT? Deserialize(string value)
        {
            return JsonSerializer.Deserialize<ConfigT>(value, _deserializerOptions);
        }

        readonly JsonSerializerOptions _serializerOptions = new()
        {
            DictionaryKeyPolicy = LowerCaseNamingPolicy.LowerCase,
            PropertyNamingPolicy = LowerCaseNamingPolicy.LowerCase,
            WriteIndented = true
        };

        readonly JsonSerializerOptions _deserializerOptions = new()
        {
            DictionaryKeyPolicy = LowerCaseNamingPolicy.LowerCase,
            PropertyNamingPolicy = LowerCaseNamingPolicy.LowerCase,
        };
    }
}