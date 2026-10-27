using System.Windows;

namespace PlatformerGame.Entities
{
    public abstract class Character : Entity
    {
        protected float Gravity { get; set; }

        protected Character(int id, Vector position, Vector size, float gravity)
            : base(id, position, size)
        {
            Gravity = gravity;
        }

        public abstract void ApplyGravity(float dt);
        public abstract void MoveAndCollide(float dt);
    }
}
