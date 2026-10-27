using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;
using PlatformerGame.Enums;

namespace PlatformerGame.Level
{
    public static class LdtkImporter
    {
        // NEW METHOD: Get all level names to populate our progression list
        public static List<string> GetLevelIdentifiers(string filePath)
        {
            var levelNames = new List<string>();
            if (!File.Exists(filePath)) return levelNames;

            string jsonString = File.ReadAllText(filePath);
            using JsonDocument doc = JsonDocument.Parse(jsonString);

            if (doc.RootElement.TryGetProperty("levels", out JsonElement levelsArray))
            {
                foreach (var level in levelsArray.EnumerateArray())
                {
                    levelNames.Add(level.GetProperty("identifier").GetString());
                }
            }
            return levelNames;
        }

        public static LevelData Import(string filePath, string targetLevelIdentifier = null)
        {
            // ... (Keep the existing Import method exactly as it is) ...
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Could not find LDtk file at: {filePath}");

            string jsonString = File.ReadAllText(filePath);
            using JsonDocument doc = JsonDocument.Parse(jsonString);

            JsonElement root = doc.RootElement;
            JsonElement targetLevel = root;

            if (root.TryGetProperty("levels", out JsonElement levelsArray))
            {
                bool found = false;
                foreach (var level in levelsArray.EnumerateArray())
                {
                    if (targetLevelIdentifier == null || level.GetProperty("identifier").GetString() == targetLevelIdentifier)
                    {
                        targetLevel = level;
                        found = true;
                        break;
                    }
                }
                if (!found) throw new Exception($"Level '{targetLevelIdentifier}' not found in LDtk file.");
            }

            string name = targetLevel.GetProperty("identifier").GetString();
            int pxWid = targetLevel.GetProperty("pxWid").GetInt32();
            int pxHei = targetLevel.GetProperty("pxHei").GetInt32();

            var levelData = new LevelData
            {
                Name = name,
                TileSize = 16,
                Screens = Math.Max(1, (int)Math.Ceiling(pxWid / 1920.0)),
                PlayerStart = new Vector(0, 0)
            };

            if (targetLevel.TryGetProperty("layerInstances", out JsonElement layers))
            {
                foreach (var layer in layers.EnumerateArray())
                {
                    if (layer.GetProperty("__type").GetString() == "Entities")
                    {
                        if (layer.TryGetProperty("entityInstances", out JsonElement entities))
                        {
                            int idCounter = 1;
                            foreach (var ent in entities.EnumerateArray())
                            {
                                string ldtkType = ent.GetProperty("__identifier").GetString();
                                string cleanType = ldtkType.Replace("_", "").Replace(" ", "");

                                if (cleanType.Equals("PlayerStart", StringComparison.OrdinalIgnoreCase)) cleanType = "LevelStart";

                                if (!Enum.TryParse(cleanType, true, out EntityType entityType))
                                {
                                    Console.WriteLine($"[LDtk Importer] Skipping unknown entity type: {ldtkType}");
                                    continue;
                                }

                                var px = ent.GetProperty("px");
                                int x = px[0].GetInt32();
                                int y = px[1].GetInt32();
                                int width = ent.GetProperty("width").GetInt32();
                                int height = ent.GetProperty("height").GetInt32();

                                var entityData = new EntityData
                                {
                                    Id = idCounter++,
                                    Type = entityType.ToString(),
                                    X = x,
                                    Y = y,
                                    Width = width,
                                    Height = height
                                };

                                if (entityType == EntityType.LevelStart) levelData.PlayerStart = new Vector(x, y);

                                if (ent.TryGetProperty("fieldInstances", out JsonElement fields))
                                {
                                    foreach (var field in fields.EnumerateArray())
                                    {
                                        string fieldName = field.GetProperty("__identifier").GetString();
                                        var val = field.GetProperty("__value");
                                        string stringVal = val.ValueKind == JsonValueKind.Null ? "null" : val.ToString();
                                        entityData.Properties[fieldName] = stringVal;
                                    }
                                }

                                levelData.Entities.Add(entityData);
                                if (entityType == EntityType.Fossil) levelData.FossilCount++;
                            }
                        }
                    }
                }
            }
            return levelData;
        }
    }
}