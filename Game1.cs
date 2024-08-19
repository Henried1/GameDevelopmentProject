using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameProject.Managers;
using GameProject.Map.Levels;

namespace GameProject
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private MapManager _mapManager;
        private GameManager _gameManager;
        private UIManager _uiManager;

        private enum GameState
        {
            StartScreen,
            Playing,
            GameOver,
            Victory
        }

        private GameState _currentState;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
            _currentState = GameState.StartScreen;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _mapManager = new MapManager(Content, GraphicsDevice);
            _gameManager = new GameManager(Content, _graphics, this);
            _uiManager = new UIManager(_graphics, _spriteBatch, Content);
        }

        protected override void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();

            switch (_currentState)
            {
                case GameState.StartScreen:
                    if (_uiManager.IsStartButtonPressed(mouseState))
                    {
                        LoadLevel1();
                        _currentState = GameState.Playing;
                    }
                    break;
                case GameState.Playing:
                    _gameManager.Update(gameTime);
                    if (_gameManager.IsGameOver)
                    {
                        _currentState = GameState.GameOver;
                    }
                    else if (_gameManager.IsVictory)
                    {
                        _currentState = GameState.Victory;
                    }
                    break;
                case GameState.GameOver:
                case GameState.Victory:
                    if (_uiManager.IsReplayButtonPressed(mouseState))
                    {
                        LoadLevel1();
                        _currentState = GameState.Playing;
                    }
                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            switch (_currentState)
            {
                case GameState.StartScreen:
                    _uiManager.DrawStartScreen();
                    break;
                case GameState.Playing:
                    var backgroundTexture = _mapManager.GetBackgroundTexture();
                    _spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.White);
                    _mapManager.Draw(_spriteBatch);
                    _gameManager.Draw(_spriteBatch);
                    _gameManager.DrawHitboxes(_spriteBatch);
                    break;
                case GameState.GameOver:
                    _uiManager.DrawGameOverScreen();
                    break;
                case GameState.Victory:
                    _uiManager.DrawVictoryScreen();
                    break;
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void LoadLevel1()
        {
            _mapManager.LoadLevel(1);

            var tileMap = _mapManager.GetTileMap();
            int tileWidth = tileMap.TileWidth;
            int tileHeight = tileMap.TileHeight;
            int mapWidth = tileMap.Width * tileWidth;
            int mapHeight = tileMap.Height * tileHeight;

            _graphics.PreferredBackBufferWidth = mapWidth;
            _graphics.PreferredBackBufferHeight = mapHeight;
            _graphics.ApplyChanges();

            _gameManager.InitializeHero(tileMap);
            _gameManager.InitializeEnemies(1);
            _gameManager.ClearPowerups();
            _gameManager.ResetGameState();
        }

        public void LoadNextLevel()
        {
            _mapManager.LoadLevel(2);

            var tileMap = _mapManager.GetTileMap();
            int tileWidth = tileMap.TileWidth;
            int tileHeight = tileMap.TileHeight;
            int mapWidth = tileMap.Width * tileWidth;
            int mapHeight = tileMap.Height * tileHeight;

            _graphics.PreferredBackBufferWidth = mapWidth;
            _graphics.PreferredBackBufferHeight = mapHeight;
            _graphics.ApplyChanges();

            _gameManager.InitializeHero(tileMap);
            _gameManager.InitializeEnemies(2);
            _gameManager.ClearPowerups();
        }
    }
}
