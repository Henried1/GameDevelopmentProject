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
        protected enum EnemyState
        {
            Idle,
            Walking,
            Attacking
        }

        protected EnemyState _currentState;
        protected Animation _idleAnimation;
        protected Animation _walkAnimation;
        protected Vector2 _position;
        protected bool _isMovingRight;
        protected bool _isDead;
        protected int _healthPoints;
        protected double _timeCounter;
        protected double _fps;
        protected double _timePerFrame;

        protected double _idleTimeCounter; 
        protected double _idleDuration = 1.0; 

        protected double _walkDuration = 2.0; 
        protected double _walkTimeCounter; 

        public int Height => _walkAnimation?.FrameHeight ?? 0;
        public int Width => _walkAnimation?.FrameWidth ?? 0;
        public bool IsDead => _isDead;
        public int Damage { get; protected set; }

        public Enemy(Vector2 startPosition, int healthPoints, double fps)
        {
            _position = startPosition;
            _isMovingRight = true; // Start moving right by default
            _isDead = false;
            _healthPoints = healthPoints;
            _fps = fps;
            _timePerFrame = 1.0 / _fps;
            _currentState = EnemyState.Walking; // Start with walking state
        }

        protected ContentManager _content;

        public virtual void LoadContent(ContentManager content)
        {
            _content = content;
        }

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

            switch (_currentState)
            {
                case EnemyState.Walking:
                    _walkTimeCounter += gameTime.ElapsedGameTime.TotalSeconds;

                    if (_walkTimeCounter >= _walkDuration)
                    {
                        _currentState = EnemyState.Idle; // Switch to idle state
                        _walkTimeCounter = 0; // Reset walk time counter
                        _idleTimeCounter = 0; // Reset idle time counter
                    }

                    // Move the enemy
                    _position.X += _isMovingRight ? 1 : -1;
                    break;

                case EnemyState.Idle:
                    _idleTimeCounter += gameTime.ElapsedGameTime.TotalSeconds;

                    if (_idleTimeCounter >= _idleDuration)
                    {
                        _currentState = EnemyState.Walking; // Switch back to walking
                        _isMovingRight = !_isMovingRight; // Reverse direction
                        _walkTimeCounter = 0; // Reset walk time counter
                    }
                    break;
            }

            if (!IsCollidingWithGround(_position, tileMap, tileWidth, tileHeight))
            {
                _position.Y += 1;
            }
            else
            {
                _position.Y = SnapToGround(_position.Y, tileMap, tileWidth, tileHeight);
            }

            // Update animation based on state
            switch (_currentState)
            {
                case EnemyState.Walking:
                    _walkAnimation.Update(gameTime);
                    break;
                case EnemyState.Idle:
                    _idleAnimation.Update(gameTime);
                    break;
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

            // Determine the SpriteEffects based on the direction
            SpriteEffects spriteEffects = _isMovingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            switch (_currentState)
            {
                case EnemyState.Walking:
                    _walkAnimation.Draw(spriteBatch, _position, spriteEffects);
                    break;
                case EnemyState.Idle:
                    _idleAnimation.Draw(spriteBatch, _position, spriteEffects);
                    break;
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

        public virtual Rectangle Hitbox
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
