using System;
using UnityEngine;

public abstract class BaseCreatureBehaviour<StateEnum> : BaseCollisionSystemParticipatorWrapper, ICreatureControllerWrapper, ICollisionSystemParticipatorWrapper, IHasSpriteRendererAndAudioSource where StateEnum : struct, Enum
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

    public abstract ICreatureController GetCreatureController();
    override public ICollisionSystemParticipator GetCollisionSystemParticipator() => GetCreatureController();

    protected void Start()
    {
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

    private void FixedUpdate() => GetCreatureController().FixedUpdate();

    public SpriteRenderer GetSpriteRenderer()
    {
        return gameObject.GetComponent<SpriteRenderer>();
    }

    public AudioSource GetAudioSource()
    {
        return gameObject.GetComponent<AudioSource>();
    }
}
