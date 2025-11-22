using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using prrprr_projekt_oop.Entities.Projectiles;
using prrprr_projekt_oop.Entities;

namespace prrprr_projekt_oop.Entities.Weapons
{
    public class Shotgun : Weapon
    {
        public Shotgun() : base(0.5f, 1, Vector2.Zero, 400f)
        {
        }

        public override Projectile TryShoot(GameTime gameTime, Vector2 origin, Vector2 direction, Texture2D projTexture, BaseEntity owner = null)
        {
            double now = gameTime.TotalGameTime.TotalSeconds;
            if (now - lastShotTime < fireRateSeconds) return null;
            if (direction == Vector2.Zero) return null;

            lastShotTime = now;

            // Fire 5 projectiles in a spread pattern (±15 degrees cone)
            int projectileCount = 5;
            float spreadAngle = 15f * (float)Math.PI / 180f; //radians
            float baseAngle = (float)Math.Atan2(direction.Y, direction.X);

            Projectile firstProjectile = null;
            Vector2 size = new Vector2(8, 8);

            for (int i = 0; i < projectileCount; i++)
            {
                float angleOffset = (i - (projectileCount - 1) / 2f) * (spreadAngle * 2 / (projectileCount - 1));
                float projectileAngle = baseAngle + angleOffset;

                Vector2 projectileDir = new Vector2(
                    (float)Math.Cos(projectileAngle),
                    (float)Math.Sin(projectileAngle)
                );

                Vector2 velocity = projectileDir * projectileSpeed;
                Vector2 spawnPos = origin + muzzleOffset;

                Projectile p = new Projectile(spawnPos, velocity, size, projTexture, 1, Color.White, damage, owner);

                if (firstProjectile == null)
                {
                    firstProjectile = p;
                }
            }

            return firstProjectile;
        }
    }
}
