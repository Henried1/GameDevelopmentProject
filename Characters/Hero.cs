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
        private Animation _attackAnimation;
        private Animation _jumpAnimation;
        private PlayerMovement _movement;

        public Hero(Vector2 startPosition, float speed)
        {
            _movement = new PlayerMovement(startPosition, speed, 0); // Temporary height value
        }

        public void LoadContent(ContentManager content)
        {
            var idleTexture = content.Load<Texture2D>("Hero/Idle");
            var walkTexture = content.Load<Texture2D>("Hero/Walk");
            var attackTexture = content.Load<Texture2D>("Hero/Attack_1");
            var jumpTexture = content.Load<Texture2D>("Hero/Jump");

            _idleAnimation = new Animation(idleTexture, 5);
            _walkAnimation = new Animation(walkTexture, 8);
            _attackAnimation = new Animation(attackTexture, 4);
            _jumpAnimation = new Animation(jumpTexture, 7);

            _movement = new PlayerMovement(_movement.Position, _movement.Speed, Height);
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState, MouseState mouseState, int[,] tileMap, int tileWidth, int tileHeight, int screenWidth, int screenHeight)
        {
            if (_idleAnimation == null || _walkAnimation == null || _attackAnimation == null || _jumpAnimation == null)
            {
                return; 
            }

            _movement.Update(keyboardState, mouseState, tileMap, tileWidth, tileHeight, screenWidth, screenHeight);

            if (_movement.IsAttacking)
            {
                _attackAnimation.Update(gameTime);
            }
            else if (_movement.IsJumping)
            {
                _jumpAnimation.Update(gameTime);
            }
            else if (_movement.IsMoving)
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
            if (_idleAnimation == null || _walkAnimation == null || _attackAnimation == null || _jumpAnimation == null)
            {
                return; // Exit if animations are not yet initialized
            }

            if (_movement.IsAttacking)
            {
                _attackAnimation.Draw(spriteBatch, _movement.Position, _movement.SpriteEffect);
            }
            else if (_movement.IsJumping)
            {
                _jumpAnimation.Draw(spriteBatch, _movement.Position, _movement.SpriteEffect);
            }
            else if (_movement.IsMoving)
            {
                _walkAnimation.Draw(spriteBatch, _movement.Position, _movement.SpriteEffect);
            }
            else
            {
                _idleAnimation.Draw(spriteBatch, _movement.Position, _movement.SpriteEffect);
            }
        }

        public int Height => _idleAnimation?.FrameHeight ?? 0;

        public Vector2 Position
        {
            get => _movement.Position;
            set => _movement.Position = value;
        }
    }
}
