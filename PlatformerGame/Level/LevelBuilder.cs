using System.Collections.Generic;
using System.Linq;
using System.Windows;
using PlatformerGame.Enums;

namespace PlatformerGame.Level
{
    public class LevelBuilder
    {
        private LevelData _data;
        private int _tileSize;
        private int _screenWidth;
        private int _nextEntityId;

        public LevelBuilder(int tileSize = 16, int screenWidth = 1920)
        {
            _tileSize = tileSize;
            _screenWidth = screenWidth;
            _nextEntityId = 1;
        }

        public void NewLevel(string name, int screens)
        {
            _data = new LevelData
            {
                Name = name,
                TileSize = _tileSize,
                Screens = screens,
                Entities = new List<EntityData>(),
                PlayerStart = new Vector(0, 0), // Default
                FossilCount = 0
            };
        }

        public void PlaceEntity(EntityType type, int x, int y)
        {
            if (_data == null) return;

            var entity = new EntityData
            {
                Id = _nextEntityId++,
                Type = type.ToString(),
                X = x,
                Y = y,
                Width = _tileSize,  // Default size, can be overridden
                Height = _tileSize
            };

            _data.Entities.Add(entity);
        }

        public void RemoveEntity(int id)
        {
            if (_data == null) return;
            _data.Entities.RemoveAll(e => e.Id == id);
        }

        public void SetPlayerStart(int x, int y)
        {
            if (_data == null) return;
            _data.PlayerStart = new Vector(x, y);
        }

        public void AddFossil(int x, int y)
        {
            PlaceEntity(EntityType.Fossil, x, y);
            if (_data != null) _data.FossilCount++;
        }

        public List<string> Validate()
        {
            var errors = new List<string>();

            if (_data == null)
            {
                errors.Add("Level data is null. Did you call NewLevel()?");
                return errors;
            }

            // Check for Player Start
            if (_data.PlayerStart == new Vector(0, 0) && !_data.Entities.Any(e => e.Type == EntityType.LevelStart.ToString()))
            {
                errors.Add("Warning: No player start position set, and no 'LevelStart' entity found.");
            }

            // Check for Level End
            if (!_data.Entities.Any(e => e.Type == EntityType.LevelEnd.ToString()))
            {
                errors.Add("Error: Level must have at least one 'LevelEnd' entity.");
            }

            return errors;
        }

        public LevelData Build()
        {
            var validationErrors = Validate();
            if (validationErrors.Any(e => e.StartsWith("Error")))
            {
                // In a real app, you might throw an exception or show a UI popup
                throw new System.Exception("Cannot build level: " + string.Join(", ", validationErrors));
            }
            return _data;
        }

        public void ExportJson(string filePath)
        {
            if (_data == null) return;
            string json = LevelJsonExporter.Serialize(_data);
            System.IO.File.WriteAllText(filePath, json);
        }
    }
}