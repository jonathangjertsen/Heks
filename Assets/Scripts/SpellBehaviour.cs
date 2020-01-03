using UnityEngine;

public class SpellBehaviour : MonoBehaviour, ISpell, ISysCollisionParticipator
{
    [SerializeField] int liveTimerTop = 50;
    private int liveTimer;
    private bool touching = false;

    protected BulletPhysics physics;
    public bool As<T>(out T converted) => ConvertToInterface.As(this, out converted);
    private TimerCollection timers;

    public void Awake()
    {
        physics = new BulletPhysics(
            new WrapperRigidbody2d(GetComponent<Rigidbody2D>()),
            new WrapperTransform(transform)
        );
        timers = new TimerCollection();
        timers.Add("contact", new Timer(liveTimerTop, onTimeout: Remove));
    }

    public void FixedUpdate()
    {
        timers.TickAll();
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

    private void Remove()
    {
        Destroy(gameObject);
    }

    public void CollidedWith(ISysCollisionParticipator other) => timers.Start("contact");
    public void ExitedCollisionWith(ISysCollisionParticipator other) => timers.Pause("contact");
    public void TriggeredWith(ISysCollisionParticipator other) { }
    public void ExitedTriggerWith(ISysCollisionParticipator other) { }
    public ISysCollisionParticipator GetSysCollisionParticipator() => this;
}
