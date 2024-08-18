using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _mapManager = new MapManager(Content, GraphicsDevice);
            _gameManager = new GameManager(Content, _graphics);
             //LoadLevel1();
             LoadLevel2(); 
        }

        private void LoadLevel1()
        {
            var level1 = new Level1(Content, GraphicsDevice);
            _mapManager.LoadLevel(level1);

            var tileMap = _mapManager.GetTileMap();
            int tileWidth = tileMap.TileWidth;
            int tileHeight = tileMap.TileHeight;
            int mapWidth = tileMap.Width * tileWidth;
            int mapHeight = tileMap.Height * tileHeight;

            _graphics.PreferredBackBufferWidth = mapWidth;
            _graphics.PreferredBackBufferHeight = mapHeight;
            _graphics.ApplyChanges();

            _gameManager.InitializeHero(tileMap);
            _gameManager.InitializeEnemies();
        }

        private void LoadLevel2()
        {
            var level2 = new Level2(Content, GraphicsDevice);
            _mapManager.LoadLevel(level2);

            var tileMap = _mapManager.GetTileMap();
            int tileWidth = tileMap.TileWidth;
            int tileHeight = tileMap.TileHeight;
            int mapWidth = tileMap.Width * tileWidth;
            int mapHeight = tileMap.Height * tileHeight;

            _graphics.PreferredBackBufferWidth = mapWidth;
            _graphics.PreferredBackBufferHeight = mapHeight;
            _graphics.ApplyChanges();

            _gameManager.InitializeHero(tileMap);
            _gameManager.InitializeEnemies();
        }

        protected override void Update(GameTime gameTime)
        {
            _gameManager.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            var backgroundTexture = _mapManager.GetBackgroundTexture();
            _spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.White);

            _mapManager.Draw(_spriteBatch);
            _gameManager.Draw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
