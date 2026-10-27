using PlatformerGame.Core;
using System;
using System.Windows;
using System.Windows.Documents;

namespace PlatformerGame.Entities
{
    public class EnemyType1 : Enemy
    {
        public EnemyType1(int id, Vector position, Vector size)
            : base(id, position, size, 1500f)
        {
            Health = 1;
            Speed = 100f;
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
            Velocity = new Vector(Speed * Direction, Velocity.Y);
        }
    }

    public class EnemyType2 : Enemy
    {
        public EnemyType2(int id, Vector position, Vector size)
            : base(id, position, size, 1500f)
        {
            Health = 1;
            Speed = 100f;
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
            if (!IsGrounded) return; 

            double checkDistance = 10.0;
            double checkX = Direction > 0 ? Position.X + Size.X : Position.X - checkDistance;

            Rect ledgeCheck = new Rect(checkX, Position.Y + Size.Y + 5, checkDistance, 5);

            var entitiesAhead = Game.Instance.CurrentLevel.GetEntitiesIn(ledgeCheck);

            bool foundGround = false;
            foreach (var e in entitiesAhead)
            {
                if (e is Platform || e is GameBlock)
                {
                    foundGround = true;
                    break;
                }
            }
            if (!foundGround)
            {
                Direction *= -1;
            }

            Velocity = new Vector(Speed * Direction, Velocity.Y);
        }
    }
}