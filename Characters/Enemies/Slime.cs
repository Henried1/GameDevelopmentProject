using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameProject.Animations;
using GameProject.Characters.Interfaces;
using GameProject.Characters.Player;

namespace GameProject.Characters
{
    public class Slime : ICharacter,ICollidable
    {
        private Animation _idleAnimation;
        private Animation _walkAnimation;
        private Vector2 _position;
        private bool _isWalking;
        private bool _isDead;
        private int _healthPoints;
        private double _timeCounter;
        private double _fps;
        private double _timePerFrame;

        public int Height => _walkAnimation?.FrameHeight ?? 0;
        public int Width => _walkAnimation?.FrameWidth ?? 0;
        public Slime(Vector2 startPosition)
        {
            _position = startPosition;
            _isWalking = false;
            _isDead = false;
            _healthPoints = 50; // Initial health points
            _fps = 10.0;
            _timePerFrame = 1.0 / _fps;
        }

        public void LoadContent(ContentManager content)
        {
            var idleTexture = content.Load<Texture2D>("Enemies/Slime/Idle");
            var walkTexture = content.Load<Texture2D>("Enemies/Slime/Walk");

            _idleAnimation = new Animation(idleTexture, 8);
            _walkAnimation = new Animation(walkTexture, 8);
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState, MouseState mouseState, int[,] tileMap, int tileWidth, int tileHeight, int screenWidth, int screenHeight)
        {
            _timeCounter += gameTime.ElapsedGameTime.TotalSeconds;

            if (_timeCounter >= _timePerFrame)
            {
                _timeCounter -= _timePerFrame;
            }

            // Simple AI for walking back and forth
            if (_isWalking)
            {
                _position.X += 1; // Move right
                if (_position.X > screenWidth - _walkAnimation.FrameWidth)
                {
                    _isWalking = false;
                }
            }
            else
            {
                _position.X -= 1; // Move left
                if (_position.X < 0)
                {
                    _isWalking = true;
                }
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

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_isWalking)
            {
                _walkAnimation.Draw(spriteBatch, _position, SpriteEffects.None);
            }
            else
            {
                _idleAnimation.Draw(spriteBatch, _position, SpriteEffects.None);
            }
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
                // colission range
                int boundingBoxWidth = 50;
                int boundingBoxHeight = 5; 
                return new Rectangle((int)Position.X, (int)Position.Y, boundingBoxWidth, boundingBoxHeight);
            }
        }
        public void OnCollision(ICollidable other)
        {
            if (other is Hero hero)
            {
                Rectangle intersection = Rectangle.Intersect(this.Hitbox, hero.Hitbox);
                if (intersection.Width > intersection.Height)
                {
                    // Vertical collision
                    if (this.Position.Y < hero.Position.Y)
                    {
                        this.Position = new Vector2(this.Position.X, this.Position.Y - intersection.Height);
                    }
                    else
                    {
                        this.Position = new Vector2(this.Position.X, this.Position.Y + intersection.Height);
                    }
                }
                else
                {
                    // Horizontal collision
                    if (this.Position.X < hero.Position.X)
                    {
                        this.Position = new Vector2(this.Position.X - intersection.Width, this.Position.Y);
                    }
                    else
                    {
                        this.Position = new Vector2(this.Position.X + intersection.Width, this.Position.Y);
                    }
                }

                // Apply damage or other effects
                this.TakeDamage(1);
            }
        }

        public Vector2 Position
        {
            get => _position;
            set => _position = value;
        }

        public bool IsDead => _isDead;
    }
}
