using System.Windows;

namespace PlatformerGame.Entities
{
    public class Spikes : Hazard
    {
        public Spikes(int id, Vector position, Vector size, int damage = 1)
            : base(id, position, size, damage)
        {
        }
    }
}