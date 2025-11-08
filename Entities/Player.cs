using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using prrprr_projekt_oop.Systems;

namespace prrprr_projekt_oop.Entities
{
    public class Player : BaseEntity
    {

        protected Keys[] keys;

        public Player(Vector2 heightAndWidth, Texture2D texture)
        : base(Game1.ScreenSize / 2 - heightAndWidth, new Vector2(5, 5), heightAndWidth, texture)
        {
            color = Color.SeaGreen;
            keys = new Keys[]
            {
                Keys.W,
                Keys.A,
                Keys.S,
                Keys.D,
                Keys.V
            };
        }

        public override void Update(GameTime gameTime)
        {

            if (InputSystem.IsKeyDown(keys[(int)PlayerKeys.Up]))
            {
                position.Y -= velocity.Y;
            }
            if (InputSystem.IsKeyDown(keys[(int)PlayerKeys.Down]))
            {
                position.Y += velocity.Y;
            }
            if (InputSystem.IsKeyDown(keys[(int)PlayerKeys.Left]))
            {
                position.X -= velocity.X;
            }
            if (InputSystem.IsKeyDown(keys[(int)PlayerKeys.Right]))
            {
                position.X += velocity.X;
            }
            if (InputSystem.IsKeyDown(keys[(int)PlayerKeys.Shoot]))
            {
                Shoot(gameTime);
            }

            // Keep player within screen bounds
            position.X = MathHelper.Clamp(position.X, 0, Game1.ScreenSize.X - hitbox.Width);
            position.Y = MathHelper.Clamp(position.Y, 0, Game1.ScreenSize.Y - hitbox.Height);

            hitbox.Location = position.ToPoint();
        }

        public void Shoot(GameTime gameTime)
        {
            
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
        }
    }
    
    public enum PlayerKeys
    {
        Up = 0,
        Left,
        Down,
        Right,
        Shoot
    }
}

