using UnityEngine;

public class SpellStraightBehaviour : SpellBehaviour, IDealsDamage
{
    public float collisionAttack = 100;
    public float CollisionAttack { get => collisionAttack; set => collisionAttack = value; }

    public void DealDamage(float amount)
    {
        collisionAttack /= 2;
    }

    override public void Launch(Vector2 initialVelocity, float charge, bool flipX)
    {
        int flipXAsInt = flipX ? 1 : -1;
        int launchY = initialVelocity.y >= 0.01f ? 1 : -1;
        Vector2 launchVelocity = new Vector2(-flipXAsInt, launchY) * 10f * charge;
        physics.Accelerate(initialVelocity + launchVelocity);
    }
}
