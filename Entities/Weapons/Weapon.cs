using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using prrprr_projekt_oop.Entities.Projectiles;
using prrprr_projekt_oop.Entities;

namespace prrprr_projekt_oop.Entities.Weapons
{
    public abstract class Weapon
    {
        protected double lastShotTime = -9999;

        protected float fireRateSeconds;
        protected int damage;
        protected Vector2 muzzleOffset;
        protected float projectileSpeed;
        protected Color projectileColor;

        public int Damage { get => damage; }
        public float FireRateSeconds { get => fireRateSeconds; }
        public float ProjectileSpeed { get => projectileSpeed; }

        protected Weapon(float fireRateSeconds = 0.25f, int damage = 1, Vector2? muzzleOffset = null, float projectileSpeed = 300f)
        {
            this.fireRateSeconds = fireRateSeconds;
            this.damage = damage;
            this.muzzleOffset = muzzleOffset ?? Vector2.Zero;
            this.projectileSpeed = projectileSpeed;
        }

        public virtual Projectile TryShoot(GameTime gameTime, Vector2 origin, Vector2 direction, Texture2D projTexture, BaseEntity owner = null)
        {
            if (projectileColor == Color.Transparent)
            {
                projectileColor = owner != null ? owner.GetType() == typeof(Player) ? Color.Green : Color.Red : Color.White;
            }
            double now = gameTime.TotalGameTime.TotalSeconds;
            if (now - lastShotTime < fireRateSeconds) return null;
            if (direction == Vector2.Zero) return null;

            lastShotTime = now;

            Vector2 dir = Vector2.Normalize(direction);
            Vector2 velocity = dir * projectileSpeed;
            Vector2 spawnPos = origin + muzzleOffset;

            Vector2 size = new Vector2(8, 8);

            Projectile p = new Projectile(spawnPos, velocity, size, projTexture, 1, projectileColor, damage, owner);
            return p;
        }
    }
}
