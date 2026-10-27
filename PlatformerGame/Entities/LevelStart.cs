using PlatformerGame.Core;
using PlatformerGame.PlayerCharacters;
using System;
using System.Windows;

namespace PlatformerGame.Entities
{
    // 1. LevelStart
    public class LevelStart : LevelObject
    {
        public LevelStart(int id, Vector position, Vector size) : base(id, position, size) { }

        public override void Update(float dt)
        {
            // Usually handled automatically by LevelData.PlayerStart
        }

        public override void Activate(Player p) { }
    }

    // 2. LevelEnd
    public class LevelEnd : LevelObject
    {
        public LevelEnd(int id, Vector position, Vector size) : base(id, position, size) { }

        public override void Update(float dt) { }

        public override void Activate(Player p)
        {
            Game.Instance.OnLevelComplete();
        }
    }

    // 3. Checkpoint (Moved here from its own file)
    public class Checkpoint : LevelObject
    {
        public bool Activated { get; private set; }

        public Checkpoint(int id, Vector position, Vector size)
            : base(id, position, size)
        {
            Activated = false;
        }

        public override void Update(float dt)
        {
            // Checkpoints are static, so there's nothing to update per frame.
        }

        public override void Activate(Player p)
        {
            if (Activated) return;

            Activated = true;
            // Visual change can happen here later (e.g., color change)
        }
    }
}