using System;
using UnityEngine;

public abstract class BaseCreatureBehaviour<StateEnum> : MonoBehaviour, ICreatureControllerWrapper, ICollisionSystemParticipatorWrapper where StateEnum : struct, Enum
{
    public BaseCreature creature;
    protected CreatureFsm<StateEnum> fsm;

    [Space]
    [Header("Debug")]
    public bool logFsmChanges = false;
    public bool logTimerCallbacks = false;

    [Space]
    [Header("Behaviour references")]
    public BarBehaviour healthBar;

    protected void Start()
    {
        creature.Init(
            new WrapperRigidbody2d(GetComponent<Rigidbody2D>()),
            new WrapperTransform(transform),
            healthBar
        );
        creature.SetDeathFinishedCallback(() => Destroy(this));
        creature.timers.logCallbacks = logTimerCallbacks;

        fsm = new CreatureFsm<StateEnum>(this)
        {
            logChanges = logFsmChanges
        };

        AddFsmStates();
    }

    protected virtual void AddFsmStates()
    {
    }

    public abstract ICreatureController GetCreatureController();

    private void FixedUpdate() => GetCreatureController().FixedUpdate();
    private void OnCollisionEnter2D(Collision2D collision)
    {
        var wrap = collision.gameObject.GetComponent<ICollisionSystemParticipatorWrapper>();
        if (wrap != null)
        {
            var other = wrap.GetCollisionSystemParticipator();
            GetCreatureController().CollidedWith(other);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        var wrap = collision.gameObject.GetComponent<ICollisionSystemParticipatorWrapper>();
        if (wrap != null)
        {
            var other = wrap.GetCollisionSystemParticipator();
            GetCreatureController().ExitedCollisionWith(other);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var wrap = collision.gameObject.GetComponent<ICollisionSystemParticipatorWrapper>();
        if (wrap != null)
        {
            var other = wrap.GetCollisionSystemParticipator();
            GetCreatureController().TriggeredWith(other);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        var wrap = collision.gameObject.GetComponent<ICollisionSystemParticipatorWrapper>();
        if (wrap != null)
        {
            var other = wrap.GetCollisionSystemParticipator();
            GetCreatureController().ExitedTriggerWith(other);
        }
    }
    public ICollisionSystemParticipator GetCollisionSystemParticipator() => GetCreatureController();
}
