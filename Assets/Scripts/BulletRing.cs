using System;
using UnityEngine;

public class BulletRing : Spell
{
    public float ringSize;
    public float maxOutwardSpeed;
    public int maxNum;
    private BulletPhysics physics;

    private void Awake()
    {
        physics = new BulletPhysics(this);
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

            BulletRing child;
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
