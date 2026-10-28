using PlatformerGame.Core;
using PlatformerGame.Entities;
using PlatformerGame.Enums;
using PlatformerGame.Interfaces;
using PlatformerGame.PlayerCharacters;
using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media.Animation;

namespace PlatformerGame.PlayerCharacters
{
    public class Player : Character, IDamageable
    {
        // --- Movement Constants ---
        public float WalkSpeed { get; set; } = 300f;
        public float RunSpeed { get; set; } = 500f;
        public float JumpForce { get; set; } = 1000f;
        public float CoyoteTime { get; set; } = 0.1f;
        public float JumpBufferTime { get; set; } = 0.1f;
        public float WallSlideSpeed { get; set; } = 100f;
        public Vector WallJumpForce { get; set; } = new Vector(350, 500);

        // --- State Timers ---
        private float coyoteTimer;
        private float jumpBufferTimer;
        private float invulnerabilityTimer;

        // --- Collision Tracking ---
        private Vector previousPosition;

        // --- Properties ---
        public int Health { get; set; }
        public int MaxHealth { get; set; } = 1;
        public PlayerState State { get; set; } = PlayerState.Idle;
        public PowerUp CurrentPowerUp { get; set; }
        public bool IsGrounded { get; set; }
        public bool IsTouchingWall { get; set; }
        public int WallDirection { get; set; }


        public PlayerStats Stats { get; private set; }

        public Player(int id, Vector position)
            : base(id, position, new Vector(64, 128), 1500f) // 64x128 size, 1500 gravity
        {
            Stats = new PlayerStats();
            Health = MaxHealth;
            previousPosition = position;

        }

        public override void Update(float dt)
        {
            previousPosition = Position; // Save for collision checks

            UpdateCoyoteTime(dt);
            if (jumpBufferTimer > 0) jumpBufferTimer -= dt;
            if (invulnerabilityTimer > 0) invulnerabilityTimer -= dt;

            if (Health <= 0)
            {
                State = PlayerState.Dead;
                return;
            }

            ApplyGravity(dt);
            MoveAndCollide(dt);
            HandleEntityInteractions();
        }

        public override void ApplyGravity(float dt)
        {
            IsGrounded = false; // Assume in air until collision proves otherwise

            Velocity += new Vector(0, Gravity * dt);

            if (Velocity.Y > 1500f) Velocity = new Vector(Velocity.X, 1500f);

            if (IsTouchingWall && Velocity.Y > 0)
            {
                State = PlayerState.WallSliding;
                if (Velocity.Y > WallSlideSpeed) Velocity = new Vector(Velocity.X, WallSlideSpeed);
            }
            else if (Velocity.Y < 0) State = PlayerState.Jumping;
            else if (Velocity.Y > 0) State = PlayerState.Falling;
        }

        public override void MoveAndCollide(float dt)
        {
            Position += new Vector(0, Velocity.Y * dt);
            CheckPlatformCollisions(isHorizontal: false);

            Position += new Vector(Velocity.X * dt, 0);
            CheckPlatformCollisions(isHorizontal: true);
        }

        private void CheckPlatformCollisions(bool isHorizontal)
        {
            if (Game.Instance.CurrentLevel == null) return;

            Rect checkBounds = GetBounds();
            if (isHorizontal)
            {
                checkBounds = new Rect(Position.X + 1, Position.Y + 1, Size.X - 2, Size.Y - 2);
            }

            var nearbyEntities = Game.Instance.CurrentLevel.GetEntitiesIn(checkBounds);

            foreach (var entity in nearbyEntities)
            {
                if (entity is Platform || entity is GameBlock)
                {
                    bool isOneWay = entity is Platform p && p.IsOneWay;
                    bool isSolid = entity is GameBlock b && b.IsSolid;

                    if (checkBounds.IntersectsWith(entity.GetBounds()))
                    {
                        if (isHorizontal)
                        {
                            if (!isOneWay)
                            {
                                if (Velocity.X > 0)
                                    Position = new Vector(entity.Position.X - Size.X, Position.Y);
                                else if (Velocity.X < 0)
                                    Position = new Vector(entity.Position.X + entity.Size.X, Position.Y);

                                Velocity = new Vector(0, Velocity.Y);
                            }
                        }
                        else
                        {
                            if (isOneWay)
                            {
                                if (Velocity.Y > 0 && previousPosition.Y + Size.Y <= entity.Position.Y + 10)
                                {
                                    Position = new Vector(Position.X, entity.Position.Y - Size.Y);
                                    Velocity = new Vector(Velocity.X, 0);
                                    IsGrounded = true;
                                }
                            }
                            else
                            {
                                if (Velocity.Y > 0) // Landed on top
                                {
                                    Position = new Vector(Position.X, entity.Position.Y - Size.Y);
                                    IsGrounded = true;
                                }
                                else if (Velocity.Y < 0)
                                {
                                    Position = new Vector(Position.X, entity.Position.Y + entity.Size.Y);

                                    if (entity is GameBlock block)
                                    {
                                        block.OnHitFromBelow(this);
                                    }
                                }
                                Velocity = new Vector(Velocity.X, 0);
                            }
                        }
                    }
                }
            }
        }

