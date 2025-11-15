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

        public List<Projectile> Projectiles {
            get => projectiles;
        }

        public Player(Vector2 heightAndWidth, Texture2D texture, Texture2D projectileTexture)
        : base(Game1.ScreenSize / 2 - heightAndWidth, 400f, heightAndWidth, texture, 5, Color.SeaGreen)
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
            switch (HP)
            {
                case 5:
                    color = Color.SeaGreen;
                    break;
                case 4:
                    color = Color.YellowGreen;
                    break;
                case 3:
                    color = Color.Yellow;
                    break;
                case 2:
                    color = Color.Orange;
                    break;
                case 1:
                    color = Color.Red;
                    break;
                default:
                    color = ColorBlink(Color.Gray, Color.DarkRed, gameTime, 3f);
                    break;
            }
            base.Draw(gameTime, spriteBatch);
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

