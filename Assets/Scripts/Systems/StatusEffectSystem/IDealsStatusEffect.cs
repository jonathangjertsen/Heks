using UnityEngine;

public interface IDealsStatusEffect
{
    IStatusEffect DealStatusEffect(ITakesStatusEffect taker);
    Vector2 Position();
}
