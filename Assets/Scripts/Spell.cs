using UnityEngine;

public class Spell : MonoBehaviour
{
    public int liveTimerTop = 50;
    private int liveTimer;

    public virtual void Launch(Vector2 initialVelocity, float charge, bool flipX)
    {
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
