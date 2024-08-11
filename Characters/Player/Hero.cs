using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameProject.Animations;
using GameProject.Mechanics;
using System;
using GameProject.Characters.Interfaces;

namespace GameProject.Characters.Player
{
    public class Hero : ICharacter, ICollidable
    {
        private Animation _idleAnimation;
        private Animation _walkAnimation;
        private Animation _attackAnimation;
        private Animation _jumpAnimation;
        private Animation _deathAnimation;
        private PlayerMovement _movement;
        private int _healthPoints;
        private bool _isDead;
        private bool _isAttacking;
        private double _attackCooldown;
        private double _attackTimer;
        public event Action OnDamageTaken;

        public int Height => _idleAnimation?.FrameHeight ?? 0;
        public int Width => _idleAnimation?.FrameWidth ?? 0;

        public Hero(Vector2 startPosition, float speed)
        {
            _movement = new PlayerMovement(startPosition, speed, 0); // Temporary height value
            _healthPoints = 100; // Initial health points
            _isDead = false;
            _isAttacking = false;
            _attackCooldown = 0.5; // Cooldown time in seconds
            _attackTimer = 0;
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
            _deathAnimation = new Animation(deathTexture, 4); // 4 frames for death animation

            _movement = new PlayerMovement(_movement.Position, _movement.Speed, Height);
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState, MouseState mouseState, int[,] tileMap, int tileWidth, int tileHeight, int screenWidth, int screenHeight)
        {
            if (_isDead)
            {
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
                _isAttacking = true;
                _attackTimer = 0;
                _attackAnimation.CurrentFrame = 0; // Reset the attack animation frame
            }

            if (_isAttacking)
            {
                _attackAnimation.Update(gameTime);
                if (_attackAnimation.CurrentFrame == _attackAnimation.FrameCount - 1)
                {
                    _isAttacking = false;
                }
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
            if (_isDead)
            {
                _deathAnimation.Draw(spriteBatch, _movement.Position, _movement.SpriteEffect);
                return;
            }

            if (_idleAnimation == null || _walkAnimation == null || _attackAnimation == null || _jumpAnimation == null)
            {
                return;
            }

            if (_isAttacking)
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
                if (_isAttacking)
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
                // Adjust the hitbox dimensions as needed
                int hitboxWidth = 40;
                int hitboxHeight = 30;
                return new Rectangle((int)Position.X, (int)Position.Y, hitboxWidth, hitboxHeight);
            }
        }

        public void OnCollision(ICollidable other)
        {
            if (other is Slime slime)
            {
                // Handle collision without affecting movement
                this.TakeDamage(1);
            }
        }
    }
}
