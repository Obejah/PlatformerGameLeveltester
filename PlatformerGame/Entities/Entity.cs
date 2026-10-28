using System;
using System.Windows;
using System.Collections.Generic;
using System.Text;
using PlatformerGame.Interfaces;

namespace PlatformerGame.Entities
{
    public abstract class Entity : IUpdatable
    {
        public int Id { get; set; }
        public Vector Position { get; set; }
        public Vector Velocity { get; set; }
        public Vector Size { get; set; }
        public bool Active { get; set; }

        public string TexturePath { get; set; } = "";

        public Vector SpawnPosition { get; private set; }

        protected Entity(int id, Vector position, Vector size)
        {
            Id = id;
            Position = position;
            SpawnPosition = position;
            Size = size;
            Velocity = new Vector(0,0);
            Active = true;
        }

        public abstract void Update(float dt);

        public virtual void OnCollision(Entity other)
        {

        }

        public Rect GetBounds()
        {
            return new Rect(Position.X, Position.Y, Size.X, Size.Y);
        }

        public virtual void Reset()
        {
            Position = SpawnPosition;
            Velocity = new Vector(0, 0);

            Active = true;
        }
    }
}
