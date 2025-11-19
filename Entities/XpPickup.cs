using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using prrprr_projekt_oop.Forms;

namespace prrprr_projekt_oop.Entities
{
    public class XpPickup : BaseEntity
    {
        private Circle collider;
        private int xpAmount;

        public bool IsExpired { get; private set; } = false;
        public Circle Collider => collider;
        public int XpAmount => xpAmount;

        public XpPickup(Vector2 centerPosition, int xpAmount, float radius, Texture2D texture)
            : base(centerPosition - new Vector2(radius, radius), 0f, new Vector2(radius * 2f, radius * 2f), texture, 0, Color.White)
        {
            this.xpAmount = xpAmount;
            this.texture = texture;
            this.collider = new Circle(centerPosition, radius);
        }

        public override void Update(GameTime gameTime)
        {
            // Update collider and hitbox position

            // Keep hitbox centered on collider position
            Vector2 center = collider.Pos;
            hitbox.X = (int)(center.X - hitbox.Width / 2f);
            hitbox.Y = (int)(center.Y - hitbox.Height / 2f);
        }

        // Pull the pickup toward a target position. dt is seconds elapsed.
        public void Attract(Vector2 target, float pullSpeed, float dt)
        {
            Vector2 center = collider.Pos;
            Vector2 dir = target - center;
            float dist = dir.Length();
            if (dist <= 0.001f) return;
            dir /= dist; // normalize

            // Move a step toward the target, clamp to avoid overshoot
            float move = pullSpeed * dt;
            if (move >= dist)
            {
                center = target;
            }
            else
            {
                center += dir * move;
            }

            collider.ChangePos(center);
            hitbox.X = (int)(center.X - hitbox.Width / 2f);
            hitbox.Y = (int)(center.Y - hitbox.Height / 2f);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, new Rectangle(hitbox.X, hitbox.Y, hitbox.Width*2, hitbox.Height*2), Color.White);
        }
    }
}
