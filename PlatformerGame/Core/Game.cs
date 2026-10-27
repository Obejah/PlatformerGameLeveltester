using PlatformerGame.Enums;
using PlatformerGame.Level;
using PlatformerGame.PlayerCharacters;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;

namespace PlatformerGame.Core
{
    public class Game
    {
        private static Game _instance;
        public static Game Instance => _instance ??= new Game();

        // --- NEW EVENT ---
        // The UI will subscribe to this to redraw the Canvas when a level changes
        public event Action OnLevelLoaded;

        public GameState State { get; set; } = GameState.Playing;
        public aLevel CurrentLevel { get; private set; }
        public Player Player { get; private set; }
        public InputManager Input { get; private set; }
        public Camera Camera { get; private set; }
        public PlayerStats Stats { get; private set; }

        // --- LEVEL PROGRESSION ---
        private string _ldtkFilePath;
        private List<string> _levelIdentifiers;
        private int _currentLevelIndex;

        private DispatcherTimer _gameTimer;
        private DateTime _lastTick;

        public void Initialize(double viewportWidth, double viewportHeight)
        {
            Input = new InputManager();
            Camera = new Camera(viewportWidth, viewportHeight);
            Stats = new PlayerStats();

            _gameTimer = new DispatcherTimer();
            _gameTimer.Interval = TimeSpan.FromMilliseconds(16);
            _gameTimer.Tick += GameLoop;
            _lastTick = DateTime.Now;
        }

        public void StartGame()
        {
            _gameTimer.Start();
        }

        // NEW METHOD: Start the campaign from an LDtk file
        public void StartCampaign(string ldtkPath)
        {
            _ldtkFilePath = ldtkPath;
            _levelIdentifiers = LdtkImporter.GetLevelIdentifiers(ldtkPath);

            if (_levelIdentifiers == null || _levelIdentifiers.Count == 0)
            {
                throw new Exception("No levels found in LDtk file!");
            }

            _currentLevelIndex = 0;
            LoadCurrentLevel();
        }

        private void LoadCurrentLevel()
        {
            string levelName = _levelIdentifiers[_currentLevelIndex];
            LevelData data = LdtkImporter.Import(_ldtkFilePath, levelName);

            CurrentLevel = new aLevel(data);
            // Default camera bounds, ideally you'd read this from LDtk level width/height
            Camera.SetLevelBounds(1920 * data.Screens, 1080);

            Player = new Player(0, CurrentLevel.GetSpawnPoint());
            Camera.Follow(Player);

            State = GameState.Playing;

            // Tell the UI to redraw
            OnLevelLoaded?.Invoke();
        }

        public void LoadNextLevel()
        {
            _currentLevelIndex++;

            if (_currentLevelIndex < _levelIdentifiers.Count)
            {
                // Reset stats? Keep coins? That's up to you.
                // Stats.Coins = 0; 
                LoadCurrentLevel();
            }
            else
            {
                // No more levels
                GameOver();
                MessageBox.Show("You beat the game!");
            }
        }

        private void GameLoop(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            float dt = (float)(now - _lastTick).TotalSeconds;
            _lastTick = now;

            if (dt > 0.1f) dt = 0.1f;

            Update(dt);
            Input.Poll();
        }

        public void Update(float dt)
        {
            if (State != GameState.Playing || Player == null || CurrentLevel == null) return;

            if (Player.Position.Y > 2000) Player.Respawn(CurrentLevel.GetSpawnPoint());

            float moveDirection = 0;
            if (Input.Left) moveDirection -= 1;
            if (Input.Right) moveDirection += 1;

            Player.Move(moveDirection, Input.Run);

            if (Input.JumpPressed) Player.TryJump();

            Player.Update(dt);
            CurrentLevel.Update(dt);
            Camera.Update(dt);

            if (!Player.IsAlive()) OnPlayerDied();
        }

        public void RestartLevel()
        {
            CurrentLevel.Reset();
            Player.Respawn(CurrentLevel.GetSpawnPoint());
            State = GameState.Playing;
        }

        public void OnPlayerDied()
        {
            if (Stats.IsGameOver()) GameOver();
            else RestartLevel();
        }

        public void OnLevelComplete()
        {
            State = GameState.LevelComplete;

            // Wait 1 second before loading next level so the player can breathe
            var transitionTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            transitionTimer.Tick += (s, e) =>
            {
                transitionTimer.Stop();
                LoadNextLevel();
            };
            transitionTimer.Start();
        }

        public void GameOver()
        {
            State = GameState.GameOver;
            _gameTimer.Stop();
        }

        // Add this method back in!
        public void LoadLevel(LevelData data)
        {
            CurrentLevel = new aLevel(data);

            // Set default camera bounds for the fallback
            Camera.SetLevelBounds(1920 * data.Screens, 1080);

            Player = new Player(0, CurrentLevel.GetSpawnPoint());

            Camera.Follow(Player);
            State = GameState.Playing;

            // This is crucial! It tells the UI to clear the canvas and redraw
            OnLevelLoaded?.Invoke();
        }
    }
}