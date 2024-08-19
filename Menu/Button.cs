using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GameProject.Menu
{
    public class Button
    {
        private Texture2D _texture;
        private Rectangle _rectangle;
        private bool _wasMousePressed;

        public Button(Texture2D texture, Rectangle rectangle)
        {
            _texture = texture;
            _rectangle = rectangle;
        }

        public bool IsPressed(MouseState mouseState)
        {
            bool isMousePressed = mouseState.LeftButton == ButtonState.Pressed;
            bool isPressed = isMousePressed && !_wasMousePressed && _rectangle.Contains(mouseState.Position);
            _wasMousePressed = isMousePressed;
            return isPressed;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _rectangle, Color.White);
        }
    }
}
