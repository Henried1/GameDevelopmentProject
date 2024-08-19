using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GameProject.Map;
using System.Collections.Generic;
using GameProject.Map.Levels;
public class Level2 : ILevel
{
    private readonly ContentManager _content;
    private readonly GraphicsDevice _graphicsDevice;

    public Level2(ContentManager content, GraphicsDevice graphicsDevice)
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
                { 0, 0, 0, 1, 1, 1, 1, 0, 0, 0 },
                { 1, 1, 1, 2, 2, 2, 2, 1, 1, 1 }
        };

        var groundTextures = new Dictionary<int, Texture2D>
            {
                { 1, _content.Load<Texture2D>("MapAssets/Ground_02") },
                { 2, _content.Load<Texture2D>("MapAssets/Ground_06") }
            };

        Texture2D collisionTexture = new Texture2D(_graphicsDevice, 1, 1);
        collisionTexture.SetData(new[] { Color.White });
        var tileMap = new TileMap(tileMapArray, groundTextures, collisionTexture)
        {
            CurrentLevel = 2 
        };

        return tileMap;
    }

    public Texture2D LoadBackground()
    {
        return _content.Load<Texture2D>("MapAssets/Background_02");
    }
}

