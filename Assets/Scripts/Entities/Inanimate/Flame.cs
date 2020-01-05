using System;

[Serializable]
public class Flame : BaseCollisionSystemParticipator, IDealsStatusEffect
{
    public override void TriggeredWith(ICollisionSystemParticipator other)
    {
        CollisionSystem.RegisterCollision(other, this);
    }

    public IStatusEffect DealStatusEffect(ITakesStatusEffect taker)
    {
        return new StatusEffect(StatusEffectType.Burn, 20);
    }
}

