using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameProject.Characters;
using GameProject.Map;
using GameProject.Map.Levels;

namespace GameProject
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private ICharacter _hero;
        private TileMap _tileMap;
        private Texture2D _backgroundTexture;
        private Level1 _level1;

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
            _backgroundTexture = Content.Load<Texture2D>("MapAssets/Background_01");

            // Initialize Level1
            _level1 = new Level1(Content, GraphicsDevice);

            // Load the first level
            LoadLevel1();
        }

        private void LoadLevel1()
        {
            _tileMap = _level1.LoadMap();

            // Resize the window to fit the tilemap
            int tileWidth = _tileMap.TileWidth;
            int tileHeight = _tileMap.TileHeight;
            int mapWidth = _tileMap.Width * tileWidth;
            int mapHeight = _tileMap.Height * tileHeight;

            _graphics.PreferredBackBufferWidth = mapWidth;
            _graphics.PreferredBackBufferHeight = mapHeight;
            _graphics.ApplyChanges();

            // Load the hero content to get the actual height
            _hero = new Hero(Vector2.Zero, 2f);
            _hero.LoadContent(Content);

            // Find the ground position and set the hero's initial position
            Vector2 heroPosition = _tileMap.FindGroundPosition(_hero.Height);
            _hero = new Hero(heroPosition, 2f);
            _hero.LoadContent(Content);
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            int tileWidth = _tileMap.TileWidth;
            int tileHeight = _tileMap.TileHeight;
            int screenWidth = _graphics.PreferredBackBufferWidth;
            int screenHeight = _graphics.PreferredBackBufferHeight;

            _hero.Update(gameTime, keyboardState, _tileMap.GetTileMapArray(), tileWidth, tileHeight, screenWidth, screenHeight);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            _spriteBatch.Draw(_backgroundTexture, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.White);
            _tileMap.Draw(_spriteBatch);
            _hero.Draw(_spriteBatch);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
