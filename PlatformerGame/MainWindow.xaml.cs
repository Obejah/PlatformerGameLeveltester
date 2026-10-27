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

            // Subscribe to the event that fires when a level finishes loading
            Game.Instance.OnLevelLoaded += DrawLevelVisuals;

            // Set your path here!
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
            GameCanvas.Children.Add(PlayerVisual);

            foreach (var entity in Game.Instance.CurrentLevel.Entities)
            {
                var rect = new Rectangle
                {
                    Width = entity.Size.X,
                    Height = entity.Size.Y,
                    Fill = Brushes.Gray
                };

                if (entity is PlatformerGame.Entities.Ground) rect.Fill = Brushes.DarkGreen;
                if (entity is PlatformerGame.Entities.Coin) rect.Fill = Brushes.Gold;
                if (entity is PlatformerGame.Entities.Checkpoint) rect.Fill = Brushes.Cyan;
                if (entity is PlatformerGame.Entities.LevelEnd) rect.Fill = Brushes.Purple;
                if (entity is PlatformerGame.Entities.Spikes) rect.Fill = Brushes.White;


                Canvas.SetLeft(rect, entity.Position.X);
                Canvas.SetTop(rect, entity.Position.Y);
                GameCanvas.Children.Add(rect);
            }
        }


        private void OnRendering(object sender, EventArgs e)
        {
            if (Game.Instance.Player == null) return;

            CameraTransform.X = -Game.Instance.Camera.Position.X;
            CameraTransform.Y = -Game.Instance.Camera.Position.Y;

            Canvas.SetLeft(PlayerVisual, Game.Instance.Player.Position.X);
            Canvas.SetTop(PlayerVisual, Game.Instance.Player.Position.Y);

            Title = $"Level: {Game.Instance.CurrentLevel.Name} | State: {Game.Instance.Player.State} | Pos: {Game.Instance.Player.Position.X:F0}, {Game.Instance.Player.Position.Y:F0}";
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