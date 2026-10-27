using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PlatformerGame.Level
{
    public class EntityData
    {
        public int Id {  get; set; }

        public string Type {  get; set; }

        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
    }
}
