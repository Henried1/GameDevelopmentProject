using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace GameProject.Map
{
    public class TileMap
    {
        private readonly int[,] _tileMap;
        private readonly Dictionary<int, Texture2D> _groundTextures;
        private readonly Texture2D _collisionTexture;
        public int CurrentLevel { get; set; }


        public int TileWidth => _groundTextures[1].Width;
        public int TileHeight => _groundTextures[1].Height;
        public int Width => _tileMap.GetLength(1);
        public int Height => _tileMap.GetLength(0);

        public TileMap(int[,] tileMap, Dictionary<int, Texture2D> groundTextures, Texture2D collisionTexture)
        {
            _tileMap = tileMap;
            _groundTextures = groundTextures;
            _collisionTexture = collisionTexture;
        }

        public int[,] GetTileMapArray()
        {
            return _tileMap;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            int tileWidth = TileWidth;
            int tileHeight = TileHeight;

            for (int y = 0; y < _tileMap.GetLength(0); y++)
            {
                for (int x = 0; x < _tileMap.GetLength(1); x++)
                {
                    if (_groundTextures.ContainsKey(_tileMap[y, x]))
                    {
                        spriteBatch.Draw(_groundTextures[_tileMap[y, x]], new Vector2(x * tileWidth, y * tileHeight), Color.White);
                    }
                }
            }
        }

        public void DrawHitboxes(SpriteBatch spriteBatch)
        {
            int tileWidth = TileWidth;
            int tileHeight = TileHeight;

            for (int y = 0; y < _tileMap.GetLength(0); y++)
            {
                for (int x = 0; x < _tileMap.GetLength(1); x++)
                {
                    if (_tileMap[y, x] != 0)
                    {
                        Rectangle hitbox = new Rectangle(x * tileWidth, y * tileHeight, tileWidth, tileHeight);
                       // spriteBatch.Draw(_collisionTexture, hitbox, Color.Red * 0.5f);
                    }
                }
            }
        }

        public Vector2 FindGroundPosition(int heroHeight)
        {
            int tileWidth = TileWidth;
            int tileHeight = TileHeight;

            for (int y = 0; y < _tileMap.GetLength(0); y++)
            {
                for (int x = 0; x < _tileMap.GetLength(1); x++)
                {
                    if (_groundTextures.ContainsKey(_tileMap[y, x]))
                    {
                        return new Vector2(x * tileWidth, (y * tileHeight) - heroHeight);
                    }
                }
            }

            return Vector2.Zero;
        }

        public Rectangle GetBounds()
        {
            return new Rectangle(0, 0, Width * TileWidth, Height * TileHeight);
        }

        public Rectangle GetTileHitbox(int x, int y)
        {
            return new Rectangle(x * TileWidth, y * TileHeight, TileWidth, TileHeight);
        }
    }
}

    