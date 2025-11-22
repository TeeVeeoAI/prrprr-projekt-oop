using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using prrprr_projekt_oop.Systems;
using prrprr_projekt_oop.Entities.Weapons;
using prrprr_projekt_oop.Entities.Projectiles;
using prrprr_projekt_oop.Forms;

namespace prrprr_projekt_oop.Entities
{
    public class Player : BaseEntity
    {
        private Keys[] keys;
        private Weapon weapon;
        private List<Projectile> projectiles = new List<Projectile>();
        private Texture2D projectileTexture;
        private float rotation = 0f;
        private float invincibilityTimer = 0f;
        private const float invincibilityDuration = 1f;
        private Circle xpPickupCollider;
        private float pullRadius = 120f;
        private float pullSpeed = 300f; // pixels per second when pulled
        private int xp = 0;
        private int level = 1;
        private int xpToNextLevel = 100;

        public int XP 
        { 
            get => xp; 
        }

        public int XPToNextLevel
        {
            get => xpToNextLevel;
        }
        public Circle XPPickupCollider
        {
            get => xpPickupCollider;
        }

        public float PullRadius {
            get => pullRadius;
        }

        public float PullSpeed {
            get => pullSpeed;
        }

        public int Level {
            get => level;
        }

        public List<Projectile> Projectiles {
            get => projectiles;
        }

        public Player(Vector2 heightAndWidth, Texture2D texture, Texture2D projectileTexture)
        : base(Game1.ScreenSize / 2 - heightAndWidth, 400f, heightAndWidth, texture, 5, Color.White)
        {
            keys = new Keys[]
            {
                Keys.W,
                Keys.A,
                Keys.S,
                Keys.D
            };

            this.projectileTexture = projectileTexture;
            xpPickupCollider = new Circle(new Vector2(position.X + hitbox.Width / 2, position.Y + hitbox.Height / 2), Math.Max(hitbox.Width, hitbox.Height) * 2f);

            weapon = new Rifle();
        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (invincibilityTimer > 0)
            {
                invincibilityTimer -= deltaTime;
            }

            if (xp >= xpToNextLevel)
            {
                xp -= xpToNextLevel;
                LevelUp();
            }

            Vector2 mousePos = InputSystem.GetMousePosition();
            Vector2 playerCenter = position + new Vector2(hitbox.Width / 2, hitbox.Height / 2);
            Vector2 dirToMouse = mousePos - playerCenter;
            rotation = (float)Math.Atan2(dirToMouse.Y, dirToMouse.X) + (float)Math.PI / 2f;

            // Reset velocity to zero and accumulate input directions
            Vector2 inputDirection = Vector2.Zero;
            
            if (InputSystem.IsKeyDown(keys[(int)PlayerKeys.Up]))
            {
                inputDirection.Y -= 1;
            }
            if (InputSystem.IsKeyDown(keys[(int)PlayerKeys.Down]))
            {
                inputDirection.Y += 1;
            }
            if (InputSystem.IsKeyDown(keys[(int)PlayerKeys.Left]))
            {
                inputDirection.X -= 1;
            }
            if (InputSystem.IsKeyDown(keys[(int)PlayerKeys.Right]))
            {
                inputDirection.X += 1;
            }

            // Normalize diagonal movement to prevent faster diagonal speed
            if (inputDirection != Vector2.Zero)
            {
                inputDirection.Normalize();
                velocity = inputDirection * speed * deltaTime;
            }
            else
            {
                velocity *= friction; // Friction only when no input
            }

            if (InputSystem.IsLeftDown())
            {
                UseWeapon(gameTime);
            }

            position += velocity;

            // Keep player within screen bounds
            position.X = MathHelper.Clamp(position.X, 0, Game1.ScreenSize.X - hitbox.Width);
            position.Y = MathHelper.Clamp(position.Y, 0, Game1.ScreenSize.Y - hitbox.Height);

            xpPickupCollider.ChangePos(new Vector2(position.X + hitbox.Width / 2, position.Y + hitbox.Height / 2));
            hitbox.Location = position.ToPoint();
        }

        public void LevelUp()
        {
            level++;
            xpToNextLevel = (int)(xpToNextLevel * 1.5f);
            speed *= 1.1f;
            weapon = new Rifle(weapon.FireRateSeconds * 0.9f, weapon.Damage * 2);
        }

        public Projectile TryShoot(GameTime gameTime, Vector2 direction, Texture2D projTexture)
        {
            if (weapon == null) return null;
            Vector2 spawn = position + new Vector2(hitbox.Width / 2, hitbox.Height / 2);
            return weapon.TryShoot(gameTime, spawn, direction, projTexture, this);
        }

        public void UseWeapon(GameTime gameTime)
        {
            Vector2 mousePos = InputSystem.GetMousePosition();
            Vector2 direction = mousePos - (position + new Vector2(hitbox.Width / 2, hitbox.Height / 2));
            Projectile projectile = TryShoot(gameTime, direction, projectileTexture);
            if (projectile != null)
            {
                projectiles.Add(projectile);
            }

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (hp <= 0)
                color = ColorBlink(Color.Red, Color.White, gameTime, 2f);
            else if (IsInvincible())
                color = ColorBlink(Color.Red, Color.White, gameTime, 4f);
            
            // Draw the player sprite with rotation centered on the sprite's center
            Vector2 origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
            spriteBatch.Draw(
                texture,
                new Vector2(hitbox.X + hitbox.Width/2, hitbox.Y + hitbox.Height/2),
                null,
                color,
                rotation,
                origin,
                hitbox.Size.ToVector2() / texture.Bounds.Size.ToVector2(),
                SpriteEffects.None,
                0f
            );
            color = Color.White;
        }

        public Color ColorBlink(Color color1, Color color2, GameTime gameTime, float blinkSpeed = 2f)
        {
            float time = (float)gameTime.TotalGameTime.TotalSeconds * blinkSpeed;
            float t = (float)(Math.Sin(time * Math.PI * 2) + 1) / 2;
            return Color.Lerp(color1, color2, t);
        }

        public void ChangeColor(Color newColor)
        {
            color = newColor;
        }

        public void TakeDamage(int damage)
        {
            hp -= damage;
        }

        public bool IsDead()
        {
            return hp <= 0;
        }

        public bool IsInvincible()
        {
            return invincibilityTimer > 0f;
        }

        public void ApplyInvincibility()
        {
            invincibilityTimer = invincibilityDuration;
        }

        public void AddXP(int amount)
        {
            if (amount <= 0) return;
            xp += amount;
        }
    }

    public enum PlayerKeys
    {
        Up = 0,
        Left,
        Down,
        Right
    }
}

