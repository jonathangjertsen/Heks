using System;
using UnityEngine;

public abstract class BaseCreatureBehaviour<StateEnum> : MonoBehaviour, ICreatureControllerWrapper, ICollisionSystemParticipatorWrapper, IHasSpriteRendererAndAudioSource where StateEnum : struct, Enum
{
    public BaseCreature creature;
    protected CreatureFsm<StateEnum> fsm;

    [Space]
    [Header("Debug")]
    public bool logFsmChanges = false;
    public bool logTimerCallbacks = false;

    [Space]
    [Header("Physics")]
    [SerializeField] protected CreaturePhysicsProperties physicsProperties;

    [Space]
    [Header("Behaviour references")]
    public BarBehaviour healthBar;

    protected void Start()
    {
        NotNull.Check(healthBar);
        NotNull.Check(physicsProperties);

        var physics = new CreaturePhysics(
            new WrapperRigidbody2d(GetComponent<Rigidbody2D>()),
            new WrapperTransform(transform),
            physicsProperties
        );
        creature.Init(physics, healthBar);
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

    public SpriteRenderer GetSpriteRenderer()
    {
        return gameObject.GetComponent<SpriteRenderer>();
    }

    public AudioSource GetAudioSource()
    {
        return gameObject.GetComponent<AudioSource>();
    }
}
