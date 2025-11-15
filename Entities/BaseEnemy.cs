using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using prrprr_projekt_oop.Systems;

namespace prrprr_projekt_oop.Entities
{
    public class BaseEnemy : BaseEntity
    {
        protected bool damageByPlayer = false;
        private Player targetPlayer;
        public bool DamageByPlayer { get => damageByPlayer; }

        public BaseEnemy(Texture2D texture, Player player = null, float speed = 200f)
            : base(EnemySpawnerSystem.PickSpawnPos(), speed, new Vector2(50, 50), texture, 3, Color.Violet /* Is Purple because Player can be Red at 1 HP */)
        {
            this.targetPlayer = player;
        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (targetPlayer != null)
            {
                // Move towards the player's current center position
                Vector2 playerCenter = targetPlayer.Position + new Vector2(targetPlayer.Hitbox.Width / 2f, targetPlayer.Hitbox.Height / 2f);
                Vector2 selfCenter = position + new Vector2(hitbox.Width / 2f, hitbox.Height / 2f);
                Vector2 dir = playerCenter - selfCenter;
                if (dir.LengthSquared() > 0.0001f)
                {
                    dir.Normalize();
                    position += dir * speed * deltaTime;
                }
            }
            else
            {
                // Fallback downward movement if no player assigned
                position.Y += speed * deltaTime;
            }

            OutOfBoundsCheck();

            hitbox.Location = position.ToPoint();
        }

        public void OutOfBoundsCheck()
        {
            if (position.Y > Game1.ScreenSize.Y)
            {
                hp = 0; // Mark for removal
            }
        }

        public void TakeDamage(int damage, bool byPlayer = false)
        {
            hp -= damage;
            if (byPlayer)
            {
                damageByPlayer = true;
            }
        }

        public void Kill(bool byPlayer = false)
        {
            if (byPlayer)
            {
                damageByPlayer = true;
            }
            hp = 0;
        }

        public bool IsDead()
        {
            return hp <= 0; 
        }

    }
}