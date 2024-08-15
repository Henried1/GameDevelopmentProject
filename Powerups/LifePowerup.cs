using GameProject.Characters.Player;
using GameProject.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameProject.Powerups
{
    public class LifePowerup : Powerup
    {
        private GameManager _gameManager;

        public LifePowerup(Vector2 position, GameManager gameManager) : base(position)
        {
            _gameManager = gameManager;
        }

        public override void ApplyEffect(Hero player)
        {
            player.RestoreFullHealth(_gameManager.HeartsAnimation);
        }

        protected override string GetTexturePath()
        {
            return "Powerups/Life";
        }
    }
}
