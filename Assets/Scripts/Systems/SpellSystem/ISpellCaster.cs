using UnityEngine;

public interface ISpellCaster : IFlipX
{
    void Cast(Vector2 initialVelocity, float charge);
}
