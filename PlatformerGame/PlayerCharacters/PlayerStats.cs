namespace PlatformerGame.PlayerCharacters
{
    public class PlayerStats
    {
        public int Lives { get; set; } = 3;
        public int Coins { get; set; } = 0;
        public int Score { get; set; } = 0;

        public void LoseLife()
        {
            Lives--;
        }

        public void GainLife()
        {
            Lives++;
        }

        public bool IsGameOver()
        {
            return Lives <= 0;
        }
    }
}
