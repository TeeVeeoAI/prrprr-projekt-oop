using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using prrprr_projekt_oop.Entities;

namespace prrprr_projekt_oop.Entities.Projectiles
{
    public class Projectile : BaseEntity
    {
        private int damage;
        private BaseEntity owner;
        private bool isExpired;
        public BaseEntity Owner
        {
            get => owner;
        }
        public bool IsExpired
        {
            get => isExpired;
        }

        public Projectile(Vector2 position, Vector2 velocity, Vector2 size, Texture2D texture, int hp, Color color, int damage, BaseEntity owner = null)
        : base(position, velocity.Length(), size, texture, hp, color)
        {
            this.damage = damage;
            this.owner = owner;
            this.velocity = velocity;
        }

        public int Damage { get => damage; }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            position += this.velocity * deltaTime;

            hitbox.Location = position.ToPoint();

            if (position.X < -hitbox.Width || position.Y < -hitbox.Height || position.X > Game1.ScreenSize.X + Hitbox.Width || position.Y > Game1.ScreenSize.Y + Hitbox.Height)
                isExpired = true;
        }

        public void Expire()
        {
            isExpired = true;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                texture, 
                position + new Vector2(hitbox.Width / 2, hitbox.Height / 2), 
                null, 
                color, 
                (float)Math.Atan2(velocity.Y, velocity.X), 
                new Vector2(texture.Width / 2f, texture.Height / 2f), 
                hitbox.Size.ToVector2() / texture.Bounds.Size.ToVector2(),
                SpriteEffects.None, 0f
            );
        }
    }
}
