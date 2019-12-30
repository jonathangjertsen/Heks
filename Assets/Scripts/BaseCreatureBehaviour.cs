using System;
using UnityEngine;

[Serializable]
public class BaseCreature
{
    public float maxHealth;
    public float axCoeffX = 0.01f;
    public float axCoeffY = 0.03f;
    public float rotCoeff = 1f;
    public float maxVelocityX = 10.0f;
    public float maxVelocityY = 10.0f;
    public float maxJerkX = 5f;
    public float maxJerkY = 5f;
    public bool logFsmChanges = false;
    public bool logTimerCallbacks = false;
    private readonly int deathToShrinkStartTimerTop = 100;
    private readonly int shrinkTimerTop = 200;

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

    public void InitTimers(Timer.Timeout onDeathCompleted)
    {
        timers = new TimerCollection();
        timers.logCallbacks = logTimerCallbacks;
        timers.Add("deathToShrinkStart", new Timer(deathToShrinkStartTimerTop, ShrinkStart));
        timers.Add("shrink", new Timer(shrinkTimerTop, onDeathCompleted, onTick: Shrinking));
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
        timers.TickAll();
    }
}

public abstract class BaseCreatureBehaviour<StateEnum> : MonoBehaviour where StateEnum : struct, Enum
{
    public BaseCreature creature;
    protected CreatureFsm<StateEnum> fsm;
    protected CreatureHealth health;
    protected FlipXCollection flipXItems;
    protected BarCollection bars;
    protected bool flipX;
    public BarBehaviour healthBar;
    private Vector2 axCoeff;
    private Vector3 initScale;

    public PlayerBehaviour player;
    protected StateEnum FsmState
    {
        get => fsm.State;
        set => fsm.State = value;
    }

    public virtual void Die()
    {
        bars.Hide();
        creature.Die();
    }

    protected void Start()
    {
        creature.InitTimers(OnDeathCompleted);
        creature.SetPhysicsFromBehaviour(this);

        fsm = new CreatureFsm<StateEnum>(this);
        fsm.logChanges = creature.logFsmChanges;

        health = new CreatureHealth(healthBar, creature.maxHealth, onZeroHealth: Die);
        bars = new BarCollection();
        bars.Add(healthBar);

        flipXItems = new FlipXCollection();
        flipXItems.Add(bars);
        flipXItems.Add(creature.physics);
    }

    public bool FlipX
    {
        get => flipXItems.FlipX;
        set => flipXItems.FlipX = value;
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
