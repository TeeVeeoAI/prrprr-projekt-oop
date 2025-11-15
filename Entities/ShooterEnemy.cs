using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using prrprr_projekt_oop.Systems;
using prrprr_projekt_oop.Entities.Weapons;
using prrprr_projekt_oop.Entities.Projectiles;

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

        public ShooterEnemy(Texture2D texture, Texture2D projectileTexture, Player player = null, float speed = 150f)
            : base(EnemySpawnerSystem.PickSpawnPos(), new Vector2(50, 50), texture, Color.Cyan, 1, 3, speed)
        {
            this.targetPlayer = player;
            this.projectileTexture = projectileTexture;
            this.weapon = new Rifle(1.5f);
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
                var projectile = weapon.TryShoot(gameTime, selfCenter, shootDir, projectileTexture, this);
                if (projectile != null)
                {
                    projectiles.Add(projectile);
                }
            }

            hitbox.Location = position.ToPoint();
        }
    }
}
