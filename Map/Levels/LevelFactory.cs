using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameProject.Map.Levels
{
    public static class LevelFactory
    {
        public static ILevel CreateLevel(int levelNumber, ContentManager content, GraphicsDevice graphicsDevice)
        {
            switch (levelNumber)
            {
                case 1:
                    return new Level1(content, graphicsDevice);
                case 2:
                    return new Level2(content, graphicsDevice);
                default:
                    throw new ArgumentException("Invalid level number");
            }
        }
    }
}
