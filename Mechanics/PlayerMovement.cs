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
        public bool IsMovingRight { get; private set; }
        public bool IsAttacking { get; private set; }
        public bool IsJumping { get; private set; }
        public SpriteEffects SpriteEffect { get; private set; }

        private float _jumpSpeed;
        private float _gravity;
        private float _verticalVelocity;
        private float _horizontalVelocity;
        private bool _isFalling;
        private int _playerHeight;
        private int _playerWidth;

        private float _acceleration;
        private float _deceleration;
        private float _currentSpeed;

        int offsetX = 0;
        int offsetY = 0;
        int offsetWidth = 30;
        int offsetHeight = 0;

        public PlayerMovement(Vector2 startPosition, float speed, int playerHeight, int playerWidth)
        {
            Position = startPosition;
            Speed = speed;
            IsMoving = false;
            IsMovingRight = false;
            IsAttacking = false;
            IsJumping = false;
            _isFalling = false;
            SpriteEffect = SpriteEffects.None;
            _jumpSpeed = 17f;
            _gravity = 0.5f;
            _verticalVelocity = 0f;
            _horizontalVelocity = 0f;
            _playerHeight = playerHeight;
            _playerWidth = playerWidth;

            _acceleration = 0.1f; 
            _deceleration = 1.0f; 
            _currentSpeed = 0f;
        }

        public void Update(KeyboardState keyboardState, MouseState mouseState, int[,] tileMap, int tileWidth, int tileHeight, int screenWidth, int screenHeight)
        {
            IsMoving = false;
            IsAttacking = mouseState.LeftButton == ButtonState.Pressed;
            Vector2 newPosition = Position;

            // Horizontal movement
            Vector2 horizontalPosition = newPosition;
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                _currentSpeed -= _acceleration;
                if (_currentSpeed < -Speed)
                {
                    _currentSpeed = -Speed;
                }
                horizontalPosition.X += _currentSpeed;
                IsMoving = true;
                IsMovingRight = false;
                SpriteEffect = SpriteEffects.FlipHorizontally;
            }
            else if (keyboardState.IsKeyDown(Keys.Right))
            {
                _currentSpeed += _acceleration;
                if (_currentSpeed > Speed)
                {
                    _currentSpeed = Speed;
                }
                horizontalPosition.X += _currentSpeed;
                IsMoving = true;
                IsMovingRight = true;
                SpriteEffect = SpriteEffects.None;
            }
            else
            {
                if (_currentSpeed > 0)
                {
                    _currentSpeed -= _deceleration;
                    if (_currentSpeed < 0)
                    {
                        _currentSpeed = 0;
                    }
                }
                else if (_currentSpeed < 0)
                {
                    _currentSpeed += _deceleration;
                    if (_currentSpeed > 0)
                    {
                        _currentSpeed = 0;
                    }
                }
                horizontalPosition.X += _currentSpeed;
            }


            if (!IsCollidingWithTile(new Vector2(horizontalPosition.X, Position.Y), tileMap, tileWidth, tileHeight))
            {
                newPosition.X = horizontalPosition.X;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Horizontal Collision Detected");
            }

            // Vertical movement (Jumping/Falling)
            if (keyboardState.IsKeyDown(Keys.Up) && !IsJumping && !_isFalling)
            {
                IsJumping = true;
                _verticalVelocity = -_jumpSpeed;
            }

            if (IsJumping || _isFalling)
            {
                _verticalVelocity += _gravity;
                newPosition.Y += _verticalVelocity;

                if (IsCollidingWithTile(new Vector2(Position.X, newPosition.Y), tileMap, tileWidth, tileHeight))
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
                if (!IsCollidingWithTile(belowPosition, tileMap, tileWidth, tileHeight))
                {
                    _isFalling = true;
                }
            }

            if (!IsCollidingWithScreenBorders(newPosition, screenWidth, screenHeight, tileWidth, tileHeight))
            {
                Position = newPosition;
            }
        }

        private bool IsCollidingWithTile(Vector2 position, int[,] tileMap, int tileWidth, int tileHeight)
        {
            int leftTileX = (int)(position.X / tileWidth);
            int rightTileX = (int)((position.X + _playerWidth - 1) / tileWidth);
            int topTileY = (int)(position.Y / tileHeight);
            int bottomTileY = (int)((position.Y + _playerHeight - 1) / tileHeight);

            if (leftTileX < 0 || rightTileX >= tileMap.GetLength(1) || topTileY < 0 || bottomTileY >= tileMap.GetLength(0))
            {
                return false;
            }

            for (int x = leftTileX; x <= rightTileX; x++)
            {
                for (int y = topTileY; y <= bottomTileY; y++)
                {
                    if (tileMap[y, x] != 0)
                    {
                        Rectangle tileHitbox = new Rectangle(x * tileWidth, y * tileHeight, tileWidth, tileHeight);
                        Rectangle playerHitbox = new Rectangle((int)position.X + offsetX, (int)position.Y + offsetY, _playerWidth - offsetWidth, _playerHeight - offsetHeight);

                        if (playerHitbox.Intersects(tileHitbox))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private float SnapToGround(float yPosition, int[,] tileMap, int tileWidth, int tileHeight)
        {
            int tileY = (int)((yPosition + _playerHeight) / tileHeight);

            while (tileY < tileMap.GetLength(0) && tileMap[tileY, (int)(Position.X / tileWidth)] != 1 && tileMap[tileY, (int)(Position.X / tileWidth)] != 2)
            {
                tileY++;
            }

            return tileY * tileHeight - _playerHeight;
        }

        private bool IsCollidingWithScreenBorders(Vector2 position, int screenWidth, int screenHeight, int tileWidth, int tileHeight)
        {
            return position.X < 0 || position.X > screenWidth - tileWidth || position.Y < 0 || position.Y > screenHeight - tileHeight;
        }

        public void SetIsMoving(bool value)
        {
            IsMoving = value;
        }
        public void SetSpeed(float speed)
        {
            Speed = speed;
        }
    }
}
