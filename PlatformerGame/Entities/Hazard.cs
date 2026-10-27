using System.Windows;

namespace PlatformerGame.Entities
{
    public class Hazard : Entity
    {
        public int Damage { get; set; }

        public Hazard(int id, Vector position, Vector size, int damage)
            : base(id, position, size)
        {
            Damage = damage;
        }

        public override void Update(float dt) { }
    }
}