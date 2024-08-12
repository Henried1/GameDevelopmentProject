using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameProject.Animations;
using GameProject.Characters.Interfaces;
using GameProject.Characters.Player;

namespace GameProject.Characters
{
    public class Slime : ICharacter, ICollidable
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

            // Check for ground collision
            if (!IsCollidingWithGround(_position, tileMap, tileWidth, tileHeight))
            {
                _position.Y += 1; // Apply gravity
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

        private bool IsCollidingWithGround(Vector2 position, int[,] tileMap, int tileWidth, int tileHeight)
        {
            int tileX = (int)(position.X / tileWidth);
            int tileY = (int)((position.Y + Height) / tileHeight);

            if (tileX < 0 || tileX >= tileMap.GetLength(1) || tileY < 0 || tileY >= tileMap.GetLength(0))
            {
                return false;
            }

            return tileMap[tileY, tileX] == 1;
        }

        private float SnapToGround(float yPosition, int[,] tileMap, int tileWidth, int tileHeight)
        {
            int tileY = (int)((yPosition + Height) / tileHeight);

            while (tileY < tileMap.GetLength(0) && tileMap[tileY, (int)(_position.X / tileWidth)] != 1)
            {
                tileY++;
            }

            return tileY * tileHeight - Height;
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
