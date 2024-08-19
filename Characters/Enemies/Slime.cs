using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GameProject.Animations;
using GameProject.Characters.Interfaces;
using GameProject.Characters.Player;
using GameProject.Managers;
using Microsoft.Xna.Framework.Input;

namespace GameProject.Characters.Enemies
{
    public class Slime : Enemy
    {
        private float _slowDownFactor = 0.01f; 
        private float _originalHeroSpeed;

        public float Speed { get; set; } = 5.0f; 

        public Slime(Vector2 startPosition, GameManager gameManager)
           : base(startPosition, 10, 10.0, gameManager)
        {
            _currentState = EnemyState.Walking; 
        }

        public override void LoadContent(ContentManager content)
        {
            var idleTexture = content.Load<Texture2D>("Enemies/Slime/Idle");
            var walkTexture = content.Load<Texture2D>("Enemies/Slime/Walk");

            _idleAnimation = new Animation(idleTexture, 8);
            _walkAnimation = new Animation(walkTexture, 8);
        }

        public override void Update(GameTime gameTime, KeyboardState keyboardState, MouseState mouseState, int[,] tileMap, int tileWidth, int tileHeight, int screenWidth, int screenHeight)
        {

            var hero = _gameManager.Hero;
            if (hero != null && !this.IsDead)
            {
                Vector2 direction = hero.Position - this.Position;
                direction.Normalize();
                this.Position += direction * (float)(this.Speed * gameTime.ElapsedGameTime.TotalSeconds);
            }
        }

        public override void OnCollision(ICollidable other)
        {
            if (other is Hero hero)
            {
                if (!this.IsDead)
                {
                    hero.TakeDamage(0.5);

                    if (hero.AttackHitbox.Intersects(this.Hitbox))
                    {
                        this.TakeDamage(10);
                    }

                    _originalHeroSpeed = hero.Movement.Speed;
                    hero.Movement.SetSpeed(hero.Movement.Speed * _slowDownFactor);
                }
            }
        }
    }

}
