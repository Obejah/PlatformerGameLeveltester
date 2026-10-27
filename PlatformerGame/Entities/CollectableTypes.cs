using PlatformerGame.PlayerCharacters;
using System.Windows;

namespace PlatformerGame.Entities
{
    public class Coin : Collectable
    {
        public Coin(int id, Vector position, Vector size) : base(id, position, size) { }

        public override void Update(float dt) { }

        public override void Collect(Player p)
        {
            if (IsCollected) return;
            IsCollected = true;
            Active = false; // Hides the coin
            p.Stats.Coins++;
            p.Stats.Score += 100;
        }
    }

    public class Fossil : Collectable
    {
        public bool IsSecret { get; set; }
        public int FossilID { get; set; }

        public Fossil(int id, Vector position, Vector size, int fossilId, bool isSecret)
            : base(id, position, size)
        {
            FossilID = fossilId;
            IsSecret = isSecret;
        }

        public override void Update(float dt) { }

        public override void Collect(Player p)
        {
            if (IsCollected) return;
            IsCollected = true;
            Active = false;
            // The Level class will track FossilsCollected
        }
    }
}