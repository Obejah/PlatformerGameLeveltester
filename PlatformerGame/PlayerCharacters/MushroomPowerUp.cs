namespace PlatformerGame.PlayerCharacters
{
    public class MushroomPowerUp : PowerUp
    {
        public MushroomPowerUp() : base("Mushroom") { }

        public override void Apply(Player p)
        {
            // Example: Increase max health or make player big
            p.MaxHealth += 1;
            p.Health = p.MaxHealth;
        }
        public override void Remove(Player p)
        {
            p.MaxHealth -= 1;
            if (p.Health > p.MaxHealth) p.Health = p.MaxHealth;
        }   
    }
}
