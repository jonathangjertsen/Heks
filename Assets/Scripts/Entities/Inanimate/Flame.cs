using System;
using UnityEngine;

[Serializable]
public class Flame : BaseCollisionSystemParticipator, IDealsStatusEffect
{
    private Vector2 position;
    public Vector2 Position() {
        return position;
    }

    public void Init(Vector2 position)
    {
        this.position = position;
    }

    public override void TriggeredWith(ICollisionSystemParticipator other)
    {
        CollisionSystem.RegisterCollision(other, this);
    }

    public IStatusEffect DealStatusEffect(ITakesStatusEffect taker)
    {
        var effect = ScriptableObject.CreateInstance<StatusEffect>();
        effect.Type = StatusEffectType.Burn;
        effect.Intensity = 20;
        return effect;
    }
}

