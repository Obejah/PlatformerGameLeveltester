using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PlatformerGame.Level
{
    public static class LevelJsonExporter
    {
        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            WriteIndented = true, // Makes the JSON readable for debugging
            PropertyNameCaseInsensitive = true
        };

        public static string Serialize(LevelData data)
        {
            try
            {
                return JsonSerializer.Serialize(data, _options);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error serializing level: {ex.Message}");
                return string.Empty;
            }
        }

        public static LevelData Deserialize(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<LevelData>(json, _options);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deserializing level: {ex.Message}");
                return null;
            }
        }
    }
}