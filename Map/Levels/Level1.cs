using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GameProject.Map;

namespace GameProject.Map.Levels
{
    public class Level1
    {
        private readonly ContentManager _content;
        private readonly GraphicsDevice _graphicsDevice;

        public Level1(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _content = content;
            _graphicsDevice = graphicsDevice;
        }

        public TileMap LoadMap()
        {
            int[,] tileMapArray = new int[,]
            {
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }
            };

            Texture2D groundTexture = _content.Load<Texture2D>("MapAssets/Ground_02");
            Texture2D collisionTexture = new Texture2D(_graphicsDevice, 1, 1);
            collisionTexture.SetData(new[] { Color.White });

            return new TileMap(tileMapArray, groundTexture, collisionTexture);
        }

        public Texture2D LoadBackground()
        {
            return _content.Load<Texture2D>("MapAssets/Background_01");
        }
    }
}
