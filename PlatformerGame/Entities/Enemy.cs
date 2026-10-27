using PlatformerGame.Core; using PlatformerGame.Interfaces;
using System;
using System.Windows;
using System.Windows.Documents;

namespace PlatformerGame.Entities
{
    public abstract class Enemy : Character, IDamageable
    {
        public int Health { get; set; }
        public bool IsDead { get; protected set; }

        public float Speed { get; set; } = 100f;
        public int Direction { get; set; } = 1;
        public bool IsGrounded { get; private set; }

        protected Enemy(int id, Vector position, Vector size, float gravity)
            : base(id, position, size, gravity) { }

        public abstract void Patrol();

        public virtual void TakeDamage(int amount)
        {
            if (IsDead) return;
            Health -= amount;
            if (Health <= 0) Die();
        }

        protected virtual void Die()
        {
            IsDead = true;
            Active = false;
        }

        public override void ApplyGravity(float dt)
        {
            IsGrounded = false;
            Velocity += new Vector(0, Gravity * dt);

            if (Velocity.Y > 1000f) Velocity = new Vector(Velocity.X, 1000f);
        }

        public override void MoveAndCollide(float dt)
        {
            if (Game.Instance.CurrentLevel == null) return;

            Position += new Vector(0, Velocity.Y * dt);

            var verticalBounds = new Rect(Position.X + 5, Position.Y, Size.X - 10, Size.Y);
            var groundCheck = Game.Instance.CurrentLevel.GetEntitiesIn(verticalBounds);

            foreach (var e in groundCheck)
            {
                if (e is Platform || e is GameBlock)
                {
                    if (GetBounds().IntersectsWith(e.GetBounds()))
                    {
                        if (Velocity.Y > 0) 
                        {
                            Position = new Vector(Position.X, e.Position.Y - Size.Y);
                            Velocity = new Vector(Velocity.X, 0);
                            IsGrounded = true;
                        }
                        else if (Velocity.Y < 0) 
                        {
                            Position = new Vector(Position.X, e.Position.Y + e.Size.Y);
                            Velocity = new Vector(Velocity.X, 0);
                        }
                    }
                }
            }

            Position += new Vector(Velocity.X * dt, 0);

            var horizontalBounds = new Rect(Position.X, Position.Y + 10, Size.X, Size.Y - 20);
            var wallCheck = Game.Instance.CurrentLevel.GetEntitiesIn(horizontalBounds);

            foreach (var e in wallCheck)
            {
                if ((e is Platform p && !p.IsOneWay) || e is GameBlock)
                {
                    if (horizontalBounds.IntersectsWith(e.GetBounds()))
                    {
                        if (Direction > 0) 
                        {
                            Position = new Vector(e.Position.X - Size.X, Position.Y);
                            Direction = -1; 
                        }
                        else if (Direction < 0) 
                        {
                            Position = new Vector(e.Position.X + e.Size.X, Position.Y);
                            Direction = 1; 
                        }

                        Velocity = new Vector(Speed * Direction, Velocity.Y);
                        break;
                    }
                }
            }
        }
    }
}