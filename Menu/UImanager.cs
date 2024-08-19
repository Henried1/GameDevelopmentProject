using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameProject.Menu;

namespace GameProject.Menu
{
    public class UIManager
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _startScreenTexture;
        private Texture2D _gameOverScreenTexture;
        private Texture2D _victoryScreenTexture;
        private Texture2D _startButtonTexture; 
        private Texture2D _replayButtonTexture; 
        private Button _startButton;
        private Button _replayButton;
        private Texture2D _defeatTextTexture;
        private Texture2D _victoryTextTexture;

        public UIManager(GraphicsDeviceManager graphics, SpriteBatch spriteBatch, ContentManager content)
        {
            _graphics = graphics;
            _spriteBatch = spriteBatch;
            LoadContent(content);
            Initialize();
        }

        private void Initialize()
        {
            int buttonWidth = 200;
            int buttonHeight = 200;
            int buttonX = (_graphics.PreferredBackBufferWidth - buttonWidth) / 2;
            int buttonY = (_graphics.PreferredBackBufferHeight - buttonHeight) / 2;
            _startButton = new Button(_startButtonTexture, new Rectangle(buttonX, buttonY, buttonWidth, buttonHeight));

            int replayButtonX = buttonX - 100;
            int replayButtonY = buttonY + 305;
            _replayButton = new Button(_replayButtonTexture, new Rectangle(replayButtonX, replayButtonY, buttonWidth, buttonHeight));
        }

        private void LoadContent(ContentManager content)
        {
            _startScreenTexture = content.Load<Texture2D>("MenuAssets/startScreen");
            _gameOverScreenTexture = content.Load<Texture2D>("MenuAssets/medievalKnight");
            _victoryScreenTexture = content.Load<Texture2D>("MenuAssets/Victory");
            _startButtonTexture = content.Load<Texture2D>("MenuAssets/start");
            _defeatTextTexture = content.Load<Texture2D>("MenuAssets/DefeatText");
            _victoryTextTexture = content.Load<Texture2D>("MenuAssets/VictoryText");
            _replayButtonTexture = content.Load<Texture2D>("MenuAssets/Replay");
        }

        public bool IsStartButtonPressed(MouseState mouseState)
        {
            return _startButton.IsPressed(mouseState);
        }

        public bool IsReplayButtonPressed(MouseState mouseState)
        {
            return _replayButton.IsPressed(mouseState);
        }

        public void DrawStartScreen()
        {
            _spriteBatch.Draw(_startScreenTexture, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.White);
            _startButton.Draw(_spriteBatch);
        }

        public void DrawGameOverScreen()
        {
            _spriteBatch.Draw(_gameOverScreenTexture, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.White);
            DrawCenteredTexture(_defeatTextTexture, -360, 0.8f);
            _replayButton.Draw(_spriteBatch);
        }

        public void DrawVictoryScreen()
        {
            _spriteBatch.Draw(_victoryScreenTexture, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.White);
            DrawCenteredTexture(_victoryTextTexture);
            _replayButton.Draw(_spriteBatch);
        }

        private void DrawCenteredTexture(Texture2D texture, int xOffset = 0, float scale = 1.0f)
        {
            Vector2 position = new Vector2(
                (_graphics.PreferredBackBufferWidth - texture.Width * scale) / 2 + xOffset,
                (_graphics.PreferredBackBufferHeight - texture.Height * scale) / 2
            );

            _spriteBatch.Draw(texture, position, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
    }
}
