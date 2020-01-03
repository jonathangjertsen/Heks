using UnityEngine;

public static class SysCollision
{
    public static float minDefense = 0.01f;

    public static void RegisterCollision(ISysCollisionParticipator first, ISysCollisionParticipator other)
    {
        CollisionDamage(first, other);
    }

    private static void CollisionDamage(ISysCollisionParticipator first, ISysCollisionParticipator other)
    {
        if (first.As(out ITakesDamage attacked) && other.As(out IDealsDamage attacker))
        {
            float defense = Mathf.Max(minDefense, attacked.CollisionDefense);
            float damage = attacker.CollisionAttack / defense;
            attacked.TakeDamage(damage);
            attacker.DealDamage(damage);
            // Debug.Log($"Dealt {damage} from {attacker} to {attacked}");
        }
    }
}
