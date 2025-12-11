using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using prrprr_projekt_oop.Systems;
using prrprr_projekt_oop.Entities.Weapons;
using prrprr_projekt_oop.Entities.Projectiles;
using prrprr_projekt_oop.Enums;

namespace prrprr_projekt_oop.Entities
{
    public class ShooterEnemy : BaseEnemy
    {
        private Player targetPlayer;
        private Weapon weapon;
        private Texture2D projectileTexture;
        private List<Projectile> projectiles = new List<Projectile>();

        public List<Projectile> Projectiles
        {
            get => projectiles;
        }

        public ShooterEnemy(Texture2D texture, Texture2D projectileTexture, Player player = null, float difficultyMultiplier = 1, float speed = 150f)
            : base(EnemySpawnerSystem.PickSpawnPos(), new Vector2(50, 50), texture, Color.Cyan, 1, 3, speed)
        {
            this.targetPlayer = player;
            this.projectileTexture = projectileTexture;
            this.weapon = new Rifle(1.5f / difficultyMultiplier, (int)(1*difficultyMultiplier));
            this.speed *= difficultyMultiplier;
            this.hp = (int)(this.hp * difficultyMultiplier);
            this.damage = (int)(this.damage * difficultyMultiplier);
        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (targetPlayer != null)
            {
                Vector2 playerCenter = targetPlayer.Position + new Vector2(targetPlayer.Hitbox.Width / 2f, targetPlayer.Hitbox.Height / 2f);
                Vector2 selfCenter = position + new Vector2(hitbox.Width / 2f, hitbox.Height / 2f);
                Vector2 dir = playerCenter - selfCenter;
                if (dir.LengthSquared() > 0.0001f)
                {
                    dir.Normalize();
                    position += dir * speed * deltaTime;
                }

                // Shoot at the player
                Vector2 shootDir = playerCenter - selfCenter;
                var spawned = weapon.TryShoot(gameTime, selfCenter, shootDir, projectileTexture, this);
                if (spawned != null && spawned.Count > 0)
                {
                    projectiles.AddRange(spawned);
                }
            }

            hitbox.Location = position.ToPoint();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 playerCenter = targetPlayer.Position + new Vector2(targetPlayer.Hitbox.Width / 2f, targetPlayer.Hitbox.Height / 2f);
            Vector2 selfCenter = position + new Vector2(hitbox.Width / 2f, hitbox.Height / 2f);
            Vector2 dir = playerCenter - selfCenter;
            spriteBatch.Draw(
                texture, 
                position + new Vector2(hitbox.Width / 2, hitbox.Height / 2), 
                null, 
                color, 
                (float)Math.Atan2(dir.Y, dir.X) + (float)Math.PI / 2f, 
                new Vector2(texture.Width / 2f, texture.Height / 2f), 
                hitbox.Size.ToVector2() / (texture.Bounds.Size.ToVector2()/2),
                SpriteEffects.None, 0f
            );
        }
    }
}
