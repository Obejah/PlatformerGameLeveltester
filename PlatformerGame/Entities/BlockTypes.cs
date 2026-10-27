using PlatformerGame.PlayerCharacters;
using System.Windows;

namespace PlatformerGame.Entities
{
    public class PowerUpBlock : GameBlock
    {
        public PowerUp Content { get; set; }
        public bool Used { get; private set; }

        public PowerUpBlock(int id, Vector position, Vector size, PowerUp content)
            : base(id, position, size, true)
        {
            Content = content;
        }

        public override void Update(float dt) { }

        public override void OnHitFromBelow(Player p)
        {
            if (Used) return;
            Used = true;

            // Give the player the powerup!
            p.ApplyPowerUp(Content);

            // Optional: Change visual state (handled in MainWindow for now)
        }
    }

    public class CoinBlock : GameBlock
    {
        public int CoinsLeft { get; set; }

        public CoinBlock(int id, Vector position, Vector size, int coins)
            : base(id, position, size, true)
        {
            CoinsLeft = coins;
        }

        public override void Update(float dt) { }

        public override void OnHitFromBelow(Player p)
        {
            if (CoinsLeft <= 0) return;
            CoinsLeft--;
            p.Stats.Coins++;
            p.Stats.Score += 10;
        }
    }

    public class BreakableBlock : GameBlock
    {
        public bool Broken { get; private set; }

        public BreakableBlock(int id, Vector position, Vector size)
            : base(id, position, size, true) { }

        public override void Update(float dt) { }

        public override void OnHitFromBelow(Player p)
        {
            Broken = true;
            Active = false; // Disappears from the level
        }
    }

    public class HiddenBlock : GameBlock
    {
        public bool Revealed { get; private set; }

        public HiddenBlock(int id, Vector position, Vector size)
            : base(id, position, size, false)
        {
            // Starts invisible and non-solid
        }

        public override void Update(float dt) { }

        public override void OnHitFromBelow(Player p)
        {
            Revealed = true;
            IsSolid = true;
        }
    }
}