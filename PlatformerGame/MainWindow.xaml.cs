using PlatformerGame.Core;
using PlatformerGame.Enums;
using PlatformerGame.Level;
using System.Windows.Media.Imaging; 
using System.Windows.Controls;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PlatformerGame
{
    public partial class MainWindow : Window
    {
        private string _ldtkPath;
        private BitmapImage _normalPlayerImg;
        private BitmapImage _mushroomPlayerImg;

        public MainWindow()
        {

            InitializeComponent();

            _normalPlayerImg = new BitmapImage(new Uri("pack://application:,,,/Assets/PlayerStatic.png"));
            _mushroomPlayerImg = new BitmapImage(new Uri("pack://application:,,,/Assets/PlayerMushroom.png"));

            Game.Instance.Initialize(GameCanvas.Width, GameCanvas.Height);
            Game.Instance.OnLevelLoaded += DrawLevelVisuals;
            Game.Instance.OnStateChanged += HandleGameStateChanged;

            _ldtkPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Levels", "level.ldtk");

            // Set initial state to Menu
            ShowMainMenu();

            CompositionTarget.Rendering += OnRendering;
            Game.Instance.StartGame(); 
        }

        private void HandleGameStateChanged()
        {
            if (Game.Instance.State == GameState.LevelComplete)
            {
                ShowLevelComplete();
            }
        }
        private void ShowLevelComplete()
        {
            MainMenuUI.Visibility = Visibility.Hidden;
            LevelSelectUI.Visibility = Visibility.Hidden;
            LevelCompleteUI.Visibility = Visibility.Visible;
            GameViewbox.Visibility = Visibility.Visible; 

            // Check if there is a next level
            if (Game.Instance.HasNextLevel())
            {
                NextLevelBtn.IsEnabled = true;
                LevelCompleteText.Text = "LEVEL COMPLETE!";
            }
            else
            {
                NextLevelBtn.IsEnabled = false;
                LevelCompleteText.Text = "GAME BEATEN!";
            }
        }

        private void NextLevelButton_Click(object sender, RoutedEventArgs e)
        {
            LevelCompleteUI.Visibility = Visibility.Hidden;
            Game.Instance.LoadNextLevel();
        }

        private void LevelSelectButton_Click(object sender, RoutedEventArgs e)
        {
            ShowLevelSelect();
        }

        private void ShowMainMenu()
        {
            Game.Instance.State = GameState.Menu;
            MainMenuUI.Visibility = Visibility.Visible;
            LevelSelectUI.Visibility = Visibility.Hidden;
            GameViewbox.Visibility = Visibility.Hidden;
        }

        private void ShowLevelSelect()
        {
            Game.Instance.State = GameState.Menu;
            MainMenuUI.Visibility = Visibility.Hidden;
            LevelCompleteUI.Visibility = Visibility.Hidden; // Add this
            LevelSelectUI.Visibility = Visibility.Visible;
            GameViewbox.Visibility = Visibility.Hidden;
        }

        private void StartGameplay()
        {
            MainMenuUI.Visibility = Visibility.Hidden;
            LevelSelectUI.Visibility = Visibility.Hidden;
            LevelCompleteUI.Visibility = Visibility.Hidden; // Add this
            GameViewbox.Visibility = Visibility.Visible;
            Game.Instance.State = GameState.Playing;
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            ShowLevelSelect();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            ShowMainMenu();
        }

        private void LevelButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag != null)
            {
                int levelIndex = int.Parse(btn.Tag.ToString());

                try
                {
                    Game.Instance.LoadCampaignLevels(_ldtkPath);

                    int totalLevels = Game.Instance.GetTotalLevels();
                    if (levelIndex >= totalLevels)
                    {
                        MessageBox.Show($"Level {levelIndex + 1} does not exist yet! You only have {totalLevels} levels in LDtk.", "Level Not Found");
                        return;
                    }

                    // 3. Load the specific level
                    Game.Instance.LoadLevelByIndex(levelIndex);

                    StartGameplay();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load level: {ex.Message}");
                }
            }
        }


        private void DrawLevelVisuals()
        {
            GameCanvas.Children.Clear();

            if (Game.Instance.Player != null)
            {
                PlayerVisual.Width = Game.Instance.Player.Size.X;
                PlayerVisual.Height = Game.Instance.Player.Size.Y;
                PlayerVisual.Tag = Game.Instance.Player;
                GameCanvas.Children.Add(PlayerVisual);
            }

            foreach (var entity in Game.Instance.CurrentLevel.Entities)
            {
                var img = new Image
                {
                    Width = entity.Size.X,
                    Height = entity.Size.Y,
                    Tag = entity,
                    Stretch = Stretch.Fill // Prevents stretching distortion
                };

                RenderOptions.SetBitmapScalingMode(img, BitmapScalingMode.NearestNeighbor);

                if (!string.IsNullOrEmpty(entity.TexturePath))
                {
                    try
                    {
                        string fullPath = $"pack://application:,,,{entity.TexturePath}";
                        img.Source = new BitmapImage(new Uri(fullPath));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[DrawLevelVisuals] Failed to load texture '{entity.TexturePath}': {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"[DrawLevelVisuals] Warning: No texture path set for {entity.GetType().Name}");
                }

                if (entity is PlatformerGame.Entities.HiddenBlock hb && !hb.Revealed)
                {
                    img.Visibility = Visibility.Hidden;
                }

                Canvas.SetLeft(img, entity.Position.X);
                Canvas.SetTop(img, entity.Position.Y);
                GameCanvas.Children.Add(img);
            }
        }

        private void OnRendering(object sender, EventArgs e)
        {
            if (Game.Instance.State == GameState.Menu || Game.Instance.Player == null) return;

            CameraTransform.X = -Game.Instance.Camera.Position.X;
            CameraTransform.Y = -Game.Instance.Camera.Position.Y;

            Canvas.SetLeft(PlayerVisual, Game.Instance.Player.Position.X);
            Canvas.SetTop(PlayerVisual, Game.Instance.Player.Position.Y);

            var targetImage = Game.Instance.Player.CurrentPowerUp is PlatformerGame.PlayerCharacters.MushroomPowerUp
                ? _mushroomPlayerImg
                : _normalPlayerImg;

            if (PlayerVisual.Source != targetImage)
            {
                PlayerVisual.Source = targetImage;
            }

            if (Game.Instance.Player.Velocity.X < 0)
            {
                PlayerVisual.RenderTransformOrigin = new Point(0.5, 0.5);
                PlayerVisual.RenderTransform = new ScaleTransform(-1, 1);
            }
            else if (Game.Instance.Player.Velocity.X > 0)
            {
                PlayerVisual.RenderTransformOrigin = new Point(0.5, 0.5);
                PlayerVisual.RenderTransform = new ScaleTransform(1, 1);
            }

            foreach (UIElement child in GameCanvas.Children)
            {
                if (child is Image img && img.Tag is PlatformerGame.Entities.Entity entity && entity != Game.Instance.Player)
                {
                    Canvas.SetLeft(img, entity.Position.X);
                    Canvas.SetTop(img, entity.Position.Y);

                    img.Visibility = entity.Active ? Visibility.Visible : Visibility.Hidden;
                    if (entity is PlatformerGame.Entities.Enemy enemy)
                    {
                        if (enemy.Direction < 0) // Moving Left
                        {
                            img.RenderTransformOrigin = new Point(0.5, 0.5);
                            img.RenderTransform = new ScaleTransform(-1, 1);
                        }
                        else // Moving Right
                        {
                            img.RenderTransformOrigin = new Point(0.5, 0.5);
                            img.RenderTransform = new ScaleTransform(1, 1);
                        }
                    }
                }
            }

            Title = $"Level: {Game.Instance.CurrentLevel.Name} | HP: {Game.Instance.Player.Health} | State: {Game.Instance.Player.State}";
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (Game.Instance.State == GameState.Playing)
                {
                    ShowMainMenu();
                }
                else
                {
                    Application.Current.Shutdown();
                }
                return; 
            }

            // Pass all other keys to the game
            Game.Instance.Input.OnKeyDown(e.Key);
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            Game.Instance.Input.OnKeyUp(e.Key);
        }
    }
}