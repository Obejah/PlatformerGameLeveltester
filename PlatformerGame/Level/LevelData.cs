using System.Windows;
using System.Collections.Generic;

namespace PlatformerGame.Level
{
    public class LevelData
    {
        public string Name {  get; set; }
        public int TileSize { get; set; }
        public int Screens { get; set; }

        public Vector PlayerStart { get; set; }

        public List<EntityData> Entities {  get; set; } = new List<EntityData>();
        public int FossilCount {  get; set; }
    }
}
