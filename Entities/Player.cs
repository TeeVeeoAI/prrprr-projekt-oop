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

namespace prrprr_projekt_oop.Entities
{
    public class Player : BaseEntity
    {
        private Keys[] keys;
        private Weapon weapon;
        private List<Projectile> projectiles = new List<Projectile>();
        private Texture2D projectileTexture;
        private float rotation = 0f;

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

            weapon = new Weapon(0.12f, 1, Vector2.Zero, 600f);
        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Calculate rotation to face the mouse
            Vector2 mousePos = InputSystem.GetMousePosition();
            Vector2 playerCenter = position + new Vector2(hitbox.Width / 2, hitbox.Height / 2);
            Vector2 dirToMouse = mousePos - playerCenter;
            rotation = (float)Math.Atan2(dirToMouse.Y, dirToMouse.X) + (float)Math.PI / 2f;

            if (InputSystem.IsKeyDown(keys[(int)PlayerKeys.Up]))
            {
                position.Y -= speed * deltaTime;
            }
            if (InputSystem.IsKeyDown(keys[(int)PlayerKeys.Down]))
            {
                position.Y += speed * deltaTime;
            }
            if (InputSystem.IsKeyDown(keys[(int)PlayerKeys.Left]))
            {
                position.X -= speed * deltaTime;
            }
            if (InputSystem.IsKeyDown(keys[(int)PlayerKeys.Right]))
            {
                position.X += speed * deltaTime;
            }
            if (InputSystem.IsLeftDown())
            {
                UseWeapon(gameTime);
            }

            // Keep player within screen bounds
            position.X = MathHelper.Clamp(position.X, 0, Game1.ScreenSize.X - hitbox.Width);
            position.Y = MathHelper.Clamp(position.Y, 0, Game1.ScreenSize.Y - hitbox.Height);

            hitbox.Location = position.ToPoint();
        }

        public Projectile TryShoot(GameTime gameTime, Vector2 direction, Texture2D projTexture)
        {
            if (weapon == null) return null;
            var spawn = position + new Vector2(hitbox.Width / 2, hitbox.Height / 2);
            return weapon.TryShoot(gameTime, spawn, direction, projTexture, this);
        }

        public void UseWeapon(GameTime gameTime)
        {
            Vector2 mousePos = InputSystem.GetMousePosition();
            Vector2 direction = mousePos - (position + new Vector2(hitbox.Width / 2, hitbox.Height / 2));
            var projectile = TryShoot(gameTime, direction, projectileTexture);
            if (projectile != null)
            {
                projectiles.Add(projectile);
            }

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (hp <= 0)
                color = ColorBlink(Color.Red, Color.White, gameTime, 2f);
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
    }

    public enum PlayerKeys
    {
        Up = 0,
        Left,
        Down,
        Right
    }
}

