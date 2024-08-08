using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameProject.Characters;
using GameProject.Map;

namespace GameProject.Managers
{
    public class GameManager
    {
        private ICharacter _hero;
        private readonly ContentManager _content;
        private readonly GraphicsDeviceManager _graphics;
        private TileMap _tileMap;

        public GameManager(ContentManager content, GraphicsDeviceManager graphics)
        {
            _content = content;
            _graphics = graphics;
        }

        public void InitializeHero(TileMap tileMap)
        {
            _tileMap = tileMap;

            _hero = new Hero(Vector2.Zero, 2f);
            _hero.LoadContent(_content);

            Vector2 heroPosition = _tileMap.FindGroundPosition(_hero.Height);
            _hero = new Hero(heroPosition, 2f);
            _hero.LoadContent(_content);
        }

        public void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();
            int tileWidth = _tileMap.TileWidth;
            int tileHeight = _tileMap.TileHeight;
            int screenWidth = _graphics.PreferredBackBufferWidth;
            int screenHeight = _graphics.PreferredBackBufferHeight;

            _hero.Update(gameTime, keyboardState, mouseState, _tileMap.GetTileMapArray(), tileWidth, tileHeight, screenWidth, screenHeight);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _hero.Draw(spriteBatch);
        }
    }
}
