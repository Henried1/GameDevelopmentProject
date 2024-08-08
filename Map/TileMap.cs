using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProject.Map
{
    public class TileMap
    {
        private readonly int[,] _tileMap;
        private readonly Texture2D _groundTexture;
        private readonly Texture2D _collisionTexture;

        public int TileWidth => _groundTexture.Width;
        public int TileHeight => _groundTexture.Height;
        public int Width => _tileMap.GetLength(1);
        public int Height => _tileMap.GetLength(0);

        public TileMap(int[,] tileMap, Texture2D groundTexture, Texture2D collisionTexture)
        {
            _tileMap = tileMap;
            _groundTexture = groundTexture;
            _collisionTexture = collisionTexture;
        }

        public int[,] GetTileMapArray()
        {
            return _tileMap;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            int tileWidth = _groundTexture.Width;
            int tileHeight = _groundTexture.Height;

            for (int y = 0; y < _tileMap.GetLength(0); y++)
            {
                for (int x = 0; x < _tileMap.GetLength(1); x++)
                {
                    if (_tileMap[y, x] == 1)
                    {
                        spriteBatch.Draw(_groundTexture, new Vector2(x * tileWidth, y * tileHeight), Color.White);

                        /*var rectangle = new Rectangle(x * tileWidth, y * tileHeight, tileWidth, tileHeight);
                        spriteBatch.Draw(_collisionTexture, rectangle, Color.Red * 0.5f);*/
                    }
                }
            }
        }

        public Vector2 FindGroundPosition(int heroHeight)
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
    }
}
