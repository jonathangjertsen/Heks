using System;
using UnityEngine;

[Serializable]
public class BaseCreature
{
    [Header("Health")]
    public float maxHealth;
    public float regenPer = 0.02f;

    [Space] [Header("Physics")]
    public float axCoeffX = 0.01f;
    public float axCoeffY = 0.03f;
    public float rotCoeff = 1f;
    public float maxJerkX = 5f;
    public float maxJerkY = 5f;

    [Space] [Header("Debug")]
    public bool logFsmChanges = false;
    public bool logTimerCallbacks = false;

    [Space] [Header("Damage")]
    public int hurtTimerTop = 60;

    private readonly int deathToShrinkStartTimerTop = 100;
    private readonly int shrinkTimerTop = 200;

    public FlipXCollection flipXItems;
    public BarCollection bars;
    public CreatureHealth health;
    public TimerCollection timers;

    public ICreaturePhysics physics;
    public void SetPhysics(ICreaturePhysics physics)
    {
        this.physics = physics;
    }

    public void SetPhysicsFromBehaviour(MonoBehaviour bh)
    {
        physics = new CreaturePhysics(
            bh,
            axCoeff: new Vector2(axCoeffX, axCoeffY),
            rotCoeff: rotCoeff,
            maxJerk: new Vector2(maxJerkX, maxJerkY)
        );
    }

    public void Init(
        Timer.Timeout onDeathCompleted,
        IBarDisplay healthBar,
        CreatureHealth.OnZeroHealth onDeathStart,
        Timer.Timeout onHurtCompleted
     )
    {
        if (physics == null)
        {
            throw new Exception("Must call SetPhysics or SetPhysicsFromBehaviour before Init");
        }

        bars = new BarCollection();
        bars.Add(healthBar);
        health = new CreatureHealth(healthBar, maxHealth, onZeroHealth: onDeathStart);

        timers = new TimerCollection();
        timers.logCallbacks = logTimerCallbacks;
        timers.Add("deathToShrinkStart", new Timer(deathToShrinkStartTimerTop, ShrinkStart));
        timers.Add("shrink", new Timer(shrinkTimerTop, onDeathCompleted, onTick: Shrinking));
        timers.Add("hurt", new Timer(hurtTimerTop, onHurtCompleted));

        flipXItems = new FlipXCollection();
        flipXItems.Add(bars);
        flipXItems.Add(physics);
    }

    public bool FlipX
    {
        get => flipXItems.FlipX;
        set => flipXItems.FlipX = value;
    }

    private void Shrinking()
    {
        float proportion = timers.Value("shrink") / (float)shrinkTimerTop;
        physics.Size = physics.InitialSize * proportion;
    }

    private void ShrinkStart()
    {
        timers.Start("shrink");
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

    public void RegenerateHealth()
    {
        health.Health += regenPer;
    }

    public void Hurt(float damage, float recoilTorque = 0f)
    {
        timers.Start("hurt");
        health.Health -= damage;
        physics.Recoil(recoilTorque);
    }
}

public abstract class BaseCreatureBehaviour<StateEnum> : MonoBehaviour where StateEnum : struct, Enum
{
    public BaseCreature creature;
    protected CreatureFsm<StateEnum> fsm;

    [Space]
    [Header("Behaviour references")]
    public BarBehaviour healthBar;
    public PlayerBehaviour player;

    protected StateEnum FsmState
    {
        get => fsm.State;
        set => fsm.State = value;
    }

    public virtual void Die()
    {
        creature.Die();
    }

    public virtual void OnHurtCompleted()
    {
    }

    protected void Start()
    {
        creature.SetPhysicsFromBehaviour(this);
        creature.Init(OnDeathCompleted, healthBar, Die, OnHurtCompleted);

        fsm = new CreatureFsm<StateEnum>(this);
        fsm.logChanges = creature.logFsmChanges;
    }

    protected void FixedUpdate()
    {
        creature.FixedUpdate();
    }

    private void OnDeathCompleted()
    {
        Destroy(this);
    }
}
