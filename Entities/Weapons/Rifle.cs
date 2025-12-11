using Microsoft.Xna.Framework;

namespace prrprr_projekt_oop.Entities.Weapons
{
    public class Rifle : Weapon
    {
        public Rifle(float fireRateSeconds = 0.15f, int damage = 1) : base(fireRateSeconds, damage, null, 600f)
        {
        }
    }
}