        private void HandleEntityInteractions()
        {
            if (Game.Instance.CurrentLevel == null) return;

            var nearbyEntities = Game.Instance.CurrentLevel.GetEntitiesIn(GetBounds());

            foreach (var entity in nearbyEntities)
            {
                if (entity == this || !entity.Active) continue;




                if (entity is Collectable c)
                {
                    c.Collect(this);
                    if (c is Fossil) Game.Instance.CurrentLevel.FossilsCollected++;
                }
                else if (entity is Hazard h)
                {
                    TakeDamage(h.Damage);
                }
                else if (entity is Checkpoint cp)
                {
                    cp.Activate(this);
                    Game.Instance.CurrentLevel.SetActiveCheckpoint(cp);
                }
                else if (entity is LevelEnd le)
                {
                    le.Activate(this);
                }
                else if (entity is Enemy e && !e.IsDead)
                {
                    if (Velocity.Y > 0 && previousPosition.Y + Size.Y <= e.Position.Y + 20)
                    {
                        e.TakeDamage(1);
                        Velocity = new Vector(Velocity.X, -JumpForce * 0.7f);
                    }
                    else
                    {
                        TakeDamage(1);
                    }
                }
            }
        }

        public void Move(float direction, bool running)
        {
            if (State == PlayerState.Dead) return;

            float targetSpeed = running ? RunSpeed : WalkSpeed;
            float targetVelocityX = direction * targetSpeed;

            Velocity = new Vector(
                Velocity.X + (targetVelocityX - Velocity.X) * 0.3f,
                Velocity.Y
            );

            if (direction != 0 && IsGrounded)
                State = running ? PlayerState.Running : PlayerState.Walking;
            else if (direction == 0 && IsGrounded && Math.Abs(Velocity.X) < 10)
                State = PlayerState.Idle;
        }

        public void TryJump()
        {
            jumpBufferTimer = JumpBufferTime;

            if (IsGrounded || coyoteTimer > 0)
            {
                Velocity = new Vector(Velocity.X, -JumpForce);
                IsGrounded = false;
                coyoteTimer = 0;
                jumpBufferTimer = 0;
                State = PlayerState.Jumping;
            }
            else if (IsTouchingWall)
            {
            }
        }


        public void UpdateCoyoteTime(float dt)
        {
            if (IsGrounded) coyoteTimer = CoyoteTime;
            else if (coyoteTimer > 0) coyoteTimer -= dt;
        }

        public void TakeDamage(int amount)
        {
            if (invulnerabilityTimer > 0 || State == PlayerState.Dead) return;

            invulnerabilityTimer = 1.0f;

            Velocity = new Vector(Velocity.X, -300f);

            if (CurrentPowerUp != null)
            {
                CurrentPowerUp.Remove(this);
                CurrentPowerUp = null;
                State = PlayerState.Damaged;
            }
            else
            {
                Health -= amount;
                State = PlayerState.Damaged;

                if (Health <= 0)
                {
                    Die();
                }
            }
        }

        public void Die()
        {
            State = PlayerState.Dead;
            Stats.LoseLife();
        }

        public void GainLife(int amount) => Stats.GainLife();
        public void GainHealth(int amount) => Health = Math.Min(Health + amount, MaxHealth);

        public void ApplyPowerUp(PowerUp p)
        {
            CurrentPowerUp?.Remove(this);
            CurrentPowerUp = p;
            CurrentPowerUp.Apply(this);
        }
        public bool IsAlive()
        {
            return Health > 0 && State != PlayerState.Dead;
        }
        public void Respawn(Vector position)
        {
            Position = position;
            previousPosition = position;
            Velocity = new Vector(0, 0);
            Health = MaxHealth;
            State = PlayerState.Idle;
            CurrentPowerUp = null;
            invulnerabilityTimer = 1.5f;
        }
    
    }
}