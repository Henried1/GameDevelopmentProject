using GameProject.Animations;
using GameProject.Characters.Interfaces;
using GameProject.Characters.Player;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace GameProject.Characters.Enemies
{
    public class Orc : Enemy
    {
        protected Animation _attackAnimation;
        private double _attackCooldown;
        private double _attackCooldownTime = 5.0;
        private Hero _hero;

        public Orc(Vector2 startPosition) : base(startPosition, 100, 10)
        {
            _attackCooldown = _attackCooldownTime;
        }

        public void SetHeroReference(Hero hero)
        {
            _hero = hero;
        }

        public override void LoadContent(ContentManager content)
        {
            var idleTexture = content.Load<Texture2D>("Enemies/Orc/Idle");
            var walkTexture = content.Load<Texture2D>("Enemies/Orc/Walk");
            var attackTexture = content.Load<Texture2D>("Enemies/Orc/Attack_1");

            _idleAnimation = new Animation(idleTexture, 5);
            _walkAnimation = new Animation(walkTexture, 7);
            _attackAnimation = new Animation(attackTexture, 4, 0.55); // Slower attack animation
        }

        public override void Update(GameTime gameTime, KeyboardState keyboardState, MouseState mouseState, int[,] tileMap, int tileWidth, int tileHeight, int screenWidth, int screenHeight)
        {
            if (_isDead)
            {
                return;
            }

            _attackCooldown -= gameTime.ElapsedGameTime.TotalSeconds;

            _timeCounter += gameTime.ElapsedGameTime.TotalSeconds;
            if (_timeCounter >= _timePerFrame)
            {
                _timeCounter -= _timePerFrame;
            }

            switch (_currentState)
            {
                case EnemyState.Walking:
                    if (IsPlayerInRange() && _attackCooldown <= 0)
                    {
                        _currentState = EnemyState.Attacking;
                        _attackAnimation.Reset();
                        FaceHero();
                    }
                    else
                    {
                        _walkTimeCounter += gameTime.ElapsedGameTime.TotalSeconds;

                        if (_walkTimeCounter >= _walkDuration)
                        {
                            _currentState = EnemyState.Idle;
                            _walkTimeCounter = 0;
                            _idleTimeCounter = 0;
                        }

                        _position.X += _isMovingRight ? 1 : -1;
                    }
                    break;

                case EnemyState.Idle:
                    _idleTimeCounter += gameTime.ElapsedGameTime.TotalSeconds;

                    if (_idleTimeCounter >= _idleDuration)
                    {
                        _currentState = EnemyState.Walking;
                        _isMovingRight = !_isMovingRight;
                        _walkTimeCounter = 0;
                    }
                    break;

                case EnemyState.Attacking:
                    _attackAnimation.Update(gameTime);

                    if (_attackAnimation.IsComplete)
                    {
                        _currentState = EnemyState.Idle;
                        _attackCooldown = _attackCooldownTime;
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

            // Update the animation based on the current state
            switch (_currentState)
            {
                case EnemyState.Walking:
                    _walkAnimation.Update(gameTime);
                    break;
                case EnemyState.Idle:
                    _idleAnimation.Update(gameTime);
                    break;
                case EnemyState.Attacking:
                    _attackAnimation.Update(gameTime);
                    break;
            }
        }

        private void FaceHero()
        {
            if (_hero != null)
            {
                _isMovingRight = _hero.Position.X > _position.X;
            }
        }

        private bool IsPlayerInRange()
        {
            if (_hero == null)
                return false;

            float distance = Vector2.Distance(_position, _hero.Position);

            float attackRange = 60f;

            return distance <= attackRange;
        }

        public override void OnCollision(ICollidable other)
        {
            if (other is Hero hero && _currentState == EnemyState.Attacking && _attackCooldown <= 0)
            {
                if (this.Hitbox.Intersects(hero.Hitbox))
                {
                    hero.TakeDamage(10);
                    _attackCooldown = _attackCooldownTime; // Reset the cooldown
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (_isDead)
            {
                return;
            }

            SpriteEffects spriteEffects = _isMovingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            switch (_currentState)
            {
                case EnemyState.Walking:
                    _walkAnimation.Draw(spriteBatch, _position, spriteEffects);
                    break;
                case EnemyState.Idle:
                    _idleAnimation.Draw(spriteBatch, _position, spriteEffects);
                    break;
                case EnemyState.Attacking:
                    _attackAnimation.Draw(spriteBatch, _position, spriteEffects);
                    break;
            }

            // Visualize the hitbox
            Texture2D hitboxTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            hitboxTexture.SetData(new[] { Color.Red });
            spriteBatch.Draw(hitboxTexture, Hitbox, Color.Red * 0.5f);
        }

        public override Rectangle Hitbox
        {
            get
            {
                int hitboxWidth = (int)(_walkAnimation.FrameWidth * 0.45f);
                int hitboxHeight = (int)(_walkAnimation.FrameHeight * 0.66f);
                int hitboxX = (int)(_position.X + (_walkAnimation.FrameWidth - hitboxWidth) / 2);
                int hitboxY = (int)(_position.Y + (_walkAnimation.FrameHeight - hitboxHeight));

                return new Rectangle(hitboxX, hitboxY, hitboxWidth, hitboxHeight);
            }
        }
    }
}
