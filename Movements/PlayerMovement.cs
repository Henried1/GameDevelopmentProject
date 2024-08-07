using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameProject.Movements
{
    public class PlayerMovement
    {
        public Vector2 Position { get; set; } 

        public float Speed { get; private set; }
        public bool IsMoving { get; private set; }
        public SpriteEffects SpriteEffect { get; private set; }

        public PlayerMovement(Vector2 startPosition, float speed)
        {
            Position = startPosition;
            Speed = speed;
            IsMoving = false;
            SpriteEffect = SpriteEffects.None;
        }

        public void Update(KeyboardState keyboardState, int[,] tileMap, int tileWidth, int tileHeight, int screenWidth, int screenHeight)
        {
            IsMoving = false;
            Vector2 newPosition = Position;

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

            if (!IsCollidingWithTileMap(newPosition, tileMap, tileWidth, tileHeight) && !IsCollidingWithScreenBorders(newPosition, screenWidth, screenHeight, tileWidth, tileHeight))
            {
                Position = newPosition;
            }
        }

        private bool IsCollidingWithTileMap(Vector2 position, int[,] tileMap, int tileWidth, int tileHeight)
        {
            int tileX = (int)(position.X / tileWidth);
            int tileY = (int)(position.Y / tileHeight);

            if (tileX < 0 || tileX >= tileMap.GetLength(1) || tileY < 0 || tileY >= tileMap.GetLength(0))
            {
                return true;
            }

            return tileMap[tileY, tileX] == 1;
        }

        private bool IsCollidingWithScreenBorders(Vector2 position, int screenWidth, int screenHeight, int tileWidth, int tileHeight)
        {
            return position.X < 0 || position.X > screenWidth - tileWidth || position.Y < 0 || position.Y > screenHeight - tileHeight;
        }
    }
}
