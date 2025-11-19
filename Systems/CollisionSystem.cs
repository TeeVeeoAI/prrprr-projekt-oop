using Microsoft.Xna.Framework;
using prrprr_projekt_oop.Entities;
using prrprr_projekt_oop.Forms;

namespace prrprr_projekt_oop.Systems
{
    public static class CollisionSystem
    {
        public static bool CheckPlayerEnemyCollision(Player player, BaseEnemy enemy)
        {
            return player.Hitbox.Intersects(enemy.Hitbox);
        }

        public static bool CheckPlayerPickupCollision(Player player, Circle pickupCircle)
        {
            return pickupCircle.Intersects(player.Hitbox);
        }
    }
}
