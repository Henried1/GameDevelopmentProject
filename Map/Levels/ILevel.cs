using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameProject.Map.Levels
{
    public interface ILevel
    {
        TileMap LoadMap();
        Texture2D LoadBackground();
    }
}
