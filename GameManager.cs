using GameProject.Animations;
using GameProject.Characters;
using GameProject.Characters.Player;
using GameProject.Map;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using GameProject.Characters.Interfaces;
using GameProject.Characters.Enemies;
using System.Collections.Generic;

namespace GameProject.Managers
{
    public class GameManager
    {
        private ICharacter _hero;
        private List<Enemy> _enemies; 
        private readonly ContentManager _content;
        private readonly GraphicsDeviceManager _graphics;
        private TileMap _tileMap;
        private bool _isGameOver;

        private Animation _heartsAnimation;

        public GameManager(ContentManager content, GraphicsDeviceManager graphics)
        {
            _content = content;
            _graphics = graphics;
            _isGameOver = false;
            _enemies = new List<Enemy>(); 
        }

        public void InitializeHero(TileMap tileMap)
        {
            _tileMap = tileMap;

            _hero = new Hero(Vector2.Zero, 2f);
            _hero.LoadContent(_content);

            Vector2 heroPosition = _tileMap.FindGroundPosition(_hero.Height);
            _hero.Position = heroPosition;

            if (_hero is Hero hero)
            {
                hero.OnDamageTaken += OnHeroDamageTaken;
            }

            var heartsTexture = _content.Load<Texture2D>("Hearts");
            _heartsAnimation = new Animation(heartsTexture, 3); // 3 frames
        }

        public void InitializeEnemies()
        {
           
            var slime = new Slime(Vector2.Zero);
            slime.LoadContent(_content);

            Vector2 slimePosition = _tileMap.FindGroundPosition(slime.Height);
            slimePosition.X += 200;
            slime.Position = slimePosition;
            var orc = new Orc(Vector2.Zero);

            orc.LoadContent(_content);
            Vector2 orcPosition = _tileMap.FindGroundPosition(orc.Height);
            orcPosition.X += 500;
            orc.Position = orcPosition;


            _enemies.Add(slime);
            _enemies.Add(orc);



        }

        private void OnHeroDamageTaken()
        {
            if (_hero is Hero hero)
            {
                float healthPercentage = hero.HealthPercentage;

                if (healthPercentage > 0.5f)
                {
                    _heartsAnimation.CurrentFrame = 0; // Full health
                }
                else if (healthPercentage > 0)
                {
                    _heartsAnimation.CurrentFrame = 1; // Half health
                }
                else
                {
                    _heartsAnimation.CurrentFrame = 2; // Zero health
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            if (_isGameOver)
            {
                return;
            }

            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();
            int tileWidth = _tileMap.TileWidth;
            int tileHeight = _tileMap.TileHeight;
            int screenWidth = _graphics.PreferredBackBufferWidth;
            int screenHeight = _graphics.PreferredBackBufferHeight;

            _hero.Update(gameTime, keyboardState, mouseState, _tileMap.GetTileMapArray(), tileWidth, tileHeight, screenWidth, screenHeight);

            foreach (var enemy in _enemies)
            {
                enemy.Update(gameTime, keyboardState, mouseState, _tileMap.GetTileMapArray(), tileWidth, tileHeight, screenWidth, screenHeight);
            }

            if (_hero.IsDead)
            {
                _isGameOver = true;
            }

            if (_hero is ICollidable heroCollidable)
            {
                foreach (var enemy in _enemies)
                {
                    if (enemy is ICollidable enemyCollidable && !enemy.IsDead && heroCollidable.Hitbox.Intersects(enemyCollidable.Hitbox))
                    {
                        heroCollidable.OnCollision(enemyCollidable);
                        enemyCollidable.OnCollision(heroCollidable);
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _tileMap.Draw(spriteBatch);

            _hero.Draw(spriteBatch);

            foreach (var enemy in _enemies)
            {
                enemy.Draw(spriteBatch);
            }

            float heartScale = 0.5f; //scale of heart
            _heartsAnimation.Draw(spriteBatch, new Vector2(0, 0), SpriteEffects.None, heartScale);
        }
    }
}
