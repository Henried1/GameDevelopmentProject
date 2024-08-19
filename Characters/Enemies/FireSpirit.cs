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

    public FireSpirit(Vector2 startPosition, int healthPoints, double fps, GameManager gameManager)
        : base(startPosition, healthPoints, fps, gameManager)
    {
        _fireballs = new List<Fireball>();
        _fireballCooldown = 0;
        _currentState = EnemyState.Walking;
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
                Patrol(gameTime, tileMap, tileWidth, tileHeight);
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

    private void Patrol(GameTime gameTime, int[,] tileMap, int tileWidth, int tileHeight)
    {
        _timeCounter += gameTime.ElapsedGameTime.TotalSeconds;

        if (_timeCounter >= _timePerFrame)
        {
            _timeCounter -= _timePerFrame;
        }

        switch (_currentState)
        {
            case EnemyState.Walking:
                _walkTimeCounter += gameTime.ElapsedGameTime.TotalSeconds;

                if (_walkTimeCounter >= _walkDuration)
                {
                    _currentState = EnemyState.Idle;
                    _walkTimeCounter = 0;
                    _idleTimeCounter = 0;
                }

                _position.X += _isMovingRight ? 1 : -1;
                break;

            case EnemyState.Idle:
                _idleTimeCounter += gameTime.ElapsedGameTime.TotalSeconds;

                if (_idleTimeCounter >= _idleDuration)
                {
                    _currentState = EnemyState.Walking;
                    _idleTimeCounter = 0;
                    _walkTimeCounter = 0;
                    _isMovingRight = !_isMovingRight; // Change direction
                }
                break;
        }

        if (!IsCollidingWithGround(_position, tileMap, tileWidth, tileHeight))
        {
            _position.Y += 1;
        }
        else
        {
            _position.Y = SnapToGround(_position.Y, tileMap, tileWidth, tileHeight);
        }
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

