using PlatformerGame.Core;
using PlatformerGame.Level;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PlatformerGame
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Game.Instance.Initialize(GameCanvas.Width, GameCanvas.Height);

            Game.Instance.OnLevelLoaded += DrawLevelVisuals;

            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;

            string ldtkPath = System.IO.Path.Combine(appDirectory, "Levels", "Testlevel1.ldtk");
                
            try
            {
                Game.Instance.StartCampaign(ldtkPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to find level: {ex.Message}");
            }

            CompositionTarget.Rendering += OnRendering;
            Game.Instance.StartGame();
        }

            private void DrawLevelVisuals()
            {
                GameCanvas.Children.Clear();

            PlayerVisual.Width = Game.Instance.Player.Size.X;
            PlayerVisual.Height = Game.Instance.Player.Size.Y;
            PlayerVisual.Tag = Game.Instance.Player;
            GameCanvas.Children.Add(PlayerVisual);

            foreach (var entity in Game.Instance.CurrentLevel.Entities)
            {
                var rect = new Rectangle
                {
                    Width = entity.Size.X,
                    Height = entity.Size.Y,
                    Fill = Brushes.Gray,
                     Tag = entity
                };

                if (entity is PlatformerGame.Entities.Ground) rect.Fill = Brushes.DarkGreen;
                if (entity is PlatformerGame.Entities.Coin) rect.Fill = Brushes.Gold;
                if (entity is PlatformerGame.Entities.Checkpoint) rect.Fill = Brushes.Cyan;
                if (entity is PlatformerGame.Entities.LevelEnd) rect.Fill = Brushes.Purple;
                if (entity is PlatformerGame.Entities.EnemyType1) rect.Fill = Brushes.Orange;
                if (entity is PlatformerGame.Entities.EnemyType2) rect.Fill = Brushes.MediumPurple;
                if (entity is PlatformerGame.Entities.Spikes) rect.Fill = Brushes.White;
                if (entity is PlatformerGame.Entities.PowerUpBlock) rect.Fill = Brushes.Orange;
                if (entity is PlatformerGame.Entities.CoinBlock) rect.Fill = Brushes.Goldenrod;
                if (entity is PlatformerGame.Entities.BreakableBlock) rect.Fill = Brushes.Brown;
                if (entity is PlatformerGame.Entities.HiddenBlock)
                {
                    var hb = entity as PlatformerGame.Entities.HiddenBlock;
                    rect.Fill = hb.Revealed ? Brushes.LightGray : Brushes.Transparent;
                }

                Canvas.SetLeft(rect, entity.Position.X);
                Canvas.SetTop(rect, entity.Position.Y);
                GameCanvas.Children.Add(rect);
            }
        }


        private void OnRendering(object sender, EventArgs e)
        {
            if (Game.Instance.Player == null || Game.Instance.CurrentLevel == null) return;

            CameraTransform.X = -Game.Instance.Camera.Position.X;
            CameraTransform.Y = -Game.Instance.Camera.Position.Y;

            foreach (UIElement child in GameCanvas.Children)
            {
                if (child is Rectangle rect && rect.Tag is PlatformerGame.Entities.Entity entity)
                {
                    Canvas.SetLeft(rect, entity.Position.X);
                    Canvas.SetTop(rect, entity.Position.Y);

                    rect.Visibility = entity.Active ? Visibility.Visible : Visibility.Hidden;
                }
            }

            if (Game.Instance.Player.CurrentPowerUp is PlatformerGame.PlayerCharacters.MushroomPowerUp)
            {
                PlayerVisual.Fill = Brushes.LimeGreen;
            }
            else
            {
                PlayerVisual.Fill = Brushes.Red;
            }

            Title = $"Level: {Game.Instance.CurrentLevel.Name} | HP: {Game.Instance.Player.Health} | State: {Game.Instance.Player.State}";
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) { Application.Current.Shutdown(); return; }
            Game.Instance.Input.OnKeyDown(e.Key);
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            Game.Instance.Input.OnKeyUp(e.Key);
        }
    }
}