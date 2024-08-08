using GameProject.Characters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameProject.Mechanics
{
    public class PlayerMovement
    {
        public Vector2 Position { get; set; }
        public float Speed { get; private set; }
        public bool IsMoving { get; private set; }
        public bool IsAttacking { get; private set; }
        public bool IsJumping { get; private set; }
        public SpriteEffects SpriteEffect { get; private set; }

        private float _jumpSpeed;
        private float _gravity;
        private float _verticalVelocity;
        private bool _isFalling;
        private int _playerHeight;

        public PlayerMovement(Vector2 startPosition, float speed, int playerHeight)
        {
            Position = startPosition;
            Speed = speed;
            IsMoving = false;
            IsAttacking = false;
            IsJumping = false;
            _isFalling = false;
            SpriteEffect = SpriteEffects.None;
            _jumpSpeed = 10f;
            _gravity = 0.5f;
            _verticalVelocity = 0f;
            _playerHeight = playerHeight;
        }

        public void Update(KeyboardState keyboardState, MouseState mouseState, int[,] tileMap, int tileWidth, int tileHeight, int screenWidth, int screenHeight)
        {
            IsMoving = false;
            IsAttacking = mouseState.LeftButton == ButtonState.Pressed;
            Vector2 newPosition = Position;

            // Horizontal movement
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                newPosition.X -= Speed;
                IsMoving = true;
                SpriteEffect = SpriteEffects.FlipHorizontally;
            }
            else if (keyboardState.IsKeyDown(Keys.Right))
            {
                newPosition.X += Speed;
                IsMoving = true;
                SpriteEffect = SpriteEffects.None;
            }

            // Jumping
            if (keyboardState.IsKeyDown(Keys.Up) && !IsJumping && !_isFalling)
            {
                IsJumping = true;
                _verticalVelocity = -_jumpSpeed;
            }

            // Vertical movement
            if (IsJumping || _isFalling)
            {
                _verticalVelocity += _gravity;
                newPosition.Y += _verticalVelocity;

                // Check collision with ground tiles (tiles with value 1)
                if (IsCollidingWithGround(newPosition, tileMap, tileWidth, tileHeight))
                {
                    newPosition.Y = SnapToGround(newPosition.Y, tileMap, tileWidth, tileHeight);
                    IsJumping = false;
                    _isFalling = false;
                    _verticalVelocity = 0f;
                }
                else
                {
                    _isFalling = true;
                }
            }
            else
            {
                Vector2 belowPosition = new Vector2(Position.X, Position.Y + 1);
                if (!IsCollidingWithGround(belowPosition, tileMap, tileWidth, tileHeight))
                {
                    _isFalling = true;
                }
            }

            // Apply the new position
            if (!IsCollidingWithScreenBorders(newPosition, screenWidth, screenHeight, tileWidth, tileHeight))
            {
                Position = newPosition;
            }
        }

        private bool IsCollidingWithGround(Vector2 position, int[,] tileMap, int tileWidth, int tileHeight)
        {
            int tileX = (int)(position.X / tileWidth);
            int tileY = (int)((position.Y + _playerHeight) / tileHeight);

            if (tileX < 0 || tileX >= tileMap.GetLength(1) || tileY < 0 || tileY >= tileMap.GetLength(0))
            {
                return false;
            }

            return tileMap[tileY, tileX] == 1;
        }

        private float SnapToGround(float yPosition, int[,] tileMap, int tileWidth, int tileHeight)
        {
            int tileY = (int)((yPosition + _playerHeight) / tileHeight);

            while (tileY < tileMap.GetLength(0) && tileMap[tileY, (int)(Position.X / tileWidth)] != 1)
            {
                tileY++;
            }

            return tileY * tileHeight - _playerHeight;
        }

        private bool IsCollidingWithScreenBorders(Vector2 position, int screenWidth, int screenHeight, int tileWidth, int tileHeight)
        {
            return position.X < 0 || position.X > screenWidth - tileWidth || position.Y < 0 || position.Y > screenHeight - tileHeight;
        }
    }
}
