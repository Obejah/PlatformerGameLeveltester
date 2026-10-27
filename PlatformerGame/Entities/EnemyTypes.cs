using System.Windows;
using PlatformerGame.Enums;

namespace PlatformerGame.Entities
{
    public class EnemyType1 : Enemy
    {
        public EnemyType1(int id, Vector position, Vector size)
            : base(id, position, size, 1200f)
        {
            Health = 1;
            Velocity = new Vector(100, 0); // Move right by default
        }

        public override void Update(float dt)
        {
            if (IsDead) return;
            Patrol();
            ApplyGravity(dt);
            MoveAndCollide(dt);
        }

        public override void Patrol()
        {
            // Placeholder for basic back-and-forth movement
        }
    }

    public class EnemyType2 : Enemy
    {
        public EnemyType2(int id, Vector position, Vector size)
            : base(id, position, size, 1200f)
        {
            Health = 2;
        }

        public override void Update(float dt)
        {
            if (IsDead) return;
            Patrol();
            ApplyGravity(dt);
            MoveAndCollide(dt);
        }

        public override void Patrol()
        {
            // Placeholder for smarter patrol
        }

        public void ShellAttack()
        {
            // Logic for a shell/charge attack
        }
    }
}