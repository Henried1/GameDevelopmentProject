using GameProject.Animations;
using GameProject.Characters.Interfaces;
using GameProject.Characters.Player;
using GameProject.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace GameProject.Characters.Enemies
{
    public class FireSpirit : Enemy
    {
        private List<Fireball> _fireballs;
        private double _fireballCooldown;
        private double _fireballCooldownTime = 2.0; 
        private Hero _hero;

        public FireSpirit(Vector2 startPosition, int healthPoints, double fps, GameManager gameManager)
           : base(startPosition, healthPoints, fps, gameManager) 
        {
            _fireballs = new List<Fireball>();
            _fireballCooldown = 0;
        }

        public void SetHeroReference(Hero hero)
        {
            _hero = hero;
        }

        public override void LoadContent(ContentManager content)
        {
            _content = content;
            Texture2D idleTexture = content.Load<Texture2D>("Enemies/FireSpirit/Idle");
            _idleAnimation = new Animation(idleTexture, 6);

            Texture2D walkTexture = content.Load<Texture2D>("Enemies/FireSpirit/Walk");
            _walkAnimation = new Animation(walkTexture, 7);
        }

        public override void Update(GameTime gameTime, KeyboardState keyboardState, MouseState mouseState, int[,] tileMap, int tileWidth, int tileHeight, int screenWidth, int screenHeight)
        {
            base.Update(gameTime, keyboardState, mouseState, tileMap, tileWidth, tileHeight, screenWidth, screenHeight);

            if (!IsAlive())
            {
                return;
            }
            _fireballCooldown -= gameTime.ElapsedGameTime.TotalSeconds;

           
            if (_hero != null && IsPlayerInRange() && _fireballCooldown <= 0)
            {
                ShootFireball();
                _fireballCooldown = _fireballCooldownTime;
            }

            foreach (var fireball in _fireballs)
            {
                fireball.Update(gameTime);

                if (fireball.CheckCollision(_hero.Hitbox))
                {
                    _hero.TakeDamage(fireball.Damage);
                    fireball.MarkForRemoval = true; 
                }
            }

            _fireballs.RemoveAll(f => f.IsOffScreen(screenWidth, screenHeight) || f.MarkForRemoval);
        }
        private void ShootFireball()
        {
            Vector2 direction = _hero.Position - _position;
            direction.Normalize();

            
            float fireballOffset = 20f;
            Vector2 fireballPosition = _position + direction * fireballOffset;

            fireballPosition.Y = _position.Y + 60; 

            var graphicsDeviceService = (IGraphicsDeviceService)_content.ServiceProvider.GetService(typeof(IGraphicsDeviceService));
            Fireball fireball = new Fireball(fireballPosition, direction, graphicsDeviceService.GraphicsDevice);
            fireball.LoadContent(_content);
            _fireballs.Add(fireball);

        }



        private bool IsPlayerInRange()
        {
            if (_hero == null)
                return false;

            float distance = Vector2.Distance(_position, _hero.Position);
            float attackRange = 300f; 

            return distance <= attackRange;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            foreach (var fireball in _fireballs)
            {
                fireball.Draw(spriteBatch);
            }
        }

        public override void OnCollision(ICollidable other)
        {
            if (other is Hero hero)
            {
                hero.TakeDamage(Damage);
            }
        }
        public bool IsAlive()
        {
            return _healthPoints > 0;
        }
    }
}
