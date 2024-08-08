using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameProject.Characters
{
    public interface ICharacter
    {
        void LoadContent(ContentManager content);
        void Update(GameTime gameTime, KeyboardState keyboardState, MouseState mouseState, int[,] tileMap, int tileWidth, int tileHeight, int screenWidth, int screenHeight);
        void Draw(SpriteBatch spriteBatch);
        int Height { get; }
        Vector2 Position { get; set; }
    }
}
