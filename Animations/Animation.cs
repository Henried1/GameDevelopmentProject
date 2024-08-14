using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProject.Animations
{
    public class Animation
    {
        private Texture2D _texture;
        private int _frameCount;
        private int _currentFrame;
        private double _frameTime;
        private double _timeCounter;

        public Animation(Texture2D texture, int frameCount, double frameTime = 0.2)
        {
            _texture = texture;
            _frameCount = frameCount;
            _frameTime = frameTime;
            _currentFrame = 0;
            _timeCounter = 0;
        }

        public int CurrentFrame
        {
            get => _currentFrame;
            set => _currentFrame = value % _frameCount;
        }

        public int FrameHeight => _texture.Height;
        public int FrameWidth => _texture.Width / _frameCount;
        public int FrameCount => _frameCount;

        public void Update(GameTime gameTime)
        {
            _timeCounter += gameTime.ElapsedGameTime.TotalSeconds;
            if (_timeCounter >= _frameTime)
            {
                _currentFrame = (_currentFrame + 1) % _frameCount;
                _timeCounter -= _frameTime;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, SpriteEffects spriteEffects, float scale = 1f)
        {
            int frameWidth = _texture.Width / _frameCount;
            int frameHeight = _texture.Height;
            Rectangle sourceRectangle = new Rectangle(_currentFrame * frameWidth, 0, frameWidth, frameHeight);
            spriteBatch.Draw(_texture, position, sourceRectangle, Color.White, 0, Vector2.Zero, scale, spriteEffects, 0);
        }

        public void Reset()
        {
            _currentFrame = 0;
            _timeCounter = 0;
        }

        public bool IsComplete => _currentFrame == _frameCount - 1 && _timeCounter >= _frameTime;
    }
}
