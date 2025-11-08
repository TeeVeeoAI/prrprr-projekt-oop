using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace prrprr_projekt_oop.Entities
{
    public class BaseEntity
    {
        protected Vector2 position;
        protected Vector2 velocity;
        protected Rectangle hitbox;
        protected Texture2D texture;
        protected Color color;

        public Rectangle Hitbox
        {
            get => hitbox;
        }
        public Vector2 Position
        {
            get => position;
        }

        public BaseEntity(Vector2 position, Vector2 velocity, Vector2 heightAndWidth, Texture2D texture)
        {
            this.position = position;
            this.velocity = velocity;
            this.hitbox = new Rectangle((int)position.X, (int)position.Y, (int)heightAndWidth.X, (int)heightAndWidth.Y);
            this.texture = texture;
        }

        public virtual void Update(GameTime gameTime) { }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, hitbox, color);
        }
    }
}