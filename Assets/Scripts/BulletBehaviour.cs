using UnityEngine;

public class BulletBehaviour : SpellBehaviour
{
    override public void Launch(Vector2 initialVelocity, float charge, bool flipX)
    {
        int flipXAsInt = flipX ? 1 : -1;
        physics.Accelerate(initialVelocity + new Vector2(5f * charge * -flipXAsInt, 5f * charge));
        physics.Torque(5000f * charge * -flipXAsInt);
    }
}
