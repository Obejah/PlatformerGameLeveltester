using PlatformerGame.PlayerCharacters;
using System.Windows;

namespace PlatformerGame.Entities
{
    public abstract class GameBlock : Entity
    {
        public bool IsSolid { get; set; }

        protected GameBlock(int id, Vector position, Vector size, bool isSolid)
            : base(id, position, size)
        {
            IsSolid = isSolid;
        }

        // Called when the player hits the block from underneath
        public abstract void OnHitFromBelow(Player p);
    }
}