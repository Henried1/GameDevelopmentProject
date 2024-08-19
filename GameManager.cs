// GameManager.cs
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
using GameProject.Managers;

namespace GameProject.Managers
{
    public class GameManager
    {
        private readonly ICharacter _hero;
        private readonly List<Enemy> _enemies;
        private readonly ContentManager _content;
        private readonly GraphicsDeviceManager _graphics;
        private readonly TileMap _tileMap;
        private readonly PowerupManager _powerupManager;
        private readonly EnemyManager _enemyManager;
        private readonly HeroManager _heroManager;
        private readonly Game1 _game; // Reference to Game1

        private Animation _heartsAnimation;
        public Animation HeartsAnimation => _heartsAnimation;

        public GameManager(ContentManager content, GraphicsDeviceManager graphics, Game1 game, TileMap tileMap)
        {
            _content = content;
            _graphics = graphics;
            _game = game; // Initialize the reference
            _tileMap = tileMap;

            _heroManager = new HeroManager(content, tileMap);
            _hero = _heroManager.InitializeHero();

            _enemyManager = new EnemyManager(content, tileMap, _hero);
            _enemies = _enemyManager.InitializeEnemies();

            _powerupManager = new PowerupManager(content);

            var heartsTexture = _content.Load<Texture2D>("Hearts");
            _heartsAnimation = new Animation(heartsTexture, 3); // 3 frames
        }

        public void Update(GameTime gameTime)
        {
            if (_hero.IsDead)
            {
                _game.GameOver();
                return;
            }

            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();
            int tileWidth = _tileMap.TileWidth;
            int tileHeight = _tileMap.TileHeight;
            int screenWidth = _graphics.PreferredBackBufferWidth;
            int screenHeight = _graphics.PreferredBackBufferHeight;

            _hero.Update(gameTime, keyboardState, mouseState, _tileMap.GetTileMapArray(), tileWidth, tileHeight, screenWidth, screenHeight);
            _enemyManager.UpdateEnemies(gameTime, keyboardState, mouseState, _tileMap.GetTileMapArray(), tileWidth, tileHeight, screenWidth, screenHeight);
            _powerupManager.UpdatePowerups(_hero);

            if (_enemyManager.AreAllEnemiesDefeated())
            {
                _game.LoadNextLevel();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _tileMap.Draw(spriteBatch);
            _hero.Draw(spriteBatch);
            _enemyManager.DrawEnemies(spriteBatch);
            _powerupManager.DrawPowerups(spriteBatch);

            float heartScale = 0.5f;
            _heartsAnimation.Draw(spriteBatch, new Vector2(0, 0), SpriteEffects.None, heartScale);
        }
    }
}
