using GameProject.Characters.Player;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameProject.Powerups
{
    public class BubblePowerup : Powerup
    {
        public BubblePowerup(Vector2 position) : base(position) { }

        public override void ApplyEffect(Hero player)
        {
            player.ActivateShield(3);
        }

        protected override string GetTexturePath()
        {
            return "Powerups/Bubble";
        }
    }
}
