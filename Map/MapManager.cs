using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GameProject.Map.Levels;
using GameProject.Map;

namespace GameProject.Managers
{
    public class MapManager
    {
        private readonly ContentManager _content;
        private readonly GraphicsDevice _graphicsDevice;
        private TileMap _tileMap;
        private Texture2D _backgroundTexture;

        public MapManager(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _content = content;
            _graphicsDevice = graphicsDevice;
        }

        public void LoadLevel(int levelNumber)
        {
            ILevel level = LevelFactory.CreateLevel(levelNumber, _content, _graphicsDevice);
            _tileMap = level.LoadMap();
            _backgroundTexture = level.LoadBackground();
        }

        public TileMap GetTileMap()
        {
            return _tileMap;
        }

        public Texture2D GetBackgroundTexture()
        {
            return _backgroundTexture;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_backgroundTexture != null)
            {
                spriteBatch.Draw(_backgroundTexture, new Rectangle(0, 0, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height), Color.White);
            }

            _tileMap?.Draw(spriteBatch);
        }
    }
}
