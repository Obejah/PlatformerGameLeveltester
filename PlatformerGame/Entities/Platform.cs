using System.Windows;

namespace PlatformerGame.Entities
{
    public abstract class Platform : Entity
    {
        public bool IsOneWay { get; protected set; }

        protected Platform(int id, Vector position, Vector size, bool isOneWay)
            : base(id, position, size)
        {
            IsOneWay = isOneWay;
        }

        public override void Update(float dt) { }
    }
}