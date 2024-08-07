using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProject.Animations
{
    public class Animation
    {
        private Texture2D _texture;
        private int _frameCount;
        private int _currentFrame;
        private double _frameTimer;
        private const double FrameTime = 100; // Time per frame in milliseconds
        private int _frameWidth;
        private int _frameHeight;

        public Animation(Texture2D texture, int frameCount)
        {
            _texture = texture;
            _frameCount = frameCount;
            _currentFrame = 0;
            _frameTimer = FrameTime;
            _frameWidth = _texture.Width / _frameCount;
            _frameHeight = _texture.Height;
        }

        public void Update(GameTime gameTime)
        {
            _frameTimer -= gameTime.ElapsedGameTime.TotalMilliseconds;
            if (_frameTimer <= 0)
            {
                _frameTimer = FrameTime;
                _currentFrame = (_currentFrame + 1) % _frameCount;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, SpriteEffects spriteEffect)
        {
            Rectangle sourceRectangle = new Rectangle(_currentFrame * _frameWidth, 0, _frameWidth, _frameHeight);
            spriteBatch.Draw(_texture, position, sourceRectangle, Color.White, 0f, Vector2.Zero, 1f, spriteEffect, 0f);
        }

        public int FrameHeight => _frameHeight;
    }
}
