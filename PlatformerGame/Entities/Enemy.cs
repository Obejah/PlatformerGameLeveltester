using System;
using System.Windows;
using PlatformerGame.Interfaces;

namespace PlatformerGame.Entities
{
    public abstract class Enemy : Character, IDamageable
    {
        public int Health { get; set; }
        public bool IsDead { get; protected set; }

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
            Velocity += new Vector(0, Gravity * dt);
            // Clamp fall speed
            if (Velocity.Y > 1000f) Velocity = new Vector(Velocity.X, 1000f);
        }

        public override void MoveAndCollide(float dt)
        {
            // Basic enemy movement (no collision yet, will fall through floor)
            Position += Velocity * dt;
        }
    }
}