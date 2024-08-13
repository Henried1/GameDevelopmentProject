using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameProject.Animations;
using GameProject.Mechanics;
using System;
using GameProject.Characters.Interfaces;
using GameProject.Characters.Enemies;

namespace GameProject.Characters.Player
{
    public class Hero : ICharacter, ICollidable
    {
        private enum HeroState
        {
            Idle,
            Walking,
            Attacking,
            Jumping,
            Dying
        }

        private HeroState _currentState;
        private Animation _idleAnimation;
        private Animation _walkAnimation;
        private Animation _attackAnimation;
        private Animation _jumpAnimation;
        private Animation _deathAnimation;
        private PlayerMovement _movement;
        private int _healthPoints;
        private bool _isDead;
        private double _attackCooldown;
        private double _attackTimer;
        public event Action OnDamageTaken;

        public int Height => _idleAnimation?.FrameHeight ?? 0;
        public int Width => _idleAnimation?.FrameWidth ?? 0;

        public Hero(Vector2 startPosition, float speed)
        {
            _movement = new PlayerMovement(startPosition, speed, 0);
            _healthPoints = 100;
            _isDead = false;
            _attackCooldown = 0.5;
            _attackTimer = 0;
            _currentState = HeroState.Idle;
        }

        public void LoadContent(ContentManager content)
        {
            var idleTexture = content.Load<Texture2D>("Hero/Idle");
            var walkTexture = content.Load<Texture2D>("Hero/Walk");
            var attackTexture = content.Load<Texture2D>("Hero/Attack_1");
            var jumpTexture = content.Load<Texture2D>("Hero/Jump");
            var deathTexture = content.Load<Texture2D>("Hero/Dead");

            _idleAnimation = new Animation(idleTexture, 5);
            _walkAnimation = new Animation(walkTexture, 8);
            _attackAnimation = new Animation(attackTexture, 4);
            _jumpAnimation = new Animation(jumpTexture, 7);
            _deathAnimation = new Animation(deathTexture, 4); 

            _movement = new PlayerMovement(_movement.Position, _movement.Speed, Height);
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState, MouseState mouseState, int[,] tileMap, int tileWidth, int tileHeight, int screenWidth, int screenHeight)
        {
            if (_isDead)
            {
                _currentState = HeroState.Dying;
                _deathAnimation.Update(gameTime);
                return;
            }

            if (_idleAnimation == null || _walkAnimation == null || _attackAnimation == null || _jumpAnimation == null)
            {
                return;
            }

            _movement.Update(keyboardState, mouseState, tileMap, tileWidth, tileHeight, screenWidth, screenHeight);

            _attackTimer += gameTime.ElapsedGameTime.TotalSeconds;

            if (mouseState.LeftButton == ButtonState.Pressed && _attackTimer >= _attackCooldown)
            {
                _currentState = HeroState.Attacking;
                _attackTimer = 0;
                _attackAnimation.CurrentFrame = 0; 
            }

            switch (_currentState)
            {
                case HeroState.Attacking:
                    _attackAnimation.Update(gameTime);
                    if (_attackAnimation.CurrentFrame == _attackAnimation.FrameCount - 1)
                    {
                        _currentState = HeroState.Idle;
                    }
                    break;
                case HeroState.Jumping:
                    _jumpAnimation.Update(gameTime);
                    if (!_movement.IsJumping)
                    {
                        _currentState = HeroState.Idle;
                    }
                    break;
                case HeroState.Walking:
                    _walkAnimation.Update(gameTime);
                    if (!_movement.IsMoving)
                    {
                        _currentState = HeroState.Idle;
                    }
                    break;
                case HeroState.Idle:
                    _idleAnimation.Update(gameTime);
                    if (_movement.IsJumping)
                    {
                        _currentState = HeroState.Jumping;
                    }
                    else if (_movement.IsMoving)
                    {
                        _currentState = HeroState.Walking;
                    }
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_isDead)
            {
                _deathAnimation.Draw(spriteBatch, _movement.Position, _movement.SpriteEffect);
                return;
            }

            if (_idleAnimation == null || _walkAnimation == null || _attackAnimation == null || _jumpAnimation == null)
            {
                return;
            }

            switch (_currentState)
            {
                case HeroState.Attacking:
                    _attackAnimation.Draw(spriteBatch, _movement.Position, _movement.SpriteEffect);
                    break;
                case HeroState.Jumping:
                    _jumpAnimation.Draw(spriteBatch, _movement.Position, _movement.SpriteEffect);
                    break;
                case HeroState.Walking:
                    _walkAnimation.Draw(spriteBatch, _movement.Position, _movement.SpriteEffect);
                    break;
                case HeroState.Idle:
                    _idleAnimation.Draw(spriteBatch, _movement.Position, _movement.SpriteEffect);
                    break;
            }

            Texture2D hitboxTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            hitboxTexture.SetData(new[] { Color.Red });
            spriteBatch.Draw(hitboxTexture, Hitbox, Color.Red * 0.5f);
        }

        public void TakeDamage(int damage)
        {
            _healthPoints -= damage;
            OnDamageTaken?.Invoke();
            if (_healthPoints <= 0)
            {
                _isDead = true;
            }
        }

        public Vector2 Position
        {
            get => _movement.Position;
            set => _movement.Position = value;
        }

        public bool IsDead => _isDead;

        public int HealthPoints => _healthPoints;

        public float HealthPercentage => (float)_healthPoints / 100;

        public Rectangle AttackHitbox
        {
            get
            {
                if (_currentState == HeroState.Attacking)
                {
                    int frameWidth = _attackAnimation.FrameWidth;
                    int frameHeight = _attackAnimation.FrameHeight;
                    return new Rectangle((int)_movement.Position.X, (int)_movement.Position.Y, frameWidth, frameHeight);
                }
                return Rectangle.Empty;
            }
        }

        public Rectangle Hitbox
        {
            get
            {
                int hitboxWidth = (int)(_idleAnimation.FrameWidth * 0.6f);
                int hitboxHeight = (int)(_idleAnimation.FrameHeight * 0.5f);
                int hitboxX = (int)(_movement.Position.X + (_idleAnimation.FrameWidth - hitboxWidth) / 2);
                int hitboxY = (int)(_movement.Position.Y + (_idleAnimation.FrameHeight - hitboxHeight));

                return new Rectangle(hitboxX, hitboxY, hitboxWidth, hitboxHeight);
            }
        }

        public void OnCollision(ICollidable other)
        {
            if (other is Enemy enemy)
            {
                if (!enemy.IsDead)
                {
                    this.TakeDamage(1);

                    if (_currentState == HeroState.Attacking && AttackHitbox.Intersects(enemy.Hitbox))
                    {
                        enemy.TakeDamage(10);
                    }
                }
            }
        }
    }
}
