using prrprr_projekt_oop.Entities;

namespace prrprr_projekt_oop.Systems
{
    public static class CollisionSystem
    {
        public static bool CheckPlayerEnemyCollision(Player player, BaseEnemy enemy)
        {
            return player.Hitbox.Intersects(enemy.Hitbox);
        }
    }
}
