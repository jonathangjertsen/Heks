using UnityEngine;

public class SpellBehaviour : MonoBehaviour, ISpell
{
    [SerializeField] int liveTimerTop = 50;
    private int liveTimer;

    protected BulletPhysics physics;

    public void Awake()
    {
        physics = new BulletPhysics(
            new WrapperRigidbody2d(GetComponent<Rigidbody2D>()),
            new WrapperTransform(transform)
        );
    }

    public virtual void Launch(Vector2 initialVelocity, float charge, bool flipX)
    {
    }

    public Sprite GetSprite()
    {
        return GetComponent<SpriteRenderer>().sprite;
    }

    public Color GetColor()
    {
        return GetComponent<SpriteRenderer>().color;
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
