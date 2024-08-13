using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameProject.Animations;
using GameProject.Characters.Interfaces;

namespace GameProject.Characters.Enemies
{
    public abstract class Enemy : ICharacter, ICollidable
    {
        protected Animation _idleAnimation;
        protected Animation _walkAnimation;
        protected Vector2 _position;
        protected bool _isWalking;
        protected bool _isDead;
        protected int _healthPoints;
        protected double _timeCounter;
        protected double _fps;
        protected double _timePerFrame;

        public int Height => _walkAnimation?.FrameHeight ?? 0;
        public int Width => _walkAnimation?.FrameWidth ?? 0;
        public bool IsDead => _isDead;

        public Enemy(Vector2 startPosition, int healthPoints, double fps)
        {
            _position = startPosition;
            _isWalking = false;
            _isDead = false;
            _healthPoints = healthPoints;
            _fps = fps;
            _timePerFrame = 1.0 / _fps;
        }

        public abstract void LoadContent(ContentManager content);

        public virtual void Update(GameTime gameTime, KeyboardState keyboardState, MouseState mouseState, int[,] tileMap, int tileWidth, int tileHeight, int screenWidth, int screenHeight)
        {
            if (_isDead)
            {
                return;
            }

            _timeCounter += gameTime.ElapsedGameTime.TotalSeconds;

            if (_timeCounter >= _timePerFrame)
            {
                _timeCounter -= _timePerFrame;
            }

            if (_isWalking)
            {
                _position.X += 1; 
                if (_position.X > screenWidth - _walkAnimation.FrameWidth)
                {
                    _isWalking = false;
                }
            }
            else
            {
                _position.X -= 1;
                if (_position.X < 0)
                {
                    _isWalking = true;
                }
            }

            if (!IsCollidingWithGround(_position, tileMap, tileWidth, tileHeight))
            {
                _position.Y += 1;
            }
            else
            {
                _position.Y = SnapToGround(_position.Y, tileMap, tileWidth, tileHeight);
            }

            if (_isWalking)
            {
                _walkAnimation.Update(gameTime);
            }
            else
            {
                _idleAnimation.Update(gameTime);
            }
        }

        protected bool IsCollidingWithGround(Vector2 position, int[,] tileMap, int tileWidth, int tileHeight)
        {
            int tileX = (int)(position.X / tileWidth);
            int tileY = (int)((position.Y + Height) / tileHeight);

            if (tileX < 0 || tileX >= tileMap.GetLength(1) || tileY < 0 || tileY >= tileMap.GetLength(0))
            {
                return false;
            }

            return tileMap[tileY, tileX] == 1;
        }

        protected float SnapToGround(float yPosition, int[,] tileMap, int tileWidth, int tileHeight)
        {
            int tileY = (int)((yPosition + Height) / tileHeight);

            while (tileY < tileMap.GetLength(0) && tileMap[tileY, (int)(_position.X / tileWidth)] != 1)
            {
                tileY++;
            }

            return tileY * tileHeight - Height;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (_isDead)
            {
                return;
            }

            if (_isWalking)
            {
                _walkAnimation.Draw(spriteBatch, _position, SpriteEffects.None);
            }
            else
            {
                _idleAnimation.Draw(spriteBatch, _position, SpriteEffects.None);
            }

            // Visualize the hitbox
            Texture2D hitboxTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            hitboxTexture.SetData(new[] { Color.Red });
            spriteBatch.Draw(hitboxTexture, Hitbox, Color.Red * 0.5f);
        }

        public void TakeDamage(int damage)
        {
            _healthPoints -= damage;
            if (_healthPoints <= 0)
            {
                _isDead = true;
            }
        }

        public Rectangle Hitbox
        {
            get
            {
                int hitboxWidth = (int)(_walkAnimation.FrameWidth * 0.45f);
                int hitboxHeight = (int)(_walkAnimation.FrameHeight * 0.3f);
                int hitboxX = (int)(_position.X + (_walkAnimation.FrameWidth - hitboxWidth) / 2);
                int hitboxY = (int)(_position.Y + (_walkAnimation.FrameHeight - hitboxHeight));

                return new Rectangle(hitboxX, hitboxY, hitboxWidth, hitboxHeight);
            }
        }

        public abstract void OnCollision(ICollidable other);

        public Vector2 Position
        {
            get => _position;
            set => _position = value;
        }
    }
}
