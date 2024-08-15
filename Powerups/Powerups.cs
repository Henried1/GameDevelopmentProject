using GameProject.Characters.Interfaces;
using GameProject.Characters.Player;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace GameProject.Powerups
{
    public abstract class Powerup : ICollidable
    {
        protected Texture2D _texture;
        protected Vector2 _position;
        protected float _scale = 0.5f; 

        public Powerup(Vector2 position)
        {
            _position = position;
        }

        public void LoadContent(ContentManager content)
        {
            _texture = content.Load<Texture2D>(GetTexturePath());
            Debug.WriteLine($"Loaded texture from {GetTexturePath()}"); // Debug statement
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _position, null, Color.White, 0f, Vector2.Zero, _scale, SpriteEffects.None, 0f);
        }

        public Vector2 Position => _position;

        public int Width => (int)(_texture?.Width * _scale ?? 0);

        public int Height => (int)(_texture?.Height * _scale ?? 0);

        public Rectangle Hitbox => new Rectangle((int)Position.X, (int)Position.Y, Width, Height);

        public void OnCollision(ICollidable other)
        {
        }

        public abstract void ApplyEffect(Hero player);

        protected abstract string GetTexturePath();
    }

}
