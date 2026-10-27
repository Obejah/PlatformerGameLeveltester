using System.Windows;

namespace PlatformerGame.Entities
{
    public class Ground : Platform
    {
        public Ground(int id, Vector position, Vector size) : base(id, position, size, false) { }
    }

    public class SolidPlatform : Platform
    {
        public SolidPlatform(int id, Vector position, Vector size) : base(id, position, size, false) { }
    }

    public class OneWayPlatform : Platform
    {
        public OneWayPlatform(int id, Vector position, Vector size) : base(id, position, size, true) { }
    }
}