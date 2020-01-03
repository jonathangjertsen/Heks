using UnityEngine;

public class BulletBehaviour : SpellBehaviour, IDealsDamage
{
    public float collisionAttack = 60;
    public float CollisionAttack { get => collisionAttack; set => collisionAttack = value; }

    public void DealDamage(float amount)
    {
        collisionAttack /= 2;
    }

    override public void Launch(Vector2 initialVelocity, float charge, bool flipX)
    {
        int flipXAsInt = flipX ? 1 : -1;
        Vector2 launchVelocity = new Vector2(-flipXAsInt, 1) * 5f * charge;
        physics.Accelerate(initialVelocity + launchVelocity);
        physics.Torque(5000f * charge * -flipXAsInt);
    }
}
