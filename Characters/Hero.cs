using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameProject.Animations;
using GameProject.Mechanics;

namespace GameProject.Characters
{
    public class Hero : ICharacter
    {
        private Animation _idleAnimation;
        private Animation _walkAnimation;
        private PlayerMovement _movement;

        public Hero(Vector2 startPosition, float speed)
        {
            _movement = new PlayerMovement(startPosition, speed);
        }

        public void LoadContent(ContentManager content)
        {
            var idleTexture = content.Load<Texture2D>("Hero/Idle");
            var walkTexture = content.Load<Texture2D>("Hero/Walk");

            _idleAnimation = new Animation(idleTexture, 5);
            _walkAnimation = new Animation(walkTexture, 8);
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState, int[,] tileMap, int tileWidth, int tileHeight, int screenWidth, int screenHeight)
        {
            _movement.Update(keyboardState, tileMap, tileWidth, tileHeight, screenWidth, screenHeight);

            if (_movement.IsMoving)
            {
                _walkAnimation.Update(gameTime);
            }
            else
            {
                _idleAnimation.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_movement.IsMoving)
            {
                _walkAnimation.Draw(spriteBatch, _movement.Position, _movement.SpriteEffect);
            }
            else
            {
                _idleAnimation.Draw(spriteBatch, _movement.Position, _movement.SpriteEffect);
            }
        }

        public int Height => _idleAnimation.FrameHeight;

        public Vector2 Position
        {
            get => _movement.Position;
            set => _movement.Position = value;
        }
    }
}
