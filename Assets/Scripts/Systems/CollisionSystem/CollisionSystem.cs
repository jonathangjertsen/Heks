using System;
using System.Collections.Generic;
using UnityEngine;

public static class CollisionSystem
{
    public static float minDefense = 0.01f;
    public static float minTimeBetweenCollisionsOfSameObject = 0.5f;
    public static Dictionary<Tuple<ICollisionSystemParticipator, ICollisionSystemParticipator>, float> lastCollision;

    static CollisionSystem()
    {
        lastCollision = new Dictionary<Tuple<ICollisionSystemParticipator, ICollisionSystemParticipator>, float>();
    }

    public static void RegisterCollision(ICollisionSystemParticipator first, ICollisionSystemParticipator other)
    {
        if (!DebounceCollision(first, other))
        {
            return;
        }
        CollisionDamage(first, other);
        CollisionStatusEffect(first, other);
    }

    private static bool DebounceCollision(ICollisionSystemParticipator first, ICollisionSystemParticipator other)
    {
        float time = Time.fixedTime;
        var tuple = new Tuple<ICollisionSystemParticipator, ICollisionSystemParticipator>(first, other);
        if (lastCollision.TryGetValue(tuple, out float value))
        {
            if (time - value < minTimeBetweenCollisionsOfSameObject)
            {
                return false;
            }
            lastCollision[tuple] = time;
        }
        else
        {
            lastCollision.Add(tuple, time);
        }
        return true;
    }

    private static void CollisionDamage(ICollisionSystemParticipator first, ICollisionSystemParticipator other)
    {
        if (first.As(out ITakesDamage taker) && other.As(out IDealsDamage dealer))
        {
            float defense = UnityEngine.Mathf.Max(minDefense, taker.CollisionDefense);
            float damage = dealer.CollisionAttack / defense;
            taker.TakeDamage(damage);
            dealer.DealDamage(damage);
        }
    }

    private static void CollisionStatusEffect(ICollisionSystemParticipator first, ICollisionSystemParticipator other)
    {
        if (first.As(out ITakesStatusEffect taker) && other.As(out IDealsStatusEffect dealer))
        {
            taker.TakeStatusEffect(dealer.DealStatusEffect(taker), dealer);
        }
    }
}
