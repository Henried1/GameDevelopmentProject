using GameProject.Animations;
using GameProject.Characters.Enemies;
using GameProject.Characters.Interfaces;
using GameProject.Characters.Player;
using GameProject.Managers;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using static System.Net.Mime.MediaTypeNames;
using System.Collections.Generic;

public class FireSpirit : Enemy
{
    private List<Fireball> _fireballs;
    private double _fireballCooldown;
    private double _fireballCooldownTime = 2.0;
    private Hero _hero;
    private float _minDistance = 200f; 
    private float _movementSpeed = 30.0f; 

    private Vector2 _patrolPoint1;
    private Vector2 _patrolPoint2;
    private bool _movingToPoint1 = true;
    private float _patrolSpeed = 10f;

    public FireSpirit(Vector2 startPosition, int healthPoints, double fps, GameManager gameManager)
        : base(startPosition, healthPoints, fps, gameManager)
    {
        _fireballs = new List<Fireball>();
        _fireballCooldown = 0;
        _currentState = EnemyState.Walking;

        // Example patrol points
        _patrolPoint1 = new Vector2(startPosition.X - 100, startPosition.Y);
        _patrolPoint2 = new Vector2(startPosition.X + 100, startPosition.Y);
    }

    public override void SetHeroReference(Hero hero)
    {
        _hero = hero;
    }

    public override void LoadContent(ContentManager content)
    {
        _content = content;
        Texture2D idleTexture = content.Load<Texture2D>("Enemies/FireSpirit/Idle");
        _idleAnimation = new Animation(idleTexture, 6);

        Texture2D walkTexture = content.Load<Texture2D>("Enemies/FireSpirit/Walk");
        _walkAnimation = new Animation(walkTexture, 7);
    }

    public override void Update(GameTime gameTime, KeyboardState keyboardState, MouseState mouseState, int[,] tileMap, int tileWidth, int tileHeight, int screenWidth, int screenHeight)
    {
        if (!IsAlive())
        {
            return;
        }

        _fireballCooldown -= gameTime.ElapsedGameTime.TotalSeconds;

        if (_hero != null)
        {
            Vector2 directionToHero = _hero.Position - _position;
            float distanceToHero = directionToHero.Length();

            if (distanceToHero < _minDistance)
            {
                _currentState = EnemyState.Walking;
                directionToHero.Normalize();
                _position -= directionToHero * (float)(_movementSpeed * gameTime.ElapsedGameTime.TotalSeconds);

                if (IsPlayerInRange() && _fireballCooldown <= 0)
                {
                    ShootFireball();
                    _fireballCooldown = _fireballCooldownTime;
                }
            }
            else
            {
                Patrol(gameTime);
            }

            foreach (var fireball in _fireballs)
            {
                fireball.Update(gameTime);

                if (fireball.CheckCollision(_hero.Hitbox))
                {
                    _hero.TakeDamage(fireball.Damage);
                    fireball.MarkForRemoval = true;
                }
            }

            _fireballs.RemoveAll(f => f.IsOffScreen(screenWidth, screenHeight) || f.MarkForRemoval);
        }

        switch (_currentState)
        {
            case EnemyState.Walking:
                _walkAnimation.Update(gameTime);
                break;
            case EnemyState.Idle:
                _idleAnimation.Update(gameTime);
                break;
        }
    }


    private void ShootFireball()
    {
        Vector2 direction = _hero.Position - _position;
        direction.Normalize();

        float fireballOffset = 20f;
        Vector2 fireballPosition = _position + direction * fireballOffset;

        fireballPosition.Y = _position.Y + 60;

        var graphicsDeviceService = (IGraphicsDeviceService)_content.ServiceProvider.GetService(typeof(IGraphicsDeviceService));
        Fireball fireball = new Fireball(fireballPosition, direction, graphicsDeviceService.GraphicsDevice);
        fireball.LoadContent(_content);
        _fireballs.Add(fireball);
    }

    private bool IsPlayerInRange()
    {
        if (_hero == null)
            return false;

        float distance = Vector2.Distance(_position, _hero.Position);
        float attackRange = 300f;

        return distance <= attackRange;
    }

    private void Patrol(GameTime gameTime)
    {
        Vector2 target = _movingToPoint1 ? _patrolPoint1 : _patrolPoint2;
        Vector2 direction = target - _position;
        float distance = direction.Length();

        if (distance < 1f)
        {
            _movingToPoint1 = !_movingToPoint1;
            _currentState = EnemyState.Idle;  
            return;
        }

        direction.Normalize();
        _position += direction * (float)(_patrolSpeed * gameTime.ElapsedGameTime.TotalSeconds);
        _currentState = EnemyState.Walking;  
    }


    public override void Draw(SpriteBatch spriteBatch)
    {
        if (!IsAlive())
        {
            return;
        }

        SpriteEffects spriteEffects = _isMovingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

        switch (_currentState)
        {
            case EnemyState.Walking:
                _walkAnimation.Draw(spriteBatch, _position, spriteEffects);
                break;
            case EnemyState.Idle:
                _idleAnimation.Draw(spriteBatch, _position, spriteEffects);
                break;
        }

        foreach (var fireball in _fireballs)
        {
            fireball.Draw(spriteBatch);
        }
    }


    public override void OnCollision(ICollidable other)
    {
        if (other is Hero hero)
        {
            hero.TakeDamage(Damage);
        }
    }

    public bool IsAlive()
    {
        return _healthPoints > 0;
    }
}
