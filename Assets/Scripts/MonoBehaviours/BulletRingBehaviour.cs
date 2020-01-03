using System;
using UnityEngine;

public class BulletRingBehaviour : SpellBehaviour, IDealsDamage
{
    public float collisionAttack = 30;
    public float CollisionAttack { get => collisionAttack; set => collisionAttack = value; }

    [SerializeField] float ringSize;
    [SerializeField] float maxOutwardSpeed;
    [SerializeField] int maxNum;

    public void DealDamage(float amount)
    {
        collisionAttack /= 2;
    }

    override public void Launch(Vector2 initialVelocity, float charge, bool flipX)
    {
        float outwardSpeed = maxOutwardSpeed * charge;
        int num = (int)(maxNum * charge);

        for (int i = 0; i < num; i++)
        {
            Vector3 offset = new Vector3(
                (float)Math.Cos(2 * Math.PI * i / num),
                (float)Math.Sin(2 * Math.PI * i / num),
                0
            );
            Vector3 normalizedOffset = offset.normalized;
            Vector2 velocityOffset = new Vector2(
                normalizedOffset.x * outwardSpeed * charge,
                normalizedOffset.y * outwardSpeed * charge
            );

            BulletRingBehaviour child;
            if (i == 0)
            {
                child = this;
            }
            else
            {
                child = Instantiate(this, transform.position + offset * ringSize, transform.rotation);
            }
            child.physics.Accelerate(initialVelocity + velocityOffset);
        }

        physics.Translate(new Vector2(ringSize, 0));
    }
}
