﻿public static class CollisionSystem
{
    public static float minDefense = 0.01f;

    public static void RegisterCollision(ICollisionSystemParticipator first, ICollisionSystemParticipator other)
    {
        CollisionDamage(first, other);
        CollisionStatusEffect(first, other);
    }

    private static void CollisionDamage(ICollisionSystemParticipator first, ICollisionSystemParticipator other)
    {
        if (first.As(out ITakesDamage taker) && other.As(out IDealsDamage dealer))
        {
            float defense = UnityEngine.Mathf.Max(minDefense, taker.CollisionDefense);
            float damage = dealer.CollisionAttack / defense;
            taker.TakeDamage(damage);
            dealer.DealDamage(damage);
            // Debug.Log($"Dealt {damage} from {dealer} to {taker}");
        }
    }

    private static void CollisionStatusEffect(ICollisionSystemParticipator first, ICollisionSystemParticipator other)
    {
        if (first.As(out ITakesStatusEffect taker) && other.As(out IDealsStatusEffect dealer))
        {
            IStatusEffect statusEffect = dealer.DealStatusEffect(taker);
            taker.TakeStatusEffect(statusEffect);
            // Debug.Log($"Dealt {statusEffect} from {dealer} to {taker}")
        }
    }
}