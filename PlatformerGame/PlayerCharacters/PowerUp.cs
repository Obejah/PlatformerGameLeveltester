namespace PlatformerGame.PlayerCharacters
{
    public abstract class PowerUp
    {
        public string Name {  get; set; }

        protected PowerUp(string name)
        {
            Name = name;
        }

        public abstract void Apply(Player p);
        public abstract void Remove(Player p);
    }
}
