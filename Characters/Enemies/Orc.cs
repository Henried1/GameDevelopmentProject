using GameProject.Animations;
using GameProject.Characters.Enemies;
using GameProject.Characters.Interfaces;
using GameProject.Characters.Player;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using static System.Net.Mime.MediaTypeNames;

public class Orc : Enemy
{
    protected Animation _attackAnimation;
    private double _attackCooldown;
    private double _attackCooldownTime = 5.0;
    private Hero _hero;
    private bool _damageApplied;
    private double _damageTimer;
    private double _damageInterval = 3.0; // Damage interval in seconds

    public Orc(Vector2 startPosition) : base(startPosition, 100, 10)
    {
        _attackCooldown = _attackCooldownTime;
        _damageApplied = false;
        _damageTimer = 0;
        Damage = 10; // Set the damage value for Orc
    }

    public void SetHeroReference(Hero hero)
    {
        _hero = hero;
    }

    public override void LoadContent(ContentManager content)
    {
        var idleTexture = content.Load<Texture2D>("Enemies/Orc/Idle");
        var walkTexture = content.Load<Texture2D>("Enemies/Orc/Walk");
        var attackTexture = content.Load<Texture2D>("Enemies/Orc/Attack_1");

        _idleAnimation = new Animation(idleTexture, 5);
        _walkAnimation = new Animation(walkTexture, 7);
        _attackAnimation = new Animation(attackTexture, 4, 0.55);
    }

    public override void Update(GameTime gameTime, KeyboardState keyboardState, MouseState mouseState, int[,] tileMap, int tileWidth, int tileHeight, int screenWidth, int screenHeight)
    {
        if (_isDead)
        {
            return;
        }

        _attackCooldown -= gameTime.ElapsedGameTime.TotalSeconds;
        _damageTimer += gameTime.ElapsedGameTime.TotalSeconds;

        _timeCounter += gameTime.ElapsedGameTime.TotalSeconds;
        if (_timeCounter >= _timePerFrame)
        {
            _timeCounter -= _timePerFrame;
        }

        switch (_currentState)
        {
            case EnemyState.Walking:
                if (IsPlayerInRange() && _attackCooldown <= 0)
                {
                    _currentState = EnemyState.Attacking;
                    _attackAnimation.Reset();
                    _damageApplied = false; // Reset the damage flag at the start of the attack
                    FaceHero();
                }
                else
                {
                    _walkTimeCounter += gameTime.ElapsedGameTime.TotalSeconds;

                    if (_walkTimeCounter >= _walkDuration)
                    {
                        _currentState = EnemyState.Idle;
                        _walkTimeCounter = 0;
                        _idleTimeCounter = 0;
                    }

                    _position.X += _isMovingRight ? 1 : -1;
                }
                break;

            case EnemyState.Idle:
                _idleTimeCounter += gameTime.ElapsedGameTime.TotalSeconds;

                if (_idleTimeCounter >= _idleDuration)
                {
                    _currentState = EnemyState.Walking;
                    _isMovingRight = !_isMovingRight;
                    _walkTimeCounter = 0;
                }
                break;

            case EnemyState.Attacking:
                _attackAnimation.Update(gameTime);

                if (_attackAnimation.CurrentFrame == 3 && _attackAnimation.FrameJustChanged && !_damageApplied)
                {
                    if (this.Hitbox.Intersects(_hero.Hitbox) && _damageTimer >= _damageInterval)
                    {
                        _hero.TakeDamage(Damage); // Apply damage once per interval
                        _damageApplied = true; // Set the flag to prevent continuous damage
                        _damageTimer = 0; // Reset the damage timer
                    }
                }

                if (_attackAnimation.IsComplete)
                {
                    _currentState = EnemyState.Idle;
                    _attackCooldown = _attackCooldownTime;
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

        switch (_currentState)
        {
            case EnemyState.Walking:
                _walkAnimation.Update(gameTime);
                break;
            case EnemyState.Idle:
                _idleAnimation.Update(gameTime);
                break;
            case EnemyState.Attacking:
                _attackAnimation.Update(gameTime);
                break;
        }
    }

    private void FaceHero()
    {
        if (_hero != null)
        {
            _isMovingRight = _hero.Position.X > _position.X;
        }
    }

    private bool IsPlayerInRange()
    {
        if (_hero == null)
            return false;

        float distance = Vector2.Distance(_position, _hero.Position);

        float attackRange = 80f;

        return distance <= attackRange;
    }

    public override void OnCollision(ICollidable other)
    {
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (_isDead)
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
            case EnemyState.Attacking:
                _attackAnimation.Draw(spriteBatch, _position, spriteEffects);
                break;
        }

        Texture2D hitboxTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
        hitboxTexture.SetData(new[] { Color.Red });

        spriteBatch.Draw(hitboxTexture, Hitbox, Color.Red * 0.5f);

        if (_currentState == EnemyState.Attacking)
        {
            spriteBatch.Draw(hitboxTexture, AttackHitbox, Color.Blue * 0.5f);
        }
    }

    public override Rectangle Hitbox
    {
        get
        {
            int hitboxWidth = (int)(_walkAnimation.FrameWidth * 0.45f);
            int hitboxHeight = (int)(_walkAnimation.FrameHeight * 0.66f);
            int hitboxX = (int)(_position.X + (_walkAnimation.FrameWidth - hitboxWidth) / 2);
            int hitboxY = (int)(_position.Y + (_walkAnimation.FrameHeight - hitboxHeight));

            return new Rectangle(hitboxX, hitboxY, hitboxWidth, hitboxHeight);
        }
    }

    public Rectangle AttackHitbox
    {
        get
        {
            int hitboxWidth = (int)(_attackAnimation.FrameWidth * 0.45f);
            int hitboxHeight = (int)(_attackAnimation.FrameHeight * 0.66f);
            int hitboxX = (int)(_position.X + (_attackAnimation.FrameWidth - hitboxWidth) / 2);
            int hitboxY = (int)(_position.Y + (_attackAnimation.FrameHeight - hitboxHeight));

            return new Rectangle(hitboxX, hitboxY, hitboxWidth, hitboxHeight);
        }
    }
}
