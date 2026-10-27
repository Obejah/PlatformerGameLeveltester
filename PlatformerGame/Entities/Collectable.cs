using PlatformerGame.Interfaces;
using PlatformerGame.PlayerCharacters;
using System.Windows;

namespace PlatformerGame.Entities
{
    public abstract class Collectable : Entity, ICollectable
    {
        public bool IsCollected { get; set; }

        protected Collectable(int id, Vector position, Vector size)
            : base(id, position, size)
        {
            IsCollected = false;
        }

        public abstract void Collect(Player p);
    }
}
