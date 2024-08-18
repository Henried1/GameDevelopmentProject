using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GameProject.Map;
using GameProject.Map.Levels;

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

        public void LoadLevel(Level1 level)
        {
            _tileMap = level.LoadMap();
            _backgroundTexture = level.LoadBackground();
        }

        public void LoadLevel(Level2 level)
        {
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
            _tileMap?.Draw(spriteBatch);
        }
    }
}
