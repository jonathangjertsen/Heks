using UnityEngine;

public class Bullet : Spell
{
    public int liveTimerTop = 50;
    private int liveTimer;
    private BulletPhysics physics;

    private void Awake()
    {
        physics = new BulletPhysics(this);
    }

    override public void Launch(Vector2 initialVelocity, float charge, bool flipX)
    {
        int flipXAsInt = flipX ? 1 : -1;
        physics.Accelerate(initialVelocity + new Vector2(0.1f * charge * -flipXAsInt, 0.1f * charge));
        physics.Torque(100f * charge * -flipXAsInt);
    }

    private void OnCollisionEnter2D()
    {
        liveTimer = liveTimerTop;
    }

    private void OnCollisionStay2D()
    {
        liveTimer -= 1;
        if (liveTimer <= 0)
        {
            Destroy(gameObject);
        }
    }
}