// EnemyFactory.cs
using Microsoft.Xna.Framework;
using GameProject.Characters.Enemies;
using GameProject.Managers;
using System;

namespace GameProject.Factories
{
    public static class EnemyFactory
    {
        public static Enemy CreateEnemy(string enemyType, Vector2 position, GameManager gameManager)
        {
            return enemyType switch
            {
                "Slime" => new Slime(position, gameManager),
                "Orc" => new Orc(position, gameManager),
                "FireSpirit" => new FireSpirit(position, 100, 10.0, gameManager),
                _ => throw new ArgumentException("Invalid enemy type")
            };
        }

    }
}
