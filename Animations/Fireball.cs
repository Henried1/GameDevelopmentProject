using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameProject.Characters.Enemies
{
    public class Fireball
    {
        private Texture2D _texture;
        private Texture2D _hitboxTexture;
        private Vector2 _position;
        private Vector2 _direction;
        public bool MarkForRemoval { get; set; }
        public int Damage { get; private set; } = 10;

        public Fireball(Vector2 position, Vector2 direction, GraphicsDevice graphicsDevice)
        {
            _position = position;
            _direction = direction;

            _hitboxTexture = new Texture2D(graphicsDevice, 1, 1);
            _hitboxTexture.SetData(new[] { Color.White });
        }

        public void LoadContent(ContentManager content)
        {
            _texture = content.Load<Texture2D>("Enemies/FireSpirit/fireball");
        }

        public void Update(GameTime gameTime)
        {
            float speed = 300f; 
            _position += _direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            float scale = 0.1f; 
            Vector2 origin = new Vector2(_texture.Width / 2, _texture.Height / 2);
            spriteBatch.Draw(_texture, _position, null, Color.White, 0f, origin, scale, SpriteEffects.FlipHorizontally, 0f);

            Rectangle fireballHitbox = new Rectangle(
                (int)(_position.X - (_texture.Width * scale) / 2),
                (int)(_position.Y - (_texture.Height * scale) / 2),
                (int)(_texture.Width * scale),
                (int)(_texture.Height * scale)
            );

            DrawRectangle(spriteBatch, fireballHitbox, Color.Red);
        }

        private void DrawRectangle(SpriteBatch spriteBatch, Rectangle rectangle, Color color)
        {
            // Draw top line
            spriteBatch.Draw(_hitboxTexture, new Rectangle(rectangle.Left, rectangle.Top, rectangle.Width, 1), color);
            // Draw left line
            spriteBatch.Draw(_hitboxTexture, new Rectangle(rectangle.Left, rectangle.Top, 1, rectangle.Height), color);
            // Draw right line
            spriteBatch.Draw(_hitboxTexture, new Rectangle(rectangle.Right, rectangle.Top, 1, rectangle.Height), color);
            // Draw bottom line
            spriteBatch.Draw(_hitboxTexture, new Rectangle(rectangle.Left, rectangle.Bottom, rectangle.Width, 1), color);
        }

        public bool CheckCollision(Rectangle hitbox)
        {
            Rectangle fireballHitbox = new Rectangle(
                (int)(_position.X - (_texture.Width * 0.1f) / 2),
                (int)(_position.Y - (_texture.Height * 0.1f) / 2),
                (int)(_texture.Width * 0.1f),
                (int)(_texture.Height * 0.1f)
            );
            return fireballHitbox.Intersects(hitbox);
        }

        public bool IsOffScreen(int screenWidth, int screenHeight)
        {
            return _position.X < 0 || _position.X > screenWidth || _position.Y < 0 || _position.Y > screenHeight;
        }
    }
}
