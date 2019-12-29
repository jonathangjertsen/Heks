using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int liveTimerTop = 50;
    private int liveTimer;
    private BulletPhysics physics;

    void Awake()
    {
        physics = new BulletPhysics(this);
    }

    public void Launch(Vector2 ax, float torque)
    {
        physics.Accelerate(ax);
        physics.Torque(torque);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        liveTimer = liveTimerTop;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        liveTimer -= 1;
        if (liveTimer <= 0)
        {
            Destroy(gameObject);
        }
    }
}
