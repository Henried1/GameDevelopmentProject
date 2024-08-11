using GameProject.Animations;
using GameProject.Characters;
using GameProject.Characters.Player;
using GameProject.Map;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using GameProject.Characters.Interfaces;

namespace GameProject.Managers
{
    public class GameManager
    {
        private ICharacter _hero;
        private ICharacter _slime;
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
        }
        public void InitializeHero(TileMap tileMap)
        {
            _tileMap = tileMap;

            _hero = new Hero(Vector2.Zero, 2f);
            _hero.LoadContent(_content);

            Vector2 heroPosition = _tileMap.FindGroundPosition(_hero.Height);
            _hero.Position = heroPosition;

            _slime = new Slime(Vector2.Zero);
            _slime.LoadContent(_content);

            
            Vector2 slimePosition = _tileMap.FindGroundPosition(_slime.Height);
            slimePosition.X += 200; 
            _slime.Position = slimePosition; 

         
            if (_hero is Hero hero)
            {
                hero.OnDamageTaken += OnHeroDamageTaken;
            }

           
            var heartsTexture = _content.Load<Texture2D>("Hearts");
            _heartsAnimation = new Animation(heartsTexture, 3); // 3 frames
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
            _slime.Update(gameTime, keyboardState, mouseState, _tileMap.GetTileMapArray(), tileWidth, tileHeight, screenWidth, screenHeight);

            if (_hero.IsDead)
            {
                _isGameOver = true;
            }

            if (_hero is ICollidable heroCollidable && _slime is ICollidable slimeCollidable)
            {
                if (heroCollidable.Hitbox.Intersects(slimeCollidable.Hitbox))
                {
                    heroCollidable.OnCollision(slimeCollidable);
                    slimeCollidable.OnCollision(heroCollidable);
                }
            }

            if (_hero is Hero hero)
            {
                Rectangle attackHitbox = hero.AttackHitbox;
                // Check for collisions with enemies or other objects here
                // Example:
                /* if (attackHitbox.Intersects(_slime.BoundingBox))
                 {
                     _slime.TakeDamage(attackDamage);
                 }*/
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _tileMap.Draw(spriteBatch);

            _hero.Draw(spriteBatch);

            _slime.Draw(spriteBatch);

            float heartScale = 0.5f; //scale of heart
            _heartsAnimation.Draw(spriteBatch, new Vector2(0, 0), SpriteEffects.None, heartScale);
        }
    }
}
