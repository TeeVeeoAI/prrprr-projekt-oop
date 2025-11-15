using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using prrprr_projekt_oop.Entities.Projectiles;
using prrprr_projekt_oop.Entities;

namespace prrprr_projekt_oop.Entities.Weapons
{
    public class Weapon
    {
        private double lastShotTime = -9999;

        private float fireRateSeconds;
        private int damage;
        private Vector2 muzzleOffset;
        private float projectileSpeed;
        public int Damage { get => damage; }
        public float FireRateSeconds { get => fireRateSeconds; }
        public float ProjectileSpeed { get => projectileSpeed; }

        public Weapon(float fireRateSeconds = 0.25f, int damage = 1, Vector2? muzzleOffset = null, float projectileSpeed = 300f)
        {
            this.fireRateSeconds = fireRateSeconds;
            this.damage = damage;
            this.muzzleOffset = muzzleOffset ?? Vector2.Zero;
            this.projectileSpeed = projectileSpeed;
        }

        public virtual Projectile TryShoot(GameTime gameTime, Vector2 origin, Vector2 direction, Texture2D projTexture, BaseEntity owner = null)
        {
            var now = gameTime.TotalGameTime.TotalSeconds;
            if (now - lastShotTime < fireRateSeconds) return null;
            if (direction == Vector2.Zero) return null;

            lastShotTime = now;

            var dir = Vector2.Normalize(direction);
            var velocity = dir * projectileSpeed;
            var spawnPos = origin + muzzleOffset;

            Vector2 size = new Vector2(8, 8);

            var p = new Projectile(spawnPos, velocity, size, projTexture, 1, Color.White, damage, owner);
            return p;
        }
    }
}
