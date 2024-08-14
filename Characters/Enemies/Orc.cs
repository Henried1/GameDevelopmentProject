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

        public override Rectangle Hitbox
        {
            get
            {
                int hitboxWidth = (int)(_walkAnimation.FrameWidth * 0.45f);
                int hitboxHeight = (int)(_walkAnimation.FrameHeight * 0.65f);
                int hitboxX = (int)(_position.X + (_walkAnimation.FrameWidth - hitboxWidth) / 2);
                int hitboxY = (int)(_position.Y + (_walkAnimation.FrameHeight - hitboxHeight));

                return new Rectangle(hitboxX, hitboxY, hitboxWidth, hitboxHeight);
            }
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
