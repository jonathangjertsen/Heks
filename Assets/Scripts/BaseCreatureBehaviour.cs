using System;
using UnityEngine;

public abstract class BaseCreatureBehaviour<StateEnum> : MonoBehaviour where StateEnum : struct, Enum
{
    protected CreatureFsm<StateEnum> fsm;
    protected CreaturePhysics physics;
    protected CreatureHealth health;
    protected TimerCollection timers;
    protected FlipXCollection flipXItems;
    protected BarCollection bars;

    protected bool flipX;

    public BarBehaviour healthBar;
    public float maxHealth;

    public float axCoeffX = 0.01f;
    public float axCoeffY = 0.03f;
    private Vector2 axCoeff;

    public float rotCoeff = 1f;
    public float maxVelocityX = 10.0f;
    public float maxVelocityY = 10.0f;
    public float maxJerkX = 5f;
    public float maxJerkY = 5f;

    public bool logFsmChanges = false;
    public bool logTimerCallbacks = false;

    private readonly int deathToShrinkStartTimerTop = 100;
    private readonly int shrinkTimerTop = 200;
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
        timers.StopAll();
        timers.Start("deathToShrinkStart");
    }

    protected void Start()
    {
        fsm = new CreatureFsm<StateEnum>(this);
        fsm.logChanges = logFsmChanges;

        physics = new CreaturePhysics(
            this,
            axCoeff: new Vector2(axCoeffX, axCoeffY),
            rotCoeff: rotCoeff,
            maxJerk: new Vector2(maxJerkX, maxJerkY)
        );
        timers = new TimerCollection();
        timers.logCallbacks = logTimerCallbacks;
        timers.Add("deathToShrinkStart", new Timer(deathToShrinkStartTimerTop, ShrinkStart));
        timers.Add("shrink", new Timer(shrinkTimerTop, ShrinkEnd, onTick: Shrinking));

        health = new CreatureHealth(healthBar, maxHealth, onZeroHealth: Die);
        bars = new BarCollection();
        bars.Add(healthBar);

        flipXItems = new FlipXCollection();
        flipXItems.Add(bars);
        flipXItems.Add(physics);
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

    private void ShrinkEnd()
    {
        Destroy(this);
    }

    public bool FlipX
    {
        get => flipXItems.FlipX;
        set => flipXItems.FlipX = value;
    }

    protected void FixedUpdate()
    {
        timers.TickAll();
    }
}
