using PlatformerGame.PlayerCharacters;
using System.Windows;

namespace PlatformerGame.Entities
{
    public abstract class LevelObject : Entity
    {
        protected LevelObject(int id, Vector position, Vector size)
            : base(id, position, size)
        {
        }

        public abstract void Activate(Player p);
    }
}