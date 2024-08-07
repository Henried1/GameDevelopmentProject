using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameProject.Characters;

namespace GameProject
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private ICharacter _hero;
        private Texture2D _groundTexture;
        private Texture2D _backgroundTexture;
        private int[,] _tileMap;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _tileMap = new int[,]
            {
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }
            };

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _groundTexture = Content.Load<Texture2D>("MapAssets/Ground_02");
            _backgroundTexture = Content.Load<Texture2D>("MapAssets/Background_01"); 

            // Resize the window to fit the tilemap
            int tileWidth = _groundTexture.Width;
            int tileHeight = _groundTexture.Height;
            int mapWidth = _tileMap.GetLength(1) * tileWidth;
            int mapHeight = _tileMap.GetLength(0) * tileHeight;

            _graphics.PreferredBackBufferWidth = mapWidth;
            _graphics.PreferredBackBufferHeight = mapHeight;
            _graphics.ApplyChanges();

            _hero = new Hero(Vector2.Zero, 2f);
            _hero.LoadContent(Content);

            Vector2 heroPosition = FindGroundPosition(_hero.Height);
            _hero = new Hero(heroPosition, 2f);
            _hero.LoadContent(Content);
        }

        private Vector2 FindGroundPosition(int heroHeight)
        {
            int tileWidth = _groundTexture.Width;
            int tileHeight = _groundTexture.Height;

            for (int y = 0; y < _tileMap.GetLength(0); y++)
            {
                for (int x = 0; x < _tileMap.GetLength(1); x++)
                {
                    if (_tileMap[y, x] == 1)
                    {
                        return new Vector2(x * tileWidth, (y * tileHeight) - heroHeight);
                    }
                }
            }

            return Vector2.Zero;
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            int tileWidth = _groundTexture.Width;
            int tileHeight = _groundTexture.Height;
            int screenWidth = _graphics.PreferredBackBufferWidth;
            int screenHeight = _graphics.PreferredBackBufferHeight;

            _hero.Update(gameTime, keyboardState, _tileMap, tileWidth, tileHeight, screenWidth, screenHeight);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            _spriteBatch.Draw(_backgroundTexture, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.White);

            int tileWidth = _groundTexture.Width;
            int tileHeight = _groundTexture.Height;

            for (int y = 0; y < _tileMap.GetLength(0); y++)
            {
                for (int x = 0; x < _tileMap.GetLength(1); x++)
                {
                    if (_tileMap[y, x] == 1)
                    {
                        _spriteBatch.Draw(_groundTexture, new Vector2(x * tileWidth, y * tileHeight), Color.White);
                    }
                }
            }

            _hero.Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
