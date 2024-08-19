using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameProject.Managers
{
    public class UIManager
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _startScreenTexture;
        private Texture2D _gameOverScreenTexture;
        private Texture2D _victoryScreenTexture;
        private Texture2D _startButtonTexture;
        private Rectangle _startButtonRectangle;
        private Texture2D _defeatTextTexture;
        private Texture2D _victoryTextTexture;
        private Texture2D _replayButtonTexture;
        private Rectangle _replayButtonRectangle;
        private bool _wasMousePressed;

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
            _startButtonRectangle = new Rectangle(buttonX, buttonY, buttonWidth, buttonHeight);
            _replayButtonRectangle = new Rectangle(buttonX, buttonY, buttonWidth, buttonHeight);
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
            bool isMousePressed = mouseState.LeftButton == ButtonState.Pressed;
            bool isPressed = isMousePressed && !_wasMousePressed && _startButtonRectangle.Contains(mouseState.Position);
            _wasMousePressed = isMousePressed;
            return isPressed;
        }

        public bool IsReplayButtonPressed(MouseState mouseState)
        {
            bool isMousePressed = mouseState.LeftButton == ButtonState.Pressed;
            bool isPressed = isMousePressed && !_wasMousePressed && _replayButtonRectangle.Contains(mouseState.Position);
            _wasMousePressed = isMousePressed;
            return isPressed;
        }

        public void DrawStartScreen()
        {
            _spriteBatch.Draw(_startScreenTexture, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.White);
            _spriteBatch.Draw(_startButtonTexture, _startButtonRectangle, Color.White);
        }

        public void DrawGameOverScreen()
        {
            _spriteBatch.Draw(_gameOverScreenTexture, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.White);
            DrawCenteredTexture(_defeatTextTexture, -360, 0.8f);
            _spriteBatch.Draw(_replayButtonTexture, _replayButtonRectangle, Color.White);
        }

        public void DrawVictoryScreen()
        {
            _spriteBatch.Draw(_victoryScreenTexture, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.White);
            DrawCenteredTexture(_victoryTextTexture);
            _spriteBatch.Draw(_replayButtonTexture, _replayButtonRectangle, Color.White);
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
