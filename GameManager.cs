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
using GameProject.Powerups;
using System.Linq;
using System.Diagnostics;
using GameProject.Factories;

namespace GameProject.Managers
{
    public class GameManager
    {
        private ICharacter _hero;
        private List<Enemy> _enemies;
        private readonly ContentManager _content;
        private readonly GraphicsDeviceManager _graphics;
        private TileMap _tileMap;
        private List<Powerup> _powerups = new List<Powerup>();
        private readonly Game1 _game;

        private Animation _heartsAnimation;
        public Animation HeartsAnimation => _heartsAnimation;
        public bool IsGameOver { get; private set; }
        public bool IsVictory { get; private set; }

        public GameManager(ContentManager content, GraphicsDeviceManager graphics, Game1 game)
        {
            _content = content;
            _graphics = graphics;
            IsGameOver = false; // Initialize IsGameOver
            IsVictory = false;  // Initialize IsVictory
            _enemies = new List<Enemy>();
            _game = game;
        }

        public void InitializeHero(TileMap tileMap)
        {
            _tileMap = tileMap;

            _hero = new Hero(Vector2.Zero,2f);
            _hero.LoadContent(_content);

            Vector2 heroPosition = _tileMap.FindGroundPosition(_hero.Height);
            heroPosition.X = 0;
            _hero.Position = heroPosition;

            if (_hero is Hero hero)
            {
                hero.OnDamageTaken += OnHeroDamageTaken;
            }

            var heartsTexture = _content.Load<Texture2D>("Hearts");
            _heartsAnimation = new Animation(heartsTexture, 3); // 3 frames
        }

        public void InitializeEnemies(int level)
        {
            _enemies.Clear();

            if (level == 1)
            {
                var slime = CreateAndPositionEnemy("Slime", 200, this);
                var orc = CreateAndPositionEnemy("Orc", 500, this);
                var fireSpirit = CreateAndPositionEnemy("FireSpirit", 800, this);

                if (_hero is Hero hero)
                {
                    orc.SetHeroReference(hero);
                    fireSpirit.SetHeroReference(hero);
                }

                _enemies.Add(slime);
                _enemies.Add(orc);
                _enemies.Add(fireSpirit);
            }
            else if (level == 2)
            {
                var slime = CreateAndPositionEnemy("Slime", 50, this);
                var orc = CreateAndPositionEnemy("Orc", 100, this);
                var fireSpirit = CreateAndPositionEnemy("FireSpirit", 700, this);

                if (_hero is Hero hero)
                {
                    orc.SetHeroReference(hero);
                    fireSpirit.SetHeroReference(hero);
                }

                _enemies.Add(slime);
                _enemies.Add(orc);
                _enemies.Add(fireSpirit);
            }
        }

        private Enemy CreateAndPositionEnemy(string enemyType, float xOffset, GameManager gameManager)
        {
            var enemy = EnemyFactory.CreateEnemy(enemyType, Vector2.Zero, gameManager);
            enemy.LoadContent(_content);

            Vector2 enemyPosition = _tileMap.FindGroundPosition(enemy.Height);
            enemyPosition.X += xOffset;
            enemy.Position = enemyPosition;

            return enemy;
        }

        private void OnHeroDamageTaken()
        {
            if (_hero is Hero hero)
            {
                float healthPercentage = hero.HealthPercentage;

                if (healthPercentage > 0.5f)
                {
                    _heartsAnimation.CurrentFrame = 0;
                }
                else if (healthPercentage > 0)
                {
                    _heartsAnimation.CurrentFrame = 1;
                }
                else
                {
                    _heartsAnimation.CurrentFrame = 2;
                }
            }
        }

        public void AddPowerup(Powerup powerup)
        {
            powerup.LoadContent(_content);
            _powerups.Add(powerup);
            Debug.WriteLine($"Powerup added at position {powerup.Position}.");
        }

        public void ClearPowerups()
        {
            _powerups.Clear();
        }

        public void Update(GameTime gameTime)
        {
            if (IsGameOver || IsVictory)
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
                IsGameOver = true;
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

            if (_hero is Hero hero)
            {
                foreach (var powerup in _powerups.ToList())
                {
                    if (hero.Hitbox.Intersects(powerup.Hitbox))
                    {
                        powerup.ApplyEffect(hero);
                        if (powerup is LifePowerup)
                        {
                            hero.RestoreFullHealth(_heartsAnimation);
                        }
                        _powerups.Remove(powerup);
                    }
                }
                if (AreAllEnemiesDefeated() && _tileMap.CurrentLevel == 2)
                {
                    IsVictory = true;
                }

                if (AreAllEnemiesDefeated())
                {
                    _game.LoadNextLevel();
                }
            }
        }

        private bool AreAllEnemiesDefeated()
        {
            return _enemies.All(enemy => enemy.IsDead);
        }

        public void DrawHitboxes(SpriteBatch spriteBatch)
        {
            _tileMap.DrawHitboxes(spriteBatch);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _tileMap.Draw(spriteBatch);

            _hero.Draw(spriteBatch);

            foreach (var enemy in _enemies)
            {
                enemy.Draw(spriteBatch);
            }

            foreach (var powerup in _powerups)
            {
                powerup.Draw(spriteBatch);
            }

            float heartScale = 0.5f;
            _heartsAnimation.Draw(spriteBatch, new Vector2(0, 0), SpriteEffects.None, heartScale);
        }

        public void ResetGameState()
        {
            IsGameOver = false;
            IsVictory = false;
        }
    }
}
