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

        private enum GameState
        {
            StartScreen,
            Playing,
            GameOver,
            Victory
        }

        private GameState _currentState;
        private Texture2D _startScreenTexture;
        private Texture2D _gameOverScreenTexture;
        private Texture2D _victoryScreenTexture;
        private Texture2D _startButtonTexture;
        private Rectangle _startButtonRectangle;
        private Texture2D _defeatTextTexture;
        private Texture2D _victoryTextTexture;
        private Texture2D _replayButtonTexture;
        private Rectangle _replayButtonRectangle;
        private bool _wasMousePressed;

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

            int buttonWidth = 200;
            int buttonHeight = 200;
            int buttonX = (_graphics.PreferredBackBufferWidth - buttonWidth) / 2;
            int buttonY = (_graphics.PreferredBackBufferHeight - buttonHeight) / 2;
            _startButtonRectangle = new Rectangle(buttonX, buttonY, buttonWidth, buttonHeight);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _mapManager = new MapManager(Content, GraphicsDevice);
            _gameManager = new GameManager(Content, _graphics, this);

            _startScreenTexture = Content.Load<Texture2D>("MenuAssets/startScreen");
            _gameOverScreenTexture = Content.Load<Texture2D>("MenuAssets/medievalKnight");
            _victoryScreenTexture = Content.Load<Texture2D>("MenuAssets/Victory");
            _startButtonTexture = Content.Load<Texture2D>("MenuAssets/start");
            _defeatTextTexture = Content.Load<Texture2D>("MenuAssets/DefeatText");
            _victoryTextTexture = Content.Load<Texture2D>("MenuAssets/VictoryText");
            _replayButtonTexture = Content.Load<Texture2D>("MenuAssets/Replay");

            int buttonWidth = 200;
            int buttonHeight = 200;
            int buttonX = _graphics.PreferredBackBufferWidth - buttonWidth - 400; 
            int buttonY = _graphics.PreferredBackBufferHeight - buttonHeight + 150;
            _startButtonRectangle = new Rectangle(buttonX, buttonY, buttonWidth, buttonHeight);
            _replayButtonRectangle = new Rectangle(buttonX, buttonY, buttonWidth, buttonHeight);
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();

            bool isMousePressed = mouseState.LeftButton == ButtonState.Pressed;

            switch (_currentState)
            {
                case GameState.StartScreen:
                    if (isMousePressed && !_wasMousePressed && _startButtonRectangle.Contains(mouseState.Position))
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
                    if (isMousePressed && !_wasMousePressed && _replayButtonRectangle.Contains(mouseState.Position))
                    {
                        LoadLevel1();
                        _currentState = GameState.Playing;
                    }
                    break;
            }

            _wasMousePressed = isMousePressed;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            switch (_currentState)
            {
                case GameState.StartScreen:
                    _spriteBatch.Draw(_startScreenTexture, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.White);
                    _spriteBatch.Draw(_startButtonTexture, _startButtonRectangle, Color.White);
                    break;
                case GameState.Playing:
                    var backgroundTexture = _mapManager.GetBackgroundTexture();
                    _spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.White);
                    _mapManager.Draw(_spriteBatch);
                    _gameManager.Draw(_spriteBatch);
                    _gameManager.DrawHitboxes(_spriteBatch);
                    break;
                case GameState.GameOver:
                    _spriteBatch.Draw(_gameOverScreenTexture, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.White);
                    DrawCenteredTexture(_defeatTextTexture, -360, 0.8f);
                    _spriteBatch.Draw(_replayButtonTexture, _replayButtonRectangle, Color.White);
                    break;
                case GameState.Victory:
                    _spriteBatch.Draw(_victoryScreenTexture, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.White);
                    DrawCenteredTexture(_victoryTextTexture);
                    _spriteBatch.Draw(_replayButtonTexture, _replayButtonRectangle, Color.White);
                    break;
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawCenteredTexture(Texture2D texture, int xOffset = 0, float scale = 1.0f)
        {
            Vector2 position = new Vector2(
                (_graphics.PreferredBackBufferWidth - texture.Width * scale) / 2 + xOffset,
                (_graphics.PreferredBackBufferHeight - texture.Height * scale) / 2
            );

            _spriteBatch.Draw(texture, position, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
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
