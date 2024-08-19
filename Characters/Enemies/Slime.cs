﻿using Microsoft.Xna.Framework;
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
        public Slime(Vector2 startPosition, GameManager gameManager)
           : base(startPosition, 10, 10.0, gameManager)
        {
            _currentState = EnemyState.Walking; // Ensure initial state is Walking
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
            base.Update(gameTime, keyboardState, mouseState, tileMap, tileWidth, tileHeight, screenWidth, screenHeight);

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
