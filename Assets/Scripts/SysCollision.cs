using UnityEngine;

public static class SysCollision
{
    public static float minDefense = 0.01f;

    public static void RegisterCollision(ISysCollisionParticipator first, ISysCollisionParticipator other)
    {
        if (first.As(out ITakesDamage attacked))
        {
            if (other.As(out IDealsDamage attacker))
            {
                float defense = Mathf.Max(minDefense, attacked.CollisionDefense);
                float damage = attacker.CollisionAttack / defense;
                attacked.TakeDamage(damage);

                // Debug.Log($"Dealt {damage} from {attacker} to {attacked}");
            }
        }
    }
}
