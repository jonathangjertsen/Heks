using System;
using UnityEngine;

[Serializable]
public class BaseCreature
{
    [Header("Health")]
    public float maxHealth;
    public float regenPer = 0.02f;

    [Space] [Header("Physics")]
    [SerializeField] protected CreaturePhysicsProperties physicsProperties;

    [Space] [Header("Damage")]
    public int hurtTimerTop = 60;

    [Space] [Header("Death timing")]
    public int deathToShrinkStartTimerTop = 100;
    public int shrinkTimerTop = 200;

    public FlipXCollection FlipXItems { get; protected set; }
    public BarCollection bars;
    public CreatureHealth health;
    public TimerCollection timers;
    public ICreaturePhysics physics;

    public void Init(
        IRigidBody2d rigidBody2d,
        ITransform transform,
        IBarDisplay healthBar
    )
    {
        physics = new CreaturePhysics(rigidBody2d, transform, physicsProperties);

        bars = new BarCollection();
        bars.Add(healthBar);
        health = new CreatureHealth(healthBar, maxHealth);

        timers = new TimerCollection();
        timers.Add("deathToShrinkStart", new Timer(deathToShrinkStartTimerTop, () => timers.Start("shrink")));
        timers.Add("shrink", new Timer(shrinkTimerTop, null, onTick: Shrinking));
        timers.Add("hurt", new Timer(hurtTimerTop, null));

        FlipXItems = new FlipXCollection();
        FlipXItems.Add(bars);
        FlipXItems.Add(physics);
    }

    public void SetOnDeathStartedCallback(CreatureHealth.OnZeroHealth callback)
    {
        health.SetZeroHealthCallback(() => {
            callback();
            Die();
        });
    }

    public void SetOnDeathFinishedCallback(Timer.Timeout callback)
    {
        timers.SetTimeoutCallback("shrink", callback);
    }

    public void SetOnHurtStartedCallback(CreatureHealth.OnHurt callback)
    {
        health.SetHurtCallback(callback);
    }

    public void SetOnHurtFinishedCallback(Timer.Timeout callback)
    {
        timers.SetTimeoutCallback("hurt", callback);
    }

    public bool FlipX
    {
        get => FlipXItems.FlipX;
        set => FlipXItems.FlipX = value;
    }

    private void Shrinking()
    {
        float proportion = timers.Value("shrink") / (float)shrinkTimerTop;
        physics.Size = physics.InitialSize * proportion;
    }

    public void Die()
    {
        timers.StopAll();
        timers.Start("deathToShrinkStart");
    }

    public void FixedUpdate()
    {
        RegenerateHealth();
        timers.TickAll();
    }

    public void Hurt(float damage, float recoilTorque = 0f)
    {
        if (health.Health > 0)
        {
            timers.Start("hurt");
            health.Hurt(damage);
            physics.Recoil(recoilTorque);
        }
    }

    private void RegenerateHealth()
    {
        health.Heal(regenPer);
    }
}

public abstract class BaseCreatureBehaviour<StateEnum> : MonoBehaviour where StateEnum : struct, Enum
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
    public PlayerBehaviour player;

    protected void Start()
    {
        creature.Init(
            new WrapperRigidbody2d(GetComponent<Rigidbody2D>()),
            new WrapperTransform(transform),
            healthBar
        );
        creature.SetOnDeathFinishedCallback(() => Destroy(this));
        creature.timers.logCallbacks = logTimerCallbacks;

        fsm = new CreatureFsm<StateEnum>(this)
        {
            logChanges = logFsmChanges
        };

        AddFsmStates();
    }

    protected void FixedUpdate()
    {
        creature.FixedUpdate();
    }

    protected virtual void AddFsmStates()
    {
    }
}
