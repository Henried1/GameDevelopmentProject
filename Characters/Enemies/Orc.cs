using GameProject.Animations;
using GameProject.Characters.Interfaces;
using GameProject.Characters.Player;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GameProject.Characters.Enemies
{
    public class Orc : Enemy
    {
        public Orc(Vector2 startPosition) : base(startPosition, 100, 10)
        {
        }

        public override void LoadContent(ContentManager content)
        {
            var idleTexture = content.Load<Texture2D>("Enemies/Orc/Idle");
            var walkTexture = content.Load<Texture2D>("Enemies/Orc/Walk");

            _idleAnimation = new Animation(idleTexture, 5);
            _walkAnimation = new Animation(walkTexture, 7);
        }

        public override void OnCollision(ICollidable other)
        {
            if (other is Hero hero)
            {
                if (!this.IsDead)
                {
                    hero.TakeDamage(1);

                    if (hero.AttackHitbox.Intersects(this.Hitbox))
                    {
                        this.TakeDamage(10);
                    }
                }
            }
        }
    }
}
